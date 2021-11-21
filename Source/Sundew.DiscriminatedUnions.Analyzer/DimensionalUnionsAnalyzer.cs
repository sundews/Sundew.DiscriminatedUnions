// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DimensionalUnionsAnalyzer.cs" company="Hukano">
// Copyright (c) Hukano. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Sundew.DiscriminatedUnions.Analyzer;

using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;
using Sundew.DiscriminatedUnions.Analyzer.SwitchExpression;
using Sundew.DiscriminatedUnions.Analyzer.SwitchStatement;

/// <summary>
/// Discriminated Union analyzer that ensures all cases in switch statements and expression are handled and that the code defining the discriminated union is a closed inheritance hierarchy.
/// </summary>
/// <seealso cref="Microsoft.CodeAnalysis.Diagnostics.DiagnosticAnalyzer" />
[DiagnosticAnalyzer(LanguageNames.CSharp)]
public class DimensionalUnionsAnalyzer : DiagnosticAnalyzer
{
    /// <summary>
    /// Diagnostic id indicating that all cases are not handled diagnostic.
    /// </summary>
    public const string SwitchAllCasesNotHandledDiagnosticId = "SDU0001";

    /// <summary>
    /// Diagnostic id indicating that the switch should not have default case.
    /// </summary>
    public const string SwitchShouldNotHaveDefaultCaseDiagnosticId = "SDU0002";

    /// <summary>
    /// Diagnostic id indicating that the switch has an unreachable null case.
    /// </summary>
    public const string SwitchHasUnreachableNullCaseDiagnosticId = "SDU0003";

    /// <summary>
    /// Diagnostic id indicating that class unions must be abstract.
    /// </summary>
    public const string ClassUnionsMustBeAbstractDiagnosticId = "SDU0004";

    /// <summary>
    /// Diagnostic id indicating that a union cannot be extended outside its assembly.
    /// </summary>
    public const string UnionsCannotBeExtendedOutsideItsAssemblyDiagnosticId = "SDU0005";

    /// <summary>
    /// Diagnostic id indicating that cases must be declared in the same assembly as its union.
    /// </summary>
    public const string CasesMustBeDeclaredInUnionAssemblyDiagnosticId = "SDU0006";

    /// <summary>
    /// Diagnostic id indicating that cases should be sealed.
    /// </summary>
    public const string CasesShouldBeSealedDiagnosticId = "SDU0007";

    /// <summary>
    /// Diagnostic id indicating that unnested cases should have a factory method in its unions.
    /// </summary>
    public const string UnnestedCasesShouldHaveFactoryMethodDiagnosticId = "SDU0008";

    /// <summary>
    /// Diagnostic id indicating that the switch should throw in default case.
    /// </summary>
    public const string SwitchShouldThrowInDefaultCaseDiagnosticId = "SDU9999";

    /// <summary>
    /// All cases not handled rule.
    /// </summary>
    public static readonly DiagnosticDescriptor SwitchAllCasesNotHandledRule = DiagnosticDescriptorHelper.Create(
        SwitchAllCasesNotHandledDiagnosticId,
        nameof(Resources.SwitchAllCasesNotHandledTitle),
        nameof(Resources.SwitchAllCasesNotHandledMessageFormat),
        Category,
        DiagnosticSeverity.Error,
        true,
        nameof(Resources.SwitchAllCasesNotHandledDescription));

    /// <summary>
    /// The switch should not have default case rule.
    /// </summary>
    public static readonly DiagnosticDescriptor SwitchShouldNotHaveDefaultCaseRule =
        DiagnosticDescriptorHelper.Create(
            SwitchShouldNotHaveDefaultCaseDiagnosticId,
            nameof(Resources.SwitchShouldNotHaveDefaultCaseTitle),
            nameof(Resources.SwitchShouldNotHaveDefaultCaseMessageFormat),
            Category,
            DiagnosticSeverity.Error,
            true,
            nameof(Resources.SwitchShouldNotHaveDefaultCaseDescription));

    /// <summary>
    /// The switch should throw in default case rule.
    /// </summary>
    public static readonly DiagnosticDescriptor SwitchHasUnreachableNullCaseRule =
        DiagnosticDescriptorHelper.Create(
            SwitchHasUnreachableNullCaseDiagnosticId,
            nameof(Resources.SwitchHasUnreachableNullCaseTitle),
            nameof(Resources.SwitchHasUnreachableNullCaseMessageFormat),
            Category,
            DiagnosticSeverity.Error,
            true,
            nameof(Resources.SwitchHasUnreachableNullCaseDescription));

    /// <summary>
    /// The discriminated union base must be abstract.
    /// </summary>
    public static readonly DiagnosticDescriptor ClassUnionsMustBeAbstractRule =
        DiagnosticDescriptorHelper.Create(
            ClassUnionsMustBeAbstractDiagnosticId,
            nameof(Resources.ClassUnionsMustBeAbstractTitle),
            nameof(Resources.ClassUnionsMustBeAbstractMessageFormat),
            Category,
            DiagnosticSeverity.Error,
            true,
            nameof(Resources.ClassDiscriminatedUnionsMustBeAbstractDescription));

    /// <summary>
    /// The union must have private protected constructor rule.
    /// </summary>
    public static readonly DiagnosticDescriptor UnionsCannotBeExtendedOutsideItsAssemblyRule =
        DiagnosticDescriptorHelper.Create(
            UnionsCannotBeExtendedOutsideItsAssemblyDiagnosticId,
            nameof(Resources.UnionsCannotBeExtendedOutsideItsAssemblyTitle),
            nameof(Resources.UnionsCannotBeExtendedOutsideItsAssemblyMessageFormat),
            Category,
            DiagnosticSeverity.Error,
            true,
            nameof(Resources.UnionsCannotBeExtendedOutsideItsAssemblyDescription));

    /// <summary>
    /// The union base interface must be internal.
    /// </summary>
    public static readonly DiagnosticDescriptor CasesMustBeDeclaredInUnionAssemblyRule =
        DiagnosticDescriptorHelper.Create(
            CasesMustBeDeclaredInUnionAssemblyDiagnosticId,
            nameof(Resources.CasesMustBeDeclaredInUnionAssemblyTitle),
            nameof(Resources.CasesMustBeDeclaredInUnionAssemblyMessageFormat),
            Category,
            DiagnosticSeverity.Error,
            true,
            nameof(Resources.CasesMustBeDeclaredInUnionAssemblyDescription));

    /// <summary>
    /// The cases should be sealed rule.
    /// </summary>
    public static readonly DiagnosticDescriptor CasesShouldBeSealedRule = DiagnosticDescriptorHelper.Create(
        CasesShouldBeSealedDiagnosticId,
        nameof(Resources.CasesShouldBeSealedTitle),
        nameof(Resources.CasesShouldBeSealedMessageFormat),
        Category,
        DiagnosticSeverity.Error,
        true,
        nameof(Resources.CasesShouldBeSealedDescription));

    /// <summary>
    /// The switch should throw in default case rule.
    /// </summary>
    public static readonly DiagnosticDescriptor UnnestedCasesShouldHaveFactoryMethodRule =
        DiagnosticDescriptorHelper.Create(
            UnnestedCasesShouldHaveFactoryMethodDiagnosticId,
            nameof(Resources.UnnestedCasesShouldHaveFactoryMethodTitle),
            nameof(Resources.UnnestedCasesShouldHaveFactoryMethodMessageFormat),
            Category,
            DiagnosticSeverity.Error,
            true,
            nameof(Resources.UnnestedCasesShouldHaveFactoryMethodDescription));

    /// <summary>
    /// The switch should throw in default case rule.
    /// </summary>
    public static readonly DiagnosticDescriptor SwitchShouldThrowInDefaultCaseRule =
        DiagnosticDescriptorHelper.Create(
            SwitchShouldThrowInDefaultCaseDiagnosticId,
            nameof(Resources.SwitchShouldThrowInDefaultCaseTitle),
            nameof(Resources.SwitchShouldThrowInDefaultCaseMessageFormat),
            Category,
            DiagnosticSeverity.Error,
            true,
            nameof(Resources.SwitchShouldThrowInDefaultCaseDescription));

    private const string Category = "ControlFlow";

    /// <summary>
    /// Gets a set of descriptors for the diagnostics that this analyzer is capable of producing.
    /// </summary>
    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => ImmutableArray.Create(
        SwitchAllCasesNotHandledRule,
        SwitchShouldNotHaveDefaultCaseRule,
        SwitchHasUnreachableNullCaseRule,
        ClassUnionsMustBeAbstractRule,
        UnionsCannotBeExtendedOutsideItsAssemblyRule,
        CasesMustBeDeclaredInUnionAssemblyRule,
        CasesShouldBeSealedRule,
        UnnestedCasesShouldHaveFactoryMethodRule,
        SwitchShouldThrowInDefaultCaseRule);

    /// <summary>
    /// Called once at session start to register actions in the analysis context.
    /// </summary>
    /// <param name="context">The context.</param>
    public override void Initialize(AnalysisContext context)
    {
        context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
        context.EnableConcurrentExecution();

        var unionBaseAnalyzer = new UnionBaseAnalyzer();
        var unionCaseAnalyzer = new UnionCaseAnalyzer();
        var unionSwitchExpressionAnalyzer = new UnionSwitchExpressionAnalyzer();
        var unionSwitchStatementAnalyzer = new UnionSwitchStatementAnalyzer();
        context.RegisterSymbolAction(
            symbolAnalysisContext =>
            {
                if (symbolAnalysisContext.Symbol is not INamedTypeSymbol namedTypeSymbol)
                {
                    return;
                }

                unionBaseAnalyzer.AnalyzeSymbol(namedTypeSymbol, symbolAnalysisContext.ReportDiagnostic);
                unionCaseAnalyzer.AnalyzeSymbol(namedTypeSymbol, symbolAnalysisContext.ReportDiagnostic);
            },
            SymbolKind.NamedType);
        context.RegisterOperationAction(unionSwitchExpressionAnalyzer.Analyze, OperationKind.SwitchExpression);
        context.RegisterOperationAction(unionSwitchStatementAnalyzer.Analyze, OperationKind.Switch);
    }
}