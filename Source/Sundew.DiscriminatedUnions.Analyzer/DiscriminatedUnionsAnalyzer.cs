// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DiscriminatedUnionsAnalyzer.cs" company="Hukano">
// Copyright (c) Hukano. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Sundew.DiscriminatedUnions.Analyzer;

using System.Collections.Generic;
using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;
using Sundew.DiscriminatedUnions.Analyzer.Analyzers;
using Sundew.DiscriminatedUnions.Analyzer.FactoryMethod;
using Sundew.DiscriminatedUnions.Analyzer.SwitchExpression;
using Sundew.DiscriminatedUnions.Analyzer.SwitchStatement;
using Sundew.DiscriminatedUnions.Shared;

/// <summary>
/// Discriminated Union analyzer that ensures all cases in switch statements and expression are handled and that the code defining the discriminated union is a closed inheritance hierarchy.
/// </summary>
/// <seealso cref="Microsoft.CodeAnalysis.Diagnostics.DiagnosticAnalyzer" />
[DiagnosticAnalyzer(LanguageNames.CSharp)]
public class DiscriminatedUnionsAnalyzer : DiagnosticAnalyzer
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
    public const string OnlyUnionsCanExtendOtherUnionsDiagnosticId = "SDU0005";

    /// <summary>
    /// Diagnostic id indicating that a union cannot be extended outside their assembly.
    /// </summary>
    public const string UnionsCannotBeExtendedOutsideTheirAssemblyDiagnosticId = "SDU0006";

    /// <summary>
    /// Diagnostic id indicating that cases must be declared in the same assembly as its union.
    /// </summary>
    public const string CasesMustBeDeclaredInUnionAssemblyDiagnosticId = "SDU0007";

    /// <summary>
    /// Diagnostic id indicating that cases should be sealed.
    /// </summary>
    public const string CasesShouldBeSealedDiagnosticId = "SDU0008";

    /// <summary>
    /// Diagnostic id indicating that unnested cases should have a factory method in its unions.
    /// </summary>
    public const string UnnestedCasesShouldHaveFactoryMethodDiagnosticId = "SDU0009";

    /// <summary>
    /// Diagnostic id indicating that factory method should have a matching CaseTypeAttribute.
    /// </summary>
    public const string FactoryMethodShouldHaveMatchingCaseTypeAttributeDiagnosticId = "SDU0010";

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
    /// The discriminated union base must be abstract rule.
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
    /// The only unions can extend other unions rule.
    /// </summary>
    public static readonly DiagnosticDescriptor OnlyUnionsCanExtendOtherUnionsRule =
        DiagnosticDescriptorHelper.Create(
            OnlyUnionsCanExtendOtherUnionsDiagnosticId,
            nameof(Resources.OnlyUnionsCanExtendOtherUnionsTitle),
            nameof(Resources.OnlyUnionsCanExtendOtherUnionsMessageFormat),
            Category,
            DiagnosticSeverity.Error,
            true,
            nameof(Resources.OnlyUnionsCanExtendOtherUnionsDescription));

    /// <summary>
    /// The union cannot be extended outside its assembly rule.
    /// </summary>
    public static readonly DiagnosticDescriptor UnionsCannotBeExtendedOutsideTheirAssemblyRule =
        DiagnosticDescriptorHelper.Create(
            UnionsCannotBeExtendedOutsideTheirAssemblyDiagnosticId,
            nameof(Resources.UnionsCannotBeExtendedOutsideTheirAssemblyTitle),
            nameof(Resources.UnionsCannotBeExtendedOutsideTheirAssemblyMessageFormat),
            Category,
            DiagnosticSeverity.Error,
            true,
            nameof(Resources.UnionsCannotBeExtendedOutsideTheirAssemblyDescription));

    /// <summary>
    /// The case must be declared in its union assembly rule.
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
    /// The unnested case should have factory method rule.
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
    /// The factory method should have a matching CaseTypeAttribute rule.
    /// </summary>
    public static readonly DiagnosticDescriptor FactoryMethodShouldHaveMatchingCaseTypeAttributeRule =
        DiagnosticDescriptorHelper.Create(
            FactoryMethodShouldHaveMatchingCaseTypeAttributeDiagnosticId,
            nameof(Resources.FactoryMethodShouldHaveMatchingCaseTypeAttributeTitle),
            nameof(Resources.FactoryMethodShouldHaveMatchingCaseTypeAttributeMessageFormat),
            Category,
            DiagnosticSeverity.Error,
            true,
            nameof(Resources.FactoryMethodShouldHaveMatchingCaseTypeAttributeDescription));

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
        OnlyUnionsCanExtendOtherUnionsRule,
        UnionsCannotBeExtendedOutsideTheirAssemblyRule,
        CasesMustBeDeclaredInUnionAssemblyRule,
        CasesShouldBeSealedRule,
        UnnestedCasesShouldHaveFactoryMethodRule,
        FactoryMethodShouldHaveMatchingCaseTypeAttributeRule,
        SwitchShouldThrowInDefaultCaseRule);

    /// <summary>
    /// Called once at session start to register actions in the analysis context.
    /// </summary>
    /// <param name="context">The context.</param>
    public override void Initialize(AnalysisContext context)
    {
        context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.Analyze);
        context.EnableConcurrentExecution();

        var unionSymbolAnalyzers = new List<IUnionSymbolAnalyzer>
        {
            new ClassUnionsMustBeAbstractAnalyzer(),
            new OnlyUnionsCanExtendOtherUnionsAnalyzer(),
            new UnionsCannotBeExtendedOutsideTheirAssemblyAnalyzer(),
            new UnionCaseAnalyzer(),
        };

        foreach (var unionSymbolAnalyzer in unionSymbolAnalyzers)
        {
            context.RegisterSymbolAction(
                symbolAnalysisContext =>
                {
                    if (symbolAnalysisContext.Symbol is not INamedTypeSymbol namedTypeSymbol)
                    {
                        return;
                    }

                    unionSymbolAnalyzer.AnalyzeSymbol(namedTypeSymbol, symbolAnalysisContext.ReportDiagnostic);
                },
                SymbolKind.NamedType);
        }

        var factoryMethodAnalyzer = new FactoryMethodAnalyzer();
        context.RegisterSymbolAction(factoryMethodAnalyzer.Analyze, SymbolKind.Method);

        var unionSwitchExpressionAnalyzer = new UnionSwitchExpressionAnalyzer();
        var unionSwitchStatementAnalyzer = new UnionSwitchStatementAnalyzer();
        context.RegisterOperationAction(unionSwitchExpressionAnalyzer.Analyze, OperationKind.SwitchExpression);
        context.RegisterOperationAction(unionSwitchStatementAnalyzer.Analyze, OperationKind.Switch);
    }
}