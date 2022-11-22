// --------------------------------------------------------------------------------------------------------------------
// <copyright file="UnionSwitchStatementAnalyzer.cs" company="Hukano">
// Copyright (c) Hukano. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Sundew.DiscriminatedUnions.Analyzer.SwitchStatement;

using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis.Operations;

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
        if (!UnionHelper.IsDiscriminatedUnion(unionType))
        {
            return;
        }

        var unionTypeWithoutNull = unionType.WithNullableAnnotation(NullableAnnotation.NotAnnotated);
        var nullCase = SwitchStatementHelper.GetNullCase(switchOperation);
        var switchNullability = UnionHelper.EvaluateSwitchNullability(
            switchOperation.Value,
            switchOperation.SemanticModel,
            nullCase != null);
        if (GetDefaultSwitchCaseOperation(switchOperation) is { } defaultSwitchCaseOperation)
        {
            if (CanSwitchReachEnd(switchOperation))
            {
                operationAnalysisContext.ReportDiagnostic(Diagnostic.Create(
                    DiscriminatedUnionsAnalyzer.SwitchShouldNotHaveDefaultCaseRule,
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
                        DiscriminatedUnionsAnalyzer.SwitchShouldThrowInDefaultCaseRule,
                        defaultSwitchCaseOperation.Syntax.GetLocation(),
                        unionTypeWithoutNull));
                }
            }
        }

        var caseTypes = UnionHelper.GetKnownCaseTypes(unionTypeWithoutNull);
        DiagnosticReporterHelper.ReportDiagnostics(
            caseTypes.ToList(),
            SwitchStatementHelper.GetHandledCaseTypes(switchOperation)
                .Where(x => x.HandlesCase)
                .Select(x => x.Type),
            nullCase,
            switchNullability,
            unionTypeWithoutNull,
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