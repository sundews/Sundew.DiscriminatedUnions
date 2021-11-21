// --------------------------------------------------------------------------------------------------------------------
// <copyright file="UnionSwitchExpressionAnalyzer.cs" company="Hukano">
// Copyright (c) Hukano. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Sundew.DiscriminatedUnions.Analyzer.SwitchExpression;

using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis.Operations;

internal class UnionSwitchExpressionAnalyzer
{
    public void Analyze(OperationAnalysisContext operationAnalysisContext)
    {
        if (!(operationAnalysisContext.Operation is ISwitchExpressionOperation switchExpressionOperation
              && switchExpressionOperation.SemanticModel != null))
        {
            return;
        }

        var unionTypeSymbol = switchExpressionOperation.Value.Type;
        var unionType = unionTypeSymbol as INamedTypeSymbol;
        if (!UnionHelper.IsDiscriminatedUnion(unionType))
        {
            return;
        }

        var unionTypeWithoutNull = unionType.WithNullableAnnotation(NullableAnnotation.NotAnnotated);
        var nullCase = SwitchExpressionHelper.GetNullCase(switchExpressionOperation);
        var switchNullability = UnionHelper.EvaluateSwitchNullability(
            switchExpressionOperation.Value,
            switchExpressionOperation.SemanticModel,
            nullCase != null);

        if (switchExpressionOperation.Arms
                .SingleOrDefault(x => x.Pattern is IDiscardPatternOperation) is { } switchExpressionArmOperation)
        {
            operationAnalysisContext.ReportDiagnostic(Diagnostic.Create(
                DimensionalUnionsAnalyzer.SwitchShouldNotHaveDefaultCaseRule,
                switchExpressionArmOperation.Syntax.GetLocation(),
                unionType));
        }

        var caseTypes = UnionHelper.GetAllCaseTypes(unionTypeWithoutNull, operationAnalysisContext.Compilation);
        DiagnosticReporterHelper.ReportDiagnostics(
            caseTypes.ToList(),
            SwitchExpressionHelper.GetHandledCaseTypes(switchExpressionOperation).Where(x => x.HandlesCase).Select(x => x.Type),
            nullCase,
            switchNullability,
            unionTypeWithoutNull,
            switchExpressionOperation,
            operationAnalysisContext.ReportDiagnostic);
    }
}