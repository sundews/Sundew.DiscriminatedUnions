// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SundewDiscriminatedUnionsAnalyzer.cs" company="Hukano">
// Copyright (c) Hukano. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Sundew.DiscriminatedUnions.Analyzer
{
    using System.Collections.Immutable;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.Diagnostics;

    /// <summary>
    /// Discriminated Union analyzer that ensures all cases in switch statements and expression are handled and that the code defining the discriminated union is a closed inheritance hierarchy.
    /// </summary>
    /// <seealso cref="Microsoft.CodeAnalysis.Diagnostics.DiagnosticAnalyzer" />
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class SundewDiscriminatedUnionsAnalyzer : DiagnosticAnalyzer
    {
        /// <summary>
        /// Diagnostic id indicating that all cases are not handled diagnostic.
        /// </summary>
        public const string AllCasesNotHandledDiagnosticId = "SDU0001";

        /// <summary>
        /// Diagnostic id indicating that the switch should not have default case.
        /// </summary>
        public const string SwitchShouldNotHaveDefaultCaseDiagnosticId = "SDU0002";

        /// <summary>
        /// Diagnostic id indicating that discriminated union can only have private protected constructors.
        /// </summary>
        public const string DiscriminatedUnionCanOnlyHavePrivateProtectedConstructorsDiagnosticId = "SDU0003";

        /// <summary>
        /// Diagnostic id indicating that the case should be sealed.
        /// </summary>
        public const string CasesShouldBeSealedDiagnosticId = "SDU0004";

        /// <summary>
        /// Diagnostic id indicating that the case should be nested within the discriminated union.
        /// </summary>
        public const string CasesShouldBeNestedDiagnosticId = "SDU0005";

        /// <summary>
        /// Diagnostic id indicating that the switch has an unreachable null case.
        /// </summary>
        public const string HasUnreachableNullCaseDiagnosticId = "SDU0006";

        /// <summary>
        /// Diagnostic id indicating that the switch should throw in default case.
        /// </summary>
        public const string SwitchShouldThrowInDefaultCaseDiagnosticId = "SDU9999";

        /// <summary>
        /// All cases not handled rule.
        /// </summary>
        public static readonly DiagnosticDescriptor AllCasesNotHandledRule = DiagnosticDescriptorHelper.Create(
            AllCasesNotHandledDiagnosticId,
            nameof(Resources.AllCasesNotHandledTitle),
            nameof(Resources.AllCasesNotHandledMessageFormat),
            Category,
            DiagnosticSeverity.Error,
            true,
            nameof(Resources.AllCasesNotHandledDescription));

        /// <summary>
        /// The switch should not have default case rule.
        /// </summary>
        public static readonly DiagnosticDescriptor SwitchShouldNotHaveDefaultCaseRule = DiagnosticDescriptorHelper.Create(
            SwitchShouldNotHaveDefaultCaseDiagnosticId,
            nameof(Resources.SwitchShouldNotHaveDefaultCaseTitle),
            nameof(Resources.SwitchShouldNotHaveDefaultCaseMessageFormat),
            Category,
            DiagnosticSeverity.Error,
            true,
            nameof(Resources.SwitchShouldNotHaveDefaultCaseDescription));

        /// <summary>
        /// The discriminated union can only have private constructors rule.
        /// </summary>
        public static readonly DiagnosticDescriptor DiscriminatedUnionCanOnlyHavePrivateConstructorsRule =
            DiagnosticDescriptorHelper.Create(
                DiscriminatedUnionCanOnlyHavePrivateProtectedConstructorsDiagnosticId,
                nameof(Resources.DiscriminatedUnionCanOnlyHavePrivateProtectedConstructorsTitle),
                nameof(Resources.DiscriminatedUnionCanOnlyHavePrivateProtectedConstructorsMessageFormat),
                Category,
                DiagnosticSeverity.Error,
                true,
                nameof(Resources.DiscriminatedUnionCanOnlyHavePrivateProtectedConstructorsDescription));

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
        /// The cases should nested rule.
        /// </summary>
        public static readonly DiagnosticDescriptor CasesShouldNestedRule = DiagnosticDescriptorHelper.Create(
            CasesShouldBeNestedDiagnosticId,
            nameof(Resources.CasesShouldBeNestedTitle),
            nameof(Resources.CasesShouldBeNestedMessageFormat),
            Category,
            DiagnosticSeverity.Error,
            true,
            nameof(Resources.CasesShouldBeNestedDescription));

        /// <summary>
        /// The switch should throw in default case rule.
        /// </summary>
        public static readonly DiagnosticDescriptor HasUnreachableNullCaseRule =
            DiagnosticDescriptorHelper.Create(
                HasUnreachableNullCaseDiagnosticId,
                nameof(Resources.HasUnreachableNullCaseTitle),
                nameof(Resources.HasUnreachableNullCaseMessageFormat),
                Category,
                DiagnosticSeverity.Error,
                true,
                nameof(Resources.HasUnreachableNullCaseDescription));

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
            AllCasesNotHandledRule,
            SwitchShouldNotHaveDefaultCaseRule,
            DiscriminatedUnionCanOnlyHavePrivateConstructorsRule,
            CasesShouldBeSealedRule,
            CasesShouldNestedRule,
            HasUnreachableNullCaseRule,
            SwitchShouldThrowInDefaultCaseRule);

        /// <summary>
        /// Called once at session start to register actions in the analysis context.
        /// </summary>
        /// <param name="context">The context.</param>
        public override void Initialize(AnalysisContext context)
        {
            context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
            context.EnableConcurrentExecution();

            var discriminatedUnionTypeAnalyzer = new DiscriminatedUnionTypeAnalyzer();
            context.RegisterSymbolAction(analysisContext => discriminatedUnionTypeAnalyzer.AnalyzeDiscriminatedUnionsTypes(analysisContext), SymbolKind.NamedType);
            var discriminatedUnionSwitchAnalyzer = new DiscriminatedUnionSwitchAnalyzer();
            context.RegisterOperationAction(discriminatedUnionSwitchAnalyzer.AnalyzeSwitchExpression, OperationKind.SwitchExpression);
            context.RegisterOperationAction(discriminatedUnionSwitchAnalyzer.AnalyzeSwitchStatement, OperationKind.Switch);
        }
    }
}