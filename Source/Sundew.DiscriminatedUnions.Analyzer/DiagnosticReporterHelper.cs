// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DiagnosticReporterHelper.cs" company="Hukano">
// Copyright (c) Hukano. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Sundew.DiscriminatedUnions.Analyzer;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.CodeAnalysis;

internal static class DiagnosticReporterHelper
{
    private const string Null = "null";

    public static void ReportDiagnostics(
        IReadOnlyList<ITypeSymbol> caseTypes,
        IEnumerable<ITypeSymbol> handledCaseTypes,
        IOperation? nullCase,
        SwitchNullability switchNullability,
        ITypeSymbol unionType,
        IOperation operation,
        Action<Diagnostic> reportDiagnostic)
    {
        var missingCaseTypes = caseTypes.Except(handledCaseTypes, SymbolEqualityComparer.Default)
            .Where(x => x != null).Select(x => x!);
        if (nullCase != null && switchNullability == SwitchNullability.HasUnreachableNullCase)
        {
            reportDiagnostic(Diagnostic.Create(
                SundewDiscriminatedUnionsAnalyzer.SwitchHasUnreachableNullCaseRule,
                nullCase.Syntax.GetLocation(),
                DiagnosticSeverity.Error));
        }

        var isNullCaseMissing = switchNullability == SwitchNullability.IsMissingNullCase;
        var missingNames = missingCaseTypes.Select(x => x.Name)
            .Concat(isNullCaseMissing ? new[] { Null } : Array.Empty<string>()).ToList();
        var hasMultiple = missingNames.Count > 1;
        if (missingNames.Any())
        {
            reportDiagnostic(
                Diagnostic.Create(
                    SundewDiscriminatedUnionsAnalyzer.SwitchAllCasesNotHandledRule,
                    operation.Syntax.GetLocation(),
                    missingNames.Aggregate(
                        new StringBuilder(),
                        (stringBuilder, cases) => stringBuilder.Append('\'').Append(cases).Append('\'').Append(',')
                            .Append(' '),
                        builder => builder.ToString(0, builder.Length - 2)),
                    hasMultiple ? Resources.Cases : Resources.Case,
                    unionType,
                    hasMultiple ? Resources.Are : Resources.Is));
        }
    }
}