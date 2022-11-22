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
        IReadOnlyList<INamedTypeSymbol> caseTypes,
        IEnumerable<INamedTypeSymbol> handledCaseTypes,
        IOperation? nullCase,
        SwitchNullability switchNullability,
        ITypeSymbol unionType,
        IOperation operation,
        Action<Diagnostic> reportDiagnostic)
    {
        var missingCaseTypes = new HashSet<INamedTypeSymbol>(caseTypes, SymbolEqualityComparer.Default);
        foreach (var handledCaseType in handledCaseTypes)
        {
            missingCaseTypes.RemoveWhere(x =>
            {
                var actualHandledCaseType = handledCaseType;
                if (x.IsGenericType || handledCaseType.IsGenericType)
                {
                    x = x.OriginalDefinition;
                    actualHandledCaseType = actualHandledCaseType.OriginalDefinition;
                }

                return SymbolEqualityComparer.Default.Equals(actualHandledCaseType, x);
            });
        }

        if (nullCase != null && switchNullability == SwitchNullability.HasUnreachableNullCase)
        {
            reportDiagnostic(Diagnostic.Create(
                DiscriminatedUnionsAnalyzer.SwitchHasUnreachableNullCaseRule,
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
                    DiscriminatedUnionsAnalyzer.SwitchAllCasesNotHandledRule,
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