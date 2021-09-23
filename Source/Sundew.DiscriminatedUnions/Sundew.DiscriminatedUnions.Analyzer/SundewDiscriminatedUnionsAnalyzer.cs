using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading;

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
        private const string Category = "Naming";

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
            context.RegisterSyntaxNodeAction(AnalyzeSwitchStatement, SyntaxKind.SwitchStatement);
            context.RegisterSyntaxNodeAction(AnalyzeSwitchExpression, SyntaxKind.SwitchExpression);
        }

        private static void AnalyzeSwitchExpression(SyntaxNodeAnalysisContext context)
        {
            if (!(context.Node is SwitchExpressionSyntax switchExpressionSyntax))
            {
                return;
            }

            var semanticModel = context.SemanticModel;
            var unionType = semanticModel.GetTypeInfo(switchExpressionSyntax.GoverningExpression, context.CancellationToken).Type as INamedTypeSymbol;
            if (!IsDiscriminatedUnionSwitch(unionType))
            {
                return;
            }

            var caseTypes = GetAllTypes(context.Compilation).Where(x => SymbolEqualityComparer.Default.Equals(x.BaseType, unionType)).ToList<ITypeSymbol>();
            var actualCaseTypes = switchExpressionSyntax.Arms.Select(switchExpressionArmSyntax =>
            {
                if (switchExpressionArmSyntax.Pattern is DeclarationPatternSyntax declarationPatternSyntax && switchExpressionArmSyntax.WhenClause == null)
                {
                    var typeInfo = semanticModel.GetTypeInfo(declarationPatternSyntax.Type);
                    return typeInfo.ConvertedType;
                }

                if (switchExpressionArmSyntax.Pattern is ConstantPatternSyntax constantPatternSyntax &&
                    constantPatternSyntax.Expression is IdentifierNameSyntax constantIdentifierNameSyntax)
                {
                    var typeInfo = semanticModel.GetTypeInfo(constantIdentifierNameSyntax);
                    return typeInfo.Type;
                }

                /* if (switchExpressionArmSyntax.Pattern is DiscardPatternSyntax discardPatternSyntax)
                {
                    context.ReportDiagnostic(Diagnostic.Create(SwitchShouldNotHaveDefaultCaseRule, discardPatternSyntax.GetLocation(), unionType));
                }*/

                if (switchExpressionArmSyntax.Pattern is DiscardPatternSyntax discardPatternSyntax)
                {
                    if (!(switchExpressionArmSyntax.Expression is ThrowExpressionSyntax throwExpressionSyntax &&
                        throwExpressionSyntax.Expression is ObjectCreationExpressionSyntax objectCreationExpressionSyntax &&
                        objectCreationExpressionSyntax.Type is IdentifierNameSyntax typeIdentifierNameSyntax &&
                        typeIdentifierNameSyntax.Identifier.ValueText == nameof(DiscriminatedUnionException) &&
                            objectCreationExpressionSyntax.ArgumentList?.Arguments.SingleOrDefault(
                                x =>
                                 x.Expression is IdentifierNameSyntax identifierNameSyntax &&
                                           identifierNameSyntax.Identifier.ValueText == switchExpressionSyntax.GoverningExpression.ToString()) != null))
                    {
                        context.ReportDiagnostic(Diagnostic.Create(SwitchShouldThrowInDefaultCaseRule, discardPatternSyntax.GetLocation(), unionType));
                    }
                }

                return null;
            }).Where(x => x != null).Select(x => x!).ToList();

            if (!switchExpressionSyntax.Arms.Any(x => x.Pattern is DiscardPatternSyntax))
            {
                context.ReportDiagnostic(Diagnostic.Create(SwitchShouldThrowInDefaultCaseRule, switchExpressionSyntax.GetLocation(), unionType));
            }

            ReportDiagnostics(caseTypes, actualCaseTypes, context);
        }


        private static void AnalyzeSwitchStatement(SyntaxNodeAnalysisContext context)
        {
            if (!(context.Node is SwitchStatementSyntax switchStatementSyntax))
            {
                return;
            }

            var semanticModel = context.SemanticModel;
            var unionType = semanticModel.GetTypeInfo(switchStatementSyntax.Expression, context.CancellationToken).Type as INamedTypeSymbol;

            if (!IsDiscriminatedUnionSwitch(unionType))
            {
                return;
            }

            var caseTypes = GetAllTypes(context.Compilation).Where(x => SymbolEqualityComparer.Default.Equals(x.BaseType, unionType)).ToList<ITypeSymbol>();
            var actualCaseTypes = switchStatementSyntax.Sections.SelectMany(x => x.Labels.Select(switchLabelSyntax =>
            {
                if (switchLabelSyntax is CasePatternSwitchLabelSyntax casePatternSwitchLabelSyntax)
                {
                    if (casePatternSwitchLabelSyntax.Pattern is DeclarationPatternSyntax declarationPatternSyntax && casePatternSwitchLabelSyntax.WhenClause == null)
                    {
                        return semanticModel.GetTypeInfo(declarationPatternSyntax.Type).ConvertedType;
                    }
                }

                if (switchLabelSyntax is CaseSwitchLabelSyntax caseSwitchLabelSyntax &&
                    caseSwitchLabelSyntax.Value is IdentifierNameSyntax constantIdentifierNameSyntax)
                {
                    var typeInfo = semanticModel.GetTypeInfo(constantIdentifierNameSyntax);
                    return typeInfo.Type;
                }

                /*if (switchLabelSyntax is DefaultSwitchLabelSyntax defaultSwitchLabelSyntax)
                {
                    context.ReportDiagnostic(Diagnostic.Create(SwitchShouldNotHaveDefaultCaseRule, defaultSwitchLabelSyntax.GetLocation(), unionType));
                }*/

                if (switchLabelSyntax is DefaultSwitchLabelSyntax defaultSwitchLabelSyntax)
                {
                    if (!(x.Statements.SingleOrDefault(x => x is ThrowStatementSyntax throwStatementSyntax &&
                          throwStatementSyntax.Expression is ObjectCreationExpressionSyntax objectCreationExpressionSyntax &&
                          objectCreationExpressionSyntax.Type is IdentifierNameSyntax typeIdentifierNameSyntax &&
                          typeIdentifierNameSyntax.Identifier.ValueText == nameof(DiscriminatedUnionException) &&
                          objectCreationExpressionSyntax.ArgumentList?.Arguments.SingleOrDefault(
                              x =>
                                  x.Expression is IdentifierNameSyntax identifierNameSyntax &&
                                  identifierNameSyntax.Identifier.ValueText == switchStatementSyntax.Expression.ToString()) != null) != null))
                    {
                        context.ReportDiagnostic(Diagnostic.Create(SwitchShouldThrowInDefaultCaseRule, defaultSwitchLabelSyntax.GetLocation(), unionType));
                    }
                }

                return null;
            })).Where(x => x != null).Select(x => x!).ToList();

            ReportDiagnostics(caseTypes, actualCaseTypes, context);
        }

        private static void ReportDiagnostics(List<ITypeSymbol> caseTypes, List<ITypeSymbol> actualCaseTypes,
            SyntaxNodeAnalysisContext context)
        {
            foreach (var actualCaseType in actualCaseTypes)
            {
                if (!caseTypes.Remove(actualCaseType))
                {
                    // Not Possible Case
                }
            }

            if (caseTypes.Any())
            {
                context.ReportDiagnostic(Diagnostic.Create(AllCasesNotHandledRule, context.Node.GetLocation(),
                    caseTypes.Aggregate(new StringBuilder(),
                        (stringBuilder, typeSymbol) => stringBuilder.Append(typeSymbol.Name).Append(',').Append(' '),
                        builder => builder.ToString(0, builder.Length - 2))));
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


        private static IEnumerable<INamedTypeSymbol> GetAllTypes(Compilation compilation) =>
            GetAllTypes(compilation.GlobalNamespace);

        private static IEnumerable<INamedTypeSymbol> GetAllTypes(INamespaceSymbol @namespace)
        {
            foreach (var type in @namespace.GetTypeMembers())
            {
                foreach (var nestedType in GetNestedTypes(type))
                {
                    yield return nestedType;
                }
            }

            foreach (var nestedNamespace in @namespace.GetNamespaceMembers())
            {
                foreach (var type in GetAllTypes(nestedNamespace))
                {
                    yield return type;
                }
            }
        }

        private static IEnumerable<INamedTypeSymbol> GetNestedTypes(INamedTypeSymbol type)
        {
            yield return type;
            foreach (var nestedType in type.GetTypeMembers().SelectMany(GetNestedTypes))
            {
                yield return nestedType;
            }
        }
    }
}
