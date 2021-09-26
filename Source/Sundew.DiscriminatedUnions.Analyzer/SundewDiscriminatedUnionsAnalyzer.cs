using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis.Operations;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Sundew.DiscriminatedUnions.Analyzer
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class SundewDiscriminatedUnionsAnalyzer : DiagnosticAnalyzer
    {
        public const string AllCasesNotHandledDiagnosticId = "SDU0001";
        // public const string SwitchShouldNotHaveDefaultCaseDiagnosticId = "SDU0002";
        public const string SwitchShouldThrowInDefaultCaseDiagnosticId = "SDU9999";

        // You can change these strings in the Resources.resx file. If you do not want your analyzer to be localize-able, you can use regular strings for Title and MessageFormat.
        // See https://github.com/dotnet/roslyn/blob/master/docs/analyzers/Localizing%20Analyzers.md for more on localization
        private const string Category = "ControlFlow";

        private static readonly DiagnosticDescriptor AllCasesNotHandledRule = DiagnosticDescriptorHelper.Create(
            AllCasesNotHandledDiagnosticId,
            nameof(Resources.AllCasesNotHandledTitle),
            nameof(Resources.AllCasesNotHandledMessageFormat),
            Category,
            DiagnosticSeverity.Error,
            true,
            nameof(Resources.AllCasesNotHandledDescription));

        /*private static readonly DiagnosticDescriptor SwitchShouldNotHaveDefaultCaseRule = DiagnosticDescriptorHelper.Create(
            SwitchShouldNotHaveDefaultCaseDiagnosticId,
            nameof(Resources.SwitchShouldNotHaveDefaultCaseTitle),
            nameof(Resources.SwitchShouldNotHaveDefaultCaseMessageFormat),
            Category,
            DiagnosticSeverity.Error,
            true,
            nameof(Resources.SwitchShouldNotHaveDefaultCaseDescription));*/

        private static readonly DiagnosticDescriptor SwitchShouldThrowInDefaultCaseRule = DiagnosticDescriptorHelper.Create(
            SwitchShouldThrowInDefaultCaseDiagnosticId,
            nameof(Resources.SwitchShouldThrowInDefaultCaseTitle),
            nameof(Resources.SwitchShouldThrowInDefaultCaseMessageFormat),
            Category,
            DiagnosticSeverity.Error,
            true,
            nameof(Resources.SwitchShouldThrowInDefaultCaseDescription));

        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => ImmutableArray.Create(AllCasesNotHandledRule, SwitchShouldThrowInDefaultCaseRule);

        public override void Initialize(AnalysisContext context)
        {
            context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
            context.EnableConcurrentExecution();

            // TODO: Consider registering other actions that act on syntax instead of or in addition to symbols
            // See https://github.com/dotnet/roslyn/blob/master/docs/analyzers/Analyzer%20Actions%20Semantics.md for more information
            context.RegisterOperationAction(AnalyzeSwitchStatement, OperationKind.Switch);
            context.RegisterOperationAction(AnalyzeSwitchExpression, OperationKind.SwitchExpression);
        }

        private static void AnalyzeSwitchExpression(OperationAnalysisContext context)
        {
            bool IsNullCaseMissing(TypeInfo unionTypeInfo, ISwitchExpressionOperation switchExpressionOperation)
            {
                if (unionTypeInfo.Nullability.FlowState != NullableFlowState.NotNull)
                {
                    if (!switchExpressionOperation.Arms.Any(x => x.Pattern is IConstantPatternOperation constantPatternOperation &&
                                                                 constantPatternOperation.Value is IConversionOperation conversionOperation &&
                                                                 conversionOperation.Operand is ILiteralOperation literalOperation &&
                                                                 literalOperation.ConstantValue.HasValue &&
                                                                 literalOperation.ConstantValue.Value == null))
                    {
                        return true;
                    }
                }

                return false;
            }

            if (!(context.Operation is ISwitchExpressionOperation switchExpressionOperation))
            {
                return;
            }

            var unionTypeInfo = switchExpressionOperation.Value.Type;
            var unionType = unionTypeInfo as INamedTypeSymbol;
            if (!IsDiscriminatedUnionSwitch(unionType))
            {
                return;
            }

            var unionTypeWithoutNull = unionType.WithNullableAnnotation(NullableAnnotation.NotAnnotated);
            var caseTypes = GetAllTypes(context.Compilation, unionTypeWithoutNull).ToList<ITypeSymbol>();
            var actualCaseTypes = switchExpressionOperation.Arms.Select(switchExpressionArmOperation =>
            {
                if (switchExpressionArmOperation.Pattern is IDeclarationPatternOperation declarationPatternSyntax)
                {
                    return declarationPatternSyntax.MatchedType;
                }

                if (switchExpressionArmOperation.Pattern is ITypePatternOperation typePatternOperation)
                {
                    return typePatternOperation.MatchedType;
                }

                /* if (switchExpressionArmSyntax.Pattern is DiscardPatternSyntax discardPatternSyntax)
                {
                    context.ReportDiagnostic(Diagnostic.Create(SwitchShouldNotHaveDefaultCaseRule, discardPatternSyntax.GetLocation(), unionType));
                }*/

                if (switchExpressionArmOperation.Pattern is IDiscardPatternOperation discardPatternOperation)
                {
                    if (!(switchExpressionArmOperation.Value is IConversionOperation conversionOperation &&
                          conversionOperation.Operand is IThrowOperation throwOperation &&
                          throwOperation.Exception is IConversionOperation exceptionConversionOperation &&
                          exceptionConversionOperation.Operand.Type!.Name.EndsWith(nameof(DiscriminatedUnionException)) &&
                          exceptionConversionOperation.Operand is IObjectCreationOperation objectCreationOperation &&
                          objectCreationOperation.Arguments.SingleOrDefault(
                              x =>
                                  x.Value is ITypeOfOperation typeOfOperation &&
                                  SymbolEqualityComparer.Default.Equals(typeOfOperation.TypeOperand, unionTypeWithoutNull)) != null))
                    {
                        context.ReportDiagnostic(Diagnostic.Create(SwitchShouldThrowInDefaultCaseRule, discardPatternOperation.Syntax.GetLocation(), unionTypeWithoutNull));
                    }
                }

                return default;
            }).Where(x => x != null).Select(x => x!).ToList();

            if (!switchExpressionOperation.Arms.Any(x => x.Pattern is IDiscardPatternOperation))
            {
                context.ReportDiagnostic(Diagnostic.Create(SwitchShouldThrowInDefaultCaseRule, switchExpressionOperation.Syntax.GetLocation(), unionType));
            }

            var isNullCaseMissing = IsNullCaseMissing(switchExpressionOperation.SemanticModel!.GetTypeInfo(switchExpressionOperation.Value.Syntax), switchExpressionOperation);
            ReportDiagnostics(caseTypes, actualCaseTypes, isNullCaseMissing, context);
        }


        private static void AnalyzeSwitchStatement(OperationAnalysisContext context)
        {
            bool IsNullCaseMissing(TypeInfo unionTypeInfo, ISwitchOperation switchOperation)
            {
                if (unionTypeInfo.Nullability.FlowState != NullableFlowState.NotNull)
                {
                    if (!switchOperation.Cases.Any(x => x.Clauses
                        .Any(x => x is IPatternCaseClauseOperation patternCaseClauseOperation &&
                                  patternCaseClauseOperation.Pattern is IConstantPatternOperation constantPatternOperation &&
                                  constantPatternOperation.Value is IConversionOperation conversionOperation &&
                                  conversionOperation.Operand is ILiteralOperation literalOperation &&
                                  literalOperation.ConstantValue.HasValue && literalOperation.ConstantValue.Value == null)))
                    {
                        return true;
                    }
                }

                return false;
            }

            if (!(context.Operation is ISwitchOperation switchOperation))
            {
                return;
            }

            var unionTypeInfo = switchOperation.Value.Type;
            var unionType = unionTypeInfo as INamedTypeSymbol;

            if (!IsDiscriminatedUnionSwitch(unionType))
            {
                return;
            }

            var unionTypeWithoutNull = unionType.WithNullableAnnotation(NullableAnnotation.NotAnnotated);
            var caseTypes = GetAllTypes(context.Compilation, unionTypeWithoutNull).ToList<ITypeSymbol>();
            var actualCaseTypes = switchOperation.Cases.SelectMany(switchCaseOperation => switchCaseOperation.Clauses.Select(caseClauseOperation =>
            {
                if (caseClauseOperation is IPatternCaseClauseOperation patternCaseClauseOperation)
                {
                    if (patternCaseClauseOperation.Pattern is IDeclarationPatternOperation declarationPatternOperation)
                    {
                        return declarationPatternOperation.MatchedType;
                    }

                    if (patternCaseClauseOperation.Pattern is ITypePatternOperation typePatternOperation)
                    {
                        return typePatternOperation.MatchedType;
                    }
                }

                /*
                if (switchLabelSyntax is DefaultSwitchLabelSyntax defaultSwitchLabelSyntax)
                {
                    context.ReportDiagnostic(Diagnostic.Create(SwitchShouldNotHaveDefaultCaseRule, defaultSwitchLabelSyntax.GetLocation(), unionType));
                }*/

                if (caseClauseOperation is IDefaultCaseClauseOperation defaultCaseClauseOperation)
                {
                    if (!(switchCaseOperation.Body.SingleOrDefault(x => x is IThrowOperation throwOperation &&
                          throwOperation.Exception is IConversionOperation exceptionConversionOperation &&
                          exceptionConversionOperation.Operand.Type!.Name.EndsWith(nameof(DiscriminatedUnionException)) &&
                          exceptionConversionOperation.Operand is IObjectCreationOperation objectCreationOperation &&
                          objectCreationOperation.Arguments.SingleOrDefault(
                              x =>
                                  x.Value is ITypeOfOperation typeOfOperation &&
                                  SymbolEqualityComparer.Default.Equals(typeOfOperation.TypeOperand, unionTypeWithoutNull)) != null) != null))
                    {
                        context.ReportDiagnostic(Diagnostic.Create(SwitchShouldThrowInDefaultCaseRule, defaultCaseClauseOperation.Syntax.GetLocation(), unionTypeWithoutNull));
                    }
                }

                return null;
            })).Where(x => x != null).Select(x => x!).ToList();
            var isNullCaseMissing = IsNullCaseMissing(switchOperation.SemanticModel!.GetTypeInfo(switchOperation.Value.Syntax), switchOperation);
            ReportDiagnostics(caseTypes, actualCaseTypes, isNullCaseMissing, context);
        }

        private static void ReportDiagnostics(
            List<ITypeSymbol> caseTypes,
            List<ITypeSymbol> actualCaseTypes,
            bool isNullCaseMissing,
            OperationAnalysisContext context)
        {
            foreach (var actualCaseType in actualCaseTypes)
            {
                if (!caseTypes.Remove(actualCaseType))
                {
                    // Not Possible Case
                }
            }

            if (caseTypes.Any() || isNullCaseMissing)
            {
                context.ReportDiagnostic(Diagnostic.Create(AllCasesNotHandledRule, context.Operation.Syntax.GetLocation(),
                    caseTypes.Aggregate(new StringBuilder(),
                        (stringBuilder, typeSymbol) => stringBuilder.Append(typeSymbol.Name).Append(',').Append(' '),
                        builder => isNullCaseMissing ? builder.Append("null").ToString() : builder.ToString(0, builder.Length - 2))));
            }
        }


        private static bool IsDiscriminatedUnionSwitch([NotNullWhen(true)] INamedTypeSymbol? unionType)
        {
            //only allow switch on enum types
            if (unionType == null || unionType.TypeKind != TypeKind.Class)
            {
                return false;
            }

            return unionType.GetAttributes().Any(attribute =>
            {
                var containingClass = attribute.AttributeClass?.ToDisplayString();
                return containingClass == typeof(Sundew.DiscriminatedUnions.DiscriminatedUnion).FullName;
            });
        }


        private static IEnumerable<INamedTypeSymbol> GetAllTypes(Compilation compilation, ITypeSymbol baseClassTypeSymbol) =>
            GetAllTypes(compilation.GlobalNamespace, baseClassTypeSymbol);

        private static IEnumerable<INamedTypeSymbol> GetAllTypes(INamespaceSymbol @namespace, ITypeSymbol baseClassTypeSymbol)
        {
            foreach (var type in @namespace.GetTypeMembers())
            {
                foreach (var nestedType in GetNestedTypes(type))
                {
                    if (SymbolEqualityComparer.Default.Equals(baseClassTypeSymbol, nestedType.BaseType))
                    {
                        yield return nestedType;
                    }
                }
            }

            foreach (var nestedNamespace in @namespace.GetNamespaceMembers())
            {
                foreach (var type in GetAllTypes(nestedNamespace, baseClassTypeSymbol))
                {
                    yield return type;
                }
            }
        }

        private static IEnumerable<INamedTypeSymbol> GetNestedTypes(INamedTypeSymbol type)
        {
            yield return type;
            foreach (var memberType in type.GetTypeMembers())
            {
                foreach (var nestedType in GetNestedTypes(memberType))
                {
                    yield return nestedType;
                }
            }
        }
    }
}
