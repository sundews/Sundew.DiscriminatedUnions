// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DiscriminatedUnionSwitchExpressionAnalyzer.cs" company="Hukano">
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

    internal class DiscriminatedUnionSwitchExpressionAnalyzer
    {
        private readonly ConcurrentBag<SwitchExpressionInfo> switchExpressionInfos = new();

        public void RegisterAndAnalyze(OperationAnalysisContext operationAnalysisContext)
        {
            if (!(operationAnalysisContext.Operation is ISwitchExpressionOperation switchExpressionOperation
                  && switchExpressionOperation.SemanticModel != null))
            {
                return;
            }

            var unionTypeSymbol = switchExpressionOperation.Value.Type;
            var unionType = unionTypeSymbol as INamedTypeSymbol;
            if (!DiscriminatedUnionHelper.IsDiscriminatedUnion(unionType))
            {
                return;
            }

            if (switchExpressionOperation.Arms
                .SingleOrDefault(x => x.Pattern is IDiscardPatternOperation) is { } switchExpressionArmOperation)
            {
                operationAnalysisContext.ReportDiagnostic(Diagnostic.Create(
                    SundewDiscriminatedUnionsAnalyzer.SwitchShouldNotHaveDefaultCaseRule,
                    switchExpressionArmOperation.Syntax.GetLocation(),
                    unionType));
            }

            var nullCase = DiscriminatedUnionHelper.GetNullCase(switchExpressionOperation);
            var switchNullability = DiscriminatedUnionHelper.EvaluateSwitchNullability(
                switchExpressionOperation.Value,
                switchExpressionOperation.SemanticModel,
                nullCase != null);

            this.switchExpressionInfos.Add(new SwitchExpressionInfo
            {
                UnionType = unionType,
                SwitchExpressionOperation = switchExpressionOperation,
                Cases = DiscriminatedUnionHelper.GetHandledCaseTypes(switchExpressionOperation),
                NullCase = DiscriminatedUnionHelper.GetNullCase(switchExpressionOperation),
                SwitchNullability = switchNullability,
                SemanticModel = switchExpressionOperation.SemanticModel,
            });
        }

        public void AnalyzeCases(
            CompilationAnalysisContext compilationAnalysisContext,
            DiscriminatedUnionRegistry discriminatedUnionRegistry)
        {
            foreach (var switchExpressionInfo in this.switchExpressionInfos)
            {
                var cases = discriminatedUnionRegistry.GetCases(switchExpressionInfo.UnionType);
            }
        }
    }
}