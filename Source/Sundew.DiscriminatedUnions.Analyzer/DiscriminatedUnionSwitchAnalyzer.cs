// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DiscriminatedUnionSwitchAnalyzer.cs" company="Hukano">
// Copyright (c) Hukano. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Sundew.DiscriminatedUnions.Analyzer
{
    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.Linq;
    using System.Text;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.Diagnostics;
    using Microsoft.CodeAnalysis.Operations;

    internal class DiscriminatedUnionSwitchAnalyzer
    {
        private const string Null = "null";

        [SuppressMessage("MicrosoftCodeAnalysisCorrectness", "RS1024:Compare symbols correctly", Justification = "False positive... using SymbolEqualityComparer")]
        private readonly ConcurrentDictionary<ISymbol, List<ITypeSymbol>> discriminatedUnions = new(SymbolEqualityComparer.Default);

        private enum SwitchNullability
        {
            None,
            IsMissingNullCase,
            HasUnreachableNullCase,
        }

        public void AnalyzeSwitchExpression(OperationAnalysisContext context)
        {
            if (!(context.Operation is ISwitchExpressionOperation switchExpressionOperation && switchExpressionOperation.SemanticModel != null))
            {
                return;
            }

            var unionTypeSymbol = switchExpressionOperation.Value.Type;
            var unionType = unionTypeSymbol as INamedTypeSymbol;
            if (!DiscriminatedUnionHelper.IsDiscriminatedUnion(unionType))
            {
                return;
            }

            var unionTypeWithoutNull = unionType.WithNullableAnnotation(NullableAnnotation.NotAnnotated);
            var actualCaseTypes = DiscriminatedUnionHelper.GetHandledCaseTypes(switchExpressionOperation);

            if (switchExpressionOperation.Arms
                .SingleOrDefault(x => x.Pattern is IDiscardPatternOperation) is { } switchExpressionArmOperation)
            {
                context.ReportDiagnostic(Diagnostic.Create(
                    SundewDiscriminatedUnionsAnalyzer.SwitchShouldNotHaveDefaultCaseRule,
                    switchExpressionArmOperation.Syntax.GetLocation(),
                    unionType));
            }

            var switchNullabilityError = EvaluateSwitchNullability(
                switchExpressionOperation.Value,
                switchExpressionOperation.SemanticModel,
                DiscriminatedUnionHelper.HasNullCase(switchExpressionOperation));

            var caseTypes = this.GetCaseTypes(unionTypeWithoutNull);

            ReportDiagnostics(
                caseTypes,
                actualCaseTypes.Where(x => x.HandlesCase).Select(x => x.Type),
                switchNullabilityError,
                unionTypeWithoutNull,
                switchExpressionOperation,
                context);
        }

        public void AnalyzeSwitchStatement(OperationAnalysisContext context)
        {
            if (!(context.Operation is ISwitchOperation switchOperation && switchOperation.SemanticModel != null))
            {
                return;
            }

            var unionTypeSymbol = switchOperation.Value.Type;
            var unionType = unionTypeSymbol as INamedTypeSymbol;

            if (!DiscriminatedUnionHelper.IsDiscriminatedUnion(unionType))
            {
                return;
            }

            var unionTypeWithoutNull = unionType.WithNullableAnnotation(NullableAnnotation.NotAnnotated);
            var handledCaseTypes = DiscriminatedUnionHelper.GetHandledCaseTypes(switchOperation);

            if (switchOperation.Cases
                .SelectMany(switchCaseOperation => switchCaseOperation.Clauses
                    .OfType<IDefaultCaseClauseOperation>()
                    .Select(x => switchCaseOperation))
                .SingleOrDefault() is { } switchCaseOperation)
            {
                if (!(switchCaseOperation.Body.SingleOrDefault(x =>
                    x is IThrowOperation { Exception: IConversionOperation exceptionConversionOperation } &&
                    exceptionConversionOperation.Operand.Type!.Name.EndsWith(
                        nameof(UnreachableCaseException)) &&
                    exceptionConversionOperation.Operand is IObjectCreationOperation
                        objectCreationOperation &&
                    objectCreationOperation.Arguments.SingleOrDefault(
                        x =>
                            x.Value is ITypeOfOperation typeOfOperation && SymbolEqualityComparer.Default.Equals(typeOfOperation.TypeOperand, unionTypeWithoutNull)) != null) != null))
                {
                    context.ReportDiagnostic(Diagnostic.Create(
                        SundewDiscriminatedUnionsAnalyzer.SwitchShouldThrowInDefaultCaseRule,
                        switchCaseOperation.Syntax.GetLocation(),
                        unionTypeWithoutNull));
                }
            }

            var switchNullabilityError = EvaluateSwitchNullability(switchOperation.Value, switchOperation.SemanticModel, DiscriminatedUnionHelper.HasNullCase(switchOperation));

            var caseTypes = this.GetCaseTypes(unionTypeWithoutNull);

            ReportDiagnostics(
                caseTypes,
                handledCaseTypes.Where(x => x.HandlesCase).Select(x => x.Type),
                switchNullabilityError,
                unionTypeWithoutNull,
                switchOperation,
                context);
        }

        private static void ReportDiagnostics(
            IReadOnlyList<ITypeSymbol> caseTypes,
            IEnumerable<ITypeSymbol> handledCaseTypes,
            SwitchNullability switchNullability,
            ITypeSymbol unionType,
            IOperation operation,
            OperationAnalysisContext context)
        {
            var missingCaseTypes = caseTypes.Except(handledCaseTypes, SymbolEqualityComparer.Default).Where(x => x != null).Select(x => x!).ToList();
            if (switchNullability == SwitchNullability.HasUnreachableNullCase)
            {
                context.ReportDiagnostic(Diagnostic.Create(
                    SundewDiscriminatedUnionsAnalyzer.HasUnreachableNullCaseRule,
                    operation.Syntax.GetLocation(),
                    DiagnosticSeverity.Error));
            }

            var isNullCaseMissing = switchNullability == SwitchNullability.IsMissingNullCase;
            var missingNames = missingCaseTypes.Select(x => x.Name).Concat(isNullCaseMissing ? new[] { Null } : Array.Empty<string>()).ToList();
            var hasMultiple = missingNames.Count > 1;
            if (missingNames.Any())
            {
                context.ReportDiagnostic(
                    Diagnostic.Create(
                        SundewDiscriminatedUnionsAnalyzer.AllCasesNotHandledRule,
                        operation.Syntax.GetLocation(),
                        missingNames.Aggregate(
                            new StringBuilder(),
                            (stringBuilder, cases) => stringBuilder.Append('\'').Append(cases).Append('\'').Append(',').Append(' '),
                            builder => builder.ToString(0, builder.Length - 2)),
                        hasMultiple ? Resources.Cases : Resources.Case,
                        unionType,
                        hasMultiple ? Resources.Are : Resources.Is));
            }
        }

        private static bool IsNullableEnabled(SemanticModel semanticModel, IOperation operation)
        {
            return (semanticModel.GetNullableContext(operation.Syntax.GetLocation()
                       .SourceSpan.Start) & NullableContext.Enabled) == NullableContext.Enabled ||
                   semanticModel.Compilation.Options.NullableContextOptions != NullableContextOptions.Disable;
        }

        private static SwitchNullability EvaluateSwitchNullability(IOperation operation, SemanticModel semanticModel, bool hasNullCase)
        {
            var unionTypeInfo = semanticModel.GetTypeInfo(operation.Syntax);
            var isNullableEnabled = IsNullableEnabled(semanticModel, operation);
            var isNullable = unionTypeInfo.ConvertedNullability.Annotation != NullableAnnotation.NotAnnotated;
            var maybeNull = unionTypeInfo.ConvertedNullability.FlowState != NullableFlowState.NotNull;
            var nullableStates = (isNullableEnabled, isNullable, maybeNull, hasNullCase);
            var switchNullabilityError = nullableStates switch
            {
                (true, false, _, true) => SwitchNullability.HasUnreachableNullCase,
                (_, _, true, false) => SwitchNullability.IsMissingNullCase,
                (_, _, false, true) => SwitchNullability.HasUnreachableNullCase,
                (_, _, true, true) => SwitchNullability.None,
                (_, _, false, false) => SwitchNullability.None,
            };
            return switchNullabilityError;
        }

        private IReadOnlyList<ITypeSymbol> GetCaseTypes(ITypeSymbol unionType)
        {
            return this.discriminatedUnions.GetOrAdd(unionType, x =>
            {
                var caseTypes = new List<ITypeSymbol>();
                foreach (var caseType in DiscriminatedUnionHelper.GetAllCaseTypes(unionType))
                {
                    caseTypes.Add(caseType);
                }

                return caseTypes;
            });
        }
    }
}