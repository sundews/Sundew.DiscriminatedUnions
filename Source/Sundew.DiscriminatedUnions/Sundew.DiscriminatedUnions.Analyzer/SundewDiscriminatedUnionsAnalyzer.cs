using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Threading;

namespace Sundew.DiscriminatedUnions.Analyzer
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class SundewDiscriminatedUnionsAnalyzer : DiagnosticAnalyzer
    {
        public const string DiagnosticId = "SDU0001";

        // You can change these strings in the Resources.resx file. If you do not want your analyzer to be localize-able, you can use regular strings for Title and MessageFormat.
        // See https://github.com/dotnet/roslyn/blob/master/docs/analyzers/Localizing%20Analyzers.md for more on localization
        private const string Category = "Naming";

        private static readonly DiagnosticDescriptor AllNotCasesHandledRule = DiagnosticDescriptorHelper.Create(
            DiagnosticId, 
            nameof(Resources.AllCasesNotHandledTitle), 
            nameof(Resources.AllCasesNotHandledMessageFormat), 
            Category, 
            DiagnosticSeverity.Error, 
            true, 
            nameof(Resources.AllCasesNotHandledDescription));

        private static readonly DiagnosticDescriptor SwitchShouldNotHaveDefaultCaseRule = DiagnosticDescriptorHelper.Create(
            DiagnosticId,
            nameof(Resources.SwitchShouldNotHaveDefaultCaseTitle),
            nameof(Resources.SwitchShouldNotHaveDefaultCaseMessageFormat),
            Category,
            DiagnosticSeverity.Error,
            true,
            nameof(Resources.SwitchShouldNotHaveDefaultCaseDescription));

        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics { get { return ImmutableArray.Create(AllNotCasesHandledRule, SwitchShouldNotHaveDefaultCaseRule); } }

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
            var enumType = semanticModel.GetTypeInfo(switchExpressionSyntax.GoverningExpression, context.CancellationToken).Type as INamedTypeSymbol;

            if (!IsDiscriminatedUnionSwitch(enumType))
            {
                return;
            }

            var caseTypes = GetAllTypes(enumType.ContainingNamespace).Where(x => SymbolEqualityComparer.Default.Equals(x.BaseType, enumType)).ToList<ITypeSymbol>();
            var actualCaseTypes = switchExpressionSyntax.Arms.Select(switchExpressionArmSyntax =>
            {
                if (switchExpressionArmSyntax.Pattern is DeclarationPatternSyntax declarationPatternSyntax && switchExpressionArmSyntax.WhenClause == null)
                {
                    var typeInfo = semanticModel.GetTypeInfo(declarationPatternSyntax.Type);
                    return typeInfo.ConvertedType;
                }

                return null;
            }).Where(x => x != null).ToList();

            ReportDiagnostics(caseTypes, actualCaseTypes, context);
        }


        private static void AnalyzeSwitchStatement(SyntaxNodeAnalysisContext context)
        {
            if (!(context.Node is SwitchStatementSyntax switchStatementSyntax))
            {
                return;
            }

            var semanticModel = context.SemanticModel;
            var enumType = semanticModel.GetTypeInfo(switchStatementSyntax.Expression, context.CancellationToken).Type as INamedTypeSymbol;

            if (!IsDiscriminatedUnionSwitch(enumType))
            {
                return;
            }

            var caseTypes = GetAllTypes(enumType.ContainingNamespace).Where(x => SymbolEqualityComparer.Default.Equals(x.BaseType, enumType)).ToList<ITypeSymbol>();
            var actualCaseTypes = switchStatementSyntax.Sections.SelectMany(x => x.Labels.Select(switchLabelSyntax =>
            {
                if (switchLabelSyntax is CasePatternSwitchLabelSyntax casePatternSwitchLabelSyntax)
                {
                    if (casePatternSwitchLabelSyntax.Pattern is DeclarationPatternSyntax declarationPatternSyntax && casePatternSwitchLabelSyntax.WhenClause == null)
                    {
                        return semanticModel.GetTypeInfo(declarationPatternSyntax.Type).ConvertedType;
                    }
                }

                return null;
            })).Where(x => x != null).ToList();

            ReportDiagnostics(caseTypes, actualCaseTypes, context);
        }

        private static void ReportDiagnostics(List<ITypeSymbol> caseTypes, List<ITypeSymbol> actualCaseTypes, SyntaxNodeAnalysisContext context)
        {
            foreach (var actualCaseType in actualCaseTypes)
            {
                if (!caseTypes.Remove(actualCaseType))
                {
                    // Not Possible Case
                }
            }

            foreach (var typeSymbol in caseTypes)
            {
                context.ReportDiagnostic(Diagnostic.Create(AllNotCasesHandledRule, context.Node.GetLocation(), $"{typeSymbol.Name} not handled in case."));
                // missing case
            }
        }


        private static bool IsDiscriminatedUnionSwitch(INamedTypeSymbol enumType)
        {
            //only allow switch on enum types
            if (enumType == null || enumType.TypeKind != TypeKind.Enum && enumType.TypeKind != TypeKind.Class)
            {
                return false;
            }

            // ignore enums marked with Flags
            return enumType.GetAttributes().Any(attribute =>
            {
                var containingClass = attribute.AttributeClass.ToDisplayString();
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
            foreach (var nestedType in type.GetTypeMembers()
                .SelectMany(nestedType => GetNestedTypes(nestedType)))
                yield return nestedType;
        }
    }
}
