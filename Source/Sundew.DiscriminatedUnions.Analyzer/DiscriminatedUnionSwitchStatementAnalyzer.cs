// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DiscriminatedUnionSwitchStatementAnalyzer.cs" company="Hukano">
// Copyright (c) Hukano. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Sundew.DiscriminatedUnions.Analyzer
{
    using System.Collections.Concurrent;
    using System.Linq;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.Diagnostics;
    using Microsoft.CodeAnalysis.Operations;

    internal class DiscriminatedUnionSwitchStatementAnalyzer
    {
        private readonly ConcurrentBag<SwitchInfo> switchInfos = new();

        public void RegisterAndAnalyze(OperationAnalysisContext operationAnalysisContext)
        {
            if (!(operationAnalysisContext.Operation is ISwitchOperation switchOperation
                  && switchOperation.SemanticModel != null))
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

            var nullCase = DiscriminatedUnionHelper.GetNullCase(switchOperation);
            var switchNullabilityError = DiscriminatedUnionHelper.EvaluateSwitchNullability(switchOperation.Value, switchOperation.SemanticModel, nullCase != null);
            this.switchInfos.Add(new SwitchInfo
            {
                SwitchOperation = switchOperation,
                Cases = DiscriminatedUnionHelper.GetHandledCaseTypes(switchOperation),
                NullCase = nullCase,
                SwitchNullabilityError = switchNullabilityError,
                SemanticModel = switchOperation.SemanticModel,
            });

            if (GetDefaultSwitchCaseOperation(switchOperation) is { } defaultSwitchCaseOperation)
            {
                if (CanSwitchReachEnd(switchOperation))
                {
                    operationAnalysisContext.ReportDiagnostic(Diagnostic.Create(
                        SundewDiscriminatedUnionsAnalyzer.SwitchShouldNotHaveDefaultCaseRule,
                        defaultSwitchCaseOperation.Syntax.GetLocation(),
                        unionType));
                }
                else
                {
                    if (!(defaultSwitchCaseOperation.Body.SingleOrDefault(x =>
                        x is IThrowOperation { Exception: IConversionOperation exceptionConversionOperation } &&
                        exceptionConversionOperation.Operand.Type!.Name.EndsWith(
                            nameof(UnreachableCaseException)) &&
                        exceptionConversionOperation.Operand is IObjectCreationOperation
                            objectCreationOperation &&
                        objectCreationOperation.Arguments.SingleOrDefault(
                            x =>
                                x.Value is ITypeOfOperation typeOfOperation &&
                                SymbolEqualityComparer.Default.Equals(typeOfOperation.TypeOperand, unionTypeWithoutNull)) != null) != null))
                    {
                        operationAnalysisContext.ReportDiagnostic(Diagnostic.Create(
                            SundewDiscriminatedUnionsAnalyzer.SwitchShouldThrowInDefaultCaseRule,
                            defaultSwitchCaseOperation.Syntax.GetLocation(),
                            unionTypeWithoutNull));
                    }
                }
            }
        }

        public void AnalyzeCases(
            CompilationAnalysisContext compilationAnalysisContext,
            DiscriminatedUnionRegistry discriminatedUnionRegistry)
        {
            foreach (var switchInfo in this.switchInfos)
            {
            }
        }

        private static bool CanSwitchReachEnd(ISwitchOperation switchOperation)
        {
            return switchOperation.Cases.Where(x => !x.Clauses.OfType<IDefaultCaseClauseOperation>().Any())
                .SelectMany(x => x.Body)
                .Any(x => x.Kind == OperationKind.Branch);
        }

        private static ISwitchCaseOperation? GetDefaultSwitchCaseOperation(ISwitchOperation switchOperation)
        {
            return switchOperation.Cases
                .SelectMany(switchCaseOperation => switchCaseOperation.Clauses
                    .OfType<IDefaultCaseClauseOperation>()
                    .Select(x => switchCaseOperation))
                .FirstOrDefault();
        }
    }
}