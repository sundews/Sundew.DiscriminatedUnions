// --------------------------------------------------------------------------------------------------------------------
// <copyright file="UnionSwitchStatementAnalyzer.cs" company="Sundews">
// Copyright (c) Sundews. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Sundew.DiscriminatedUnions.Analyzer.SwitchStatement;

using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis.Operations;
using Sundew.DiscriminatedUnions.Shared;

internal class UnionSwitchStatementAnalyzer
{
    public void Analyze(OperationAnalysisContext operationAnalysisContext)
    {
        if (!(operationAnalysisContext.Operation is ISwitchOperation switchOperation &&
              switchOperation.SemanticModel != null))
        {
            return;
        }

        var unionTypeSymbol = switchOperation.Value.Type;
        var unionType = unionTypeSymbol as INamedTypeSymbol;
        if (!unionType.IsDiscriminatedUnionLike())
        {
            return;
        }

        var nonNullableUnionType = UnionHelper.GetNonNullableUnionType(unionType);
        var nullCase = SwitchStatementHelper.GetNullCase(switchOperation);
        var switchNullability = UnionHelper.EvaluateSwitchNullability(
            switchOperation.Value,
            switchOperation.SemanticModel,
            nullCase.HasValue);
        if (GetDefaultSwitchCaseOperation(switchOperation) is { } defaultSwitchCaseOperation)
        {
            if (CanSwitchReachEnd(switchOperation))
            {
                if (nonNullableUnionType.TypeKind != TypeKind.Enum)
                {
                    operationAnalysisContext.ReportDiagnostic(Diagnostic.Create(
                        DiscriminatedUnionsAnalyzer.SwitchShouldNotHaveDefaultCaseRule,
                        defaultSwitchCaseOperation.Syntax.GetLocation(),
                        nonNullableUnionType));
                }
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
                                SymbolEqualityComparer.Default.Equals(typeOfOperation.TypeOperand, nonNullableUnionType)) != null) != null))
                {
                    operationAnalysisContext.ReportDiagnostic(Diagnostic.Create(
                        DiscriminatedUnionsAnalyzer.SwitchShouldThrowInDefaultCaseRule,
                        defaultSwitchCaseOperation.Syntax.GetLocation(),
                        unionType));
                }
            }
        }

        var caseTypes = UnionHelper.GetKnownCases(nonNullableUnionType);
        DiagnosticReporterHelper.ReportDiagnostics(
            caseTypes.ToList(),
            SwitchStatementHelper.GetHandledCaseTypes(switchOperation).ToList(),
            nullCase,
            switchNullability,
            unionType,
            switchOperation,
            operationAnalysisContext.ReportDiagnostic);
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