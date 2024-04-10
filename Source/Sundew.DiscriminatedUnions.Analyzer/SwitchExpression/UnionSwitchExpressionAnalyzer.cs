// --------------------------------------------------------------------------------------------------------------------
// <copyright file="UnionSwitchExpressionAnalyzer.cs" company="Sundews">
// Copyright (c) Sundews. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Sundew.DiscriminatedUnions.Analyzer.SwitchExpression;

using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis.Operations;
using Sundew.DiscriminatedUnions.Shared;

internal class UnionSwitchExpressionAnalyzer
{
    public void Analyze(OperationAnalysisContext operationAnalysisContext)
    {
        if (!(operationAnalysisContext.Operation is ISwitchExpressionOperation switchExpressionOperation &&
              switchExpressionOperation.SemanticModel != null))
        {
            return;
        }

        var unionTypeSymbol = switchExpressionOperation.Value.Type;
        var unionType = unionTypeSymbol as INamedTypeSymbol;
        if (!unionType.IsDiscriminatedUnionLike())
        {
            return;
        }

        var nonNullableUnionType = UnionHelper.GetNonNullableUnionType(unionType);
        var nullCase = SwitchExpressionHelper.GetNullCase(switchExpressionOperation);
        var switchNullability = UnionHelper.EvaluateSwitchNullability(
            switchExpressionOperation.Value,
            switchExpressionOperation.SemanticModel,
            nullCase != null);

        if (switchExpressionOperation.Arms
                .SingleOrDefault(x => x.Pattern is IDiscardPatternOperation) is { } switchExpressionArmOperation
            && nonNullableUnionType.TypeKind != TypeKind.Enum)
        {
            operationAnalysisContext.ReportDiagnostic(Diagnostic.Create(
                DiscriminatedUnionsAnalyzer.SwitchShouldNotHaveDefaultCaseRule,
                switchExpressionArmOperation.Syntax.GetLocation(),
                unionType));
        }

        var caseTypes = UnionHelper.GetKnownCases(nonNullableUnionType).ToList();
        DiagnosticReporterHelper.ReportDiagnostics(
            caseTypes,
            SwitchExpressionHelper.GetHandledCaseTypes(switchExpressionOperation).ToList(),
            nullCase,
            switchNullability,
            unionType,
            switchExpressionOperation,
            operationAnalysisContext.ReportDiagnostic);
    }
}