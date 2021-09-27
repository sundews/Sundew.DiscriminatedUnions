// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SundewDiscriminatedUnionsAnalyzer.cs" company="Hukano">
// Copyright (c) Hukano. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Sundew.DiscriminatedUnions.Analyzer
{
    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.Collections.Immutable;
    using System.Diagnostics.CodeAnalysis;
    using System.Linq;
    using System.Text;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.Diagnostics;
    using Microsoft.CodeAnalysis.Operations;

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

        //// public const string SwitchShouldNotHaveDefaultCaseDiagnosticId = "SDU0002";

        /// <summary>
        /// Diagnostic id indicating that discriminated union can only have private protected constructors.
        /// </summary>
        public const string DiscriminatedUnionCanOnlyHavePrivateProtectedConstructorsDiagnosticId = "SDU9997";

        /// <summary>
        /// Diagnostic id indicating that the cases should be sealed.
        /// </summary>
        public const string CasesShouldBeSealedDiagnosticId = "SDU9998";

        /// <summary>
        /// Diagnostic id indicating that the switch should throw in default case.
        /// </summary>
        public const string SwitchShouldThrowInDefaultCaseDiagnosticId = "SDU9999";

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

        private static readonly DiagnosticDescriptor DiscriminatedUnionCanOnlyHavePrivateProtectedConstructorsRule = DiagnosticDescriptorHelper.Create(
            DiscriminatedUnionCanOnlyHavePrivateProtectedConstructorsDiagnosticId,
            nameof(Resources.DiscriminatedUnionCanOnlyHavePrivateProtectedConstructorsTitle),
            nameof(Resources.DiscriminatedUnionCanOnlyHavePrivateProtectedConstructorsMessageFormat),
            Category,
            DiagnosticSeverity.Error,
            true,
            nameof(Resources.DiscriminatedUnionCanOnlyHavePrivateProtectedConstructorsDescription));

        private static readonly DiagnosticDescriptor CasesShouldBeSealedRule = DiagnosticDescriptorHelper.Create(
            CasesShouldBeSealedDiagnosticId,
            nameof(Resources.CasesShouldBeSealedTitle),
            nameof(Resources.CasesShouldBeSealedMessageFormat),
            Category,
            DiagnosticSeverity.Error,
            true,
            nameof(Resources.CasesShouldBeSealedDescription));

        private static readonly DiagnosticDescriptor SwitchShouldThrowInDefaultCaseRule = DiagnosticDescriptorHelper.Create(
            SwitchShouldThrowInDefaultCaseDiagnosticId,
            nameof(Resources.SwitchShouldThrowInDefaultCaseTitle),
            nameof(Resources.SwitchShouldThrowInDefaultCaseMessageFormat),
            Category,
            DiagnosticSeverity.Error,
            true,
            nameof(Resources.SwitchShouldThrowInDefaultCaseDescription));

        /// <summary>
        /// Gets a set of descriptors for the diagnostics that this analyzer is capable of producing.
        /// </summary>
        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => ImmutableArray.Create(AllCasesNotHandledRule, SwitchShouldThrowInDefaultCaseRule, DiscriminatedUnionCanOnlyHavePrivateProtectedConstructorsRule, CasesShouldBeSealedRule);

        /// <summary>
        /// Called once at session start to register actions in the analysis context.
        /// </summary>
        /// <param name="context">The context.</param>
        public override void Initialize(AnalysisContext context)
        {
            context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
            context.EnableConcurrentExecution();

            var discriminatedUnionAnalyzer = new DiscriminatedUnionAnalyzer();
            context.RegisterOperationAction(discriminatedUnionAnalyzer.AnalyzeSwitchStatement, OperationKind.Switch);
            context.RegisterOperationAction(discriminatedUnionAnalyzer.AnalyzeSwitchExpression, OperationKind.SwitchExpression);
            context.RegisterSymbolAction(discriminatedUnionAnalyzer.AnalyzeDiscriminatedUnionsTypes, SymbolKind.NamedType);
        }

        private class DiscriminatedUnionAnalyzer
        {
            private const string Null = "null";
            private const string Is = "is";
            private const string Are = "are";

            [SuppressMessage("MicrosoftCodeAnalysisCorrectness", "RS1024:Compare symbols correctly", Justification = "False positive... using SymbolEqualityComparer")]
            private readonly ConcurrentDictionary<ISymbol, ImmutableHashSet<ITypeSymbol>> discriminatedUnions = new (SymbolEqualityComparer.Default);

            public void AnalyzeDiscriminatedUnionsTypes(SymbolAnalysisContext context)
            {
                if (context.Symbol is not INamedTypeSymbol namedTypeSymbol)
                {
                    return;
                }

                if (IsDiscriminatedUnion(namedTypeSymbol))
                {
                    var invalidConstructors = namedTypeSymbol.Constructors.Where(x => x.DeclaredAccessibility != Accessibility.ProtectedAndInternal).ToList();
                    foreach (var invalidConstructor in invalidConstructors)
                    {
                        foreach (var declaringSyntaxReference in invalidConstructor.DeclaringSyntaxReferences)
                        {
                            context.ReportDiagnostic(Diagnostic.Create(
                                DiscriminatedUnionCanOnlyHavePrivateProtectedConstructorsRule,
                                declaringSyntaxReference.GetSyntax().GetLocation(),
                                namedTypeSymbol));
                        }
                    }

                    this.discriminatedUnions.GetOrAdd(namedTypeSymbol, symbol => ImmutableHashSet.Create<ITypeSymbol>(SymbolEqualityComparer.Default));
                    return;
                }

                this.EnsureCaseExists(namedTypeSymbol, context.ReportDiagnostic);
            }

            public void AnalyzeSwitchExpression(OperationAnalysisContext context)
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

                if (!(context.Operation is ISwitchExpressionOperation switchExpressionOperation) || switchExpressionOperation.SemanticModel == null)
                {
                    return;
                }

                var unionTypeInfo = switchExpressionOperation.Value.Type;
                var unionType = unionTypeInfo as INamedTypeSymbol;
                if (!IsDiscriminatedUnionSwitch(unionType))
                {
                    return;
                }

                this.discriminatedUnions.GetOrAdd(unionType, symbol => ImmutableHashSet.Create<ITypeSymbol>(SymbolEqualityComparer.Default));

                var unionTypeWithoutNull = unionType.WithNullableAnnotation(NullableAnnotation.NotAnnotated);
                var actualCaseTypes = switchExpressionOperation.Arms.Select(switchExpressionArmOperation =>
                {
                    if (switchExpressionArmOperation.Pattern is IDeclarationPatternOperation declarationPatternSyntax)
                    {
                        return this.EnsureCaseExists(declarationPatternSyntax.MatchedType, context.ReportDiagnostic);
                    }

                    if (switchExpressionArmOperation.Pattern is ITypePatternOperation typePatternOperation)
                    {
                        return this.EnsureCaseExists(typePatternOperation.MatchedType, context.ReportDiagnostic);
                    }

                    /* if (switchExpressionArmSyntax.Pattern is DiscardPatternSyntax discardPatternSyntax)
                    {
                        context.ReportDiagnostic(Diagnostic.Create(SwitchShouldNotHaveDefaultCaseRule, discardPatternSyntax.GetLocation(), unionType));
                    }*/

                    if (switchExpressionArmOperation.Pattern is IDiscardPatternOperation discardPatternOperation)
                    {
                        if (!(switchExpressionArmOperation.Value is IConversionOperation { Operand: IThrowOperation { Exception: IConversionOperation exceptionConversionOperation } } &&
                              exceptionConversionOperation.Operand.Type!.Name.EndsWith(nameof(UnreachableCaseException)) &&
                              exceptionConversionOperation.Operand is IObjectCreationOperation objectCreationOperation &&
                              objectCreationOperation.Arguments.SingleOrDefault(
                            x =>
                                x.Value is ITypeOfOperation typeOfOperation &&
                                SymbolEqualityComparer.Default.Equals(typeOfOperation.TypeOperand, unionTypeWithoutNull)) != null))
                        {
                            context.ReportDiagnostic(Diagnostic.Create(SwitchShouldThrowInDefaultCaseRule, switchExpressionArmOperation.Syntax.GetLocation(), unionTypeWithoutNull));
                        }
                    }

                    return default;
                }).Where(x => x != null).Select(x => x!).ToList();
                if (!this.discriminatedUnions.TryGetValue(unionTypeWithoutNull, out var caseTypes))
                {
                    return;
                }

                if (!switchExpressionOperation.Arms.Any(x => x.Pattern is IDiscardPatternOperation))
                {
                    context.ReportDiagnostic(Diagnostic.Create(SwitchShouldThrowInDefaultCaseRule, switchExpressionOperation.Syntax.GetLocation(), unionType));
                }

                var isNullCaseMissing = IsNullCaseMissing(switchExpressionOperation.SemanticModel.GetTypeInfo(switchExpressionOperation.Value.Syntax), switchExpressionOperation);
#pragma warning disable RS1024 // Compare symbols correctly
                ReportDiagnostics(new HashSet<ITypeSymbol>(caseTypes, SymbolEqualityComparer.Default), actualCaseTypes, isNullCaseMissing, unionTypeWithoutNull, context);
#pragma warning restore RS1024 // Compare symbols correctly
            }

            public void AnalyzeSwitchStatement(OperationAnalysisContext context)
            {
                bool IsNullCaseMissing(TypeInfo unionTypeInfo, ISwitchOperation switchOperation)
                {
                    if (unionTypeInfo.Nullability.FlowState != NullableFlowState.NotNull)
                    {
                        if (!switchOperation.Cases.Any(x => x.Clauses.Any(
                            x => x is IPatternCaseClauseOperation patternCaseClauseOperation &&
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

                if (!(context.Operation is ISwitchOperation switchOperation) || switchOperation.SemanticModel == null)
                {
                    return;
                }

                var unionTypeInfo = switchOperation.Value.Type;
                var unionType = unionTypeInfo as INamedTypeSymbol;

                if (!IsDiscriminatedUnionSwitch(unionType))
                {
                    return;
                }

                this.discriminatedUnions.GetOrAdd(unionType, symbol => ImmutableHashSet.Create<ITypeSymbol>(SymbolEqualityComparer.Default));

                var unionTypeWithoutNull = unionType.WithNullableAnnotation(NullableAnnotation.NotAnnotated);
                var actualCaseTypes = switchOperation.Cases.SelectMany(switchCaseOperation => switchCaseOperation.Clauses.Select(caseClauseOperation =>
                {
                    if (caseClauseOperation is IPatternCaseClauseOperation patternCaseClauseOperation)
                    {
                        if (patternCaseClauseOperation.Pattern is IDeclarationPatternOperation declarationPatternOperation)
                        {
                            return this.EnsureCaseExists(declarationPatternOperation.MatchedType, context.ReportDiagnostic);
                        }

                        if (patternCaseClauseOperation.Pattern is ITypePatternOperation typePatternOperation)
                        {
                            return this.EnsureCaseExists(typePatternOperation.MatchedType, context.ReportDiagnostic);
                        }
                    }

                    /*
                    if (switchLabelSyntax is DefaultSwitchLabelSyntax defaultSwitchLabelSyntax)
                    {
                        context.ReportDiagnostic(Diagnostic.Create(SwitchShouldNotHaveDefaultCaseRule, defaultSwitchLabelSyntax.GetLocation(), unionType));
                    }*/

                    if (caseClauseOperation is IDefaultCaseClauseOperation defaultCaseClauseOperation)
                    {
                        if (!(switchCaseOperation.Body.SingleOrDefault(x => x is IThrowOperation { Exception: IConversionOperation exceptionConversionOperation } &&
                                                                            exceptionConversionOperation.Operand.Type!.Name.EndsWith(nameof(UnreachableCaseException)) &&
                                                                            exceptionConversionOperation.Operand is IObjectCreationOperation objectCreationOperation &&
                                                                            objectCreationOperation.Arguments.SingleOrDefault(
                            x =>
                                x.Value is ITypeOfOperation typeOfOperation &&
                                SymbolEqualityComparer.Default.Equals(typeOfOperation.TypeOperand, unionTypeWithoutNull)) != null) != null))
                        {
                            context.ReportDiagnostic(Diagnostic.Create(SwitchShouldThrowInDefaultCaseRule, switchCaseOperation.Syntax.GetLocation(), unionTypeWithoutNull));
                        }
                    }

                    return null;
                })).Where(x => x != null).Select(x => x!).ToList();
                if (!this.discriminatedUnions.TryGetValue(unionTypeWithoutNull, out var caseTypes))
                {
                    return;
                }

                var isNullCaseMissing = IsNullCaseMissing(switchOperation.SemanticModel.GetTypeInfo(switchOperation.Value.Syntax), switchOperation);
#pragma warning disable RS1024 // Compare symbols correctly
                ReportDiagnostics(new HashSet<ITypeSymbol>(caseTypes, SymbolEqualityComparer.Default), actualCaseTypes, isNullCaseMissing, unionTypeWithoutNull, context);
#pragma warning restore RS1024 // Compare symbols correctly
            }

            private static void ReportDiagnostics(
                ISet<ITypeSymbol> caseTypes,
                List<ITypeSymbol> actualCaseTypes,
                bool isNullCaseMissing,
                ITypeSymbol unionType,
                OperationAnalysisContext context)
            {
                foreach (var actualCaseType in actualCaseTypes)
                {
                    if (!caseTypes.Remove(actualCaseType))
                    {
                        // Not Possible Case
                    }
                }

                var hasMultiple = caseTypes.Count > 1;
                if (caseTypes.Any() || isNullCaseMissing)
                {
                    context.ReportDiagnostic(
                        Diagnostic.Create(
                            AllCasesNotHandledRule,
                            context.Operation.Syntax.GetLocation(),
                            caseTypes.OrderBy(x => x.Name).Aggregate(
                                new StringBuilder(),
                                (stringBuilder, typeSymbol) =>
                                    stringBuilder.Append(typeSymbol.Name).Append(',')
                                        .Append(' '),
                                builder => isNullCaseMissing
                                    ? builder.Append(Null).ToString()
                                    : builder.ToString(0, builder.Length - 2)),
                            hasMultiple ? 's' : string.Empty,
                            unionType,
                            hasMultiple ? Are : Is));
                }
            }

            private static bool IsDiscriminatedUnionSwitch([NotNullWhen(true)] INamedTypeSymbol? unionType)
            {
                if (unionType == null || unionType.TypeKind != TypeKind.Class)
                {
                    return false;
                }

                return IsDiscriminatedUnion(unionType);
            }

            private static bool IsDiscriminatedUnion(INamedTypeSymbol unionType)
            {
                return unionType.IsAbstract && unionType.GetAttributes().Any(attribute =>
                {
                    var containingClass = attribute.AttributeClass?.ToDisplayString();
                    return containingClass == typeof(Sundew.DiscriminatedUnions.DiscriminatedUnion).FullName;
                });
            }

            private ITypeSymbol? EnsureCaseExists(ITypeSymbol? caseType, Action<Diagnostic> reportDiagnostic)
            {
                if (caseType?.BaseType != null && IsDiscriminatedUnion(caseType.BaseType))
                {
                    this.discriminatedUnions.AddOrUpdate(
                        caseType.BaseType,
                        symbol =>
                        {
                            if (!caseType.IsSealed)
                            {
                                foreach (var declaringSyntaxReference in caseType.DeclaringSyntaxReferences)
                                {
                                    reportDiagnostic(Diagnostic.Create(CasesShouldBeSealedRule, declaringSyntaxReference.GetSyntax().GetLocation(), caseType));
                                }
                            }

                            return ImmutableHashSet.Create<ITypeSymbol>(SymbolEqualityComparer.Default, caseType);
                        },
                        (_, immutableHashSet) => immutableHashSet.Add(caseType));
                }

                return caseType;
            }
        }
    }
}
