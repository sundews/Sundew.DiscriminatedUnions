﻿// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DiagnosticReporterHelper.cs" company="Sundews">
// Copyright (c) Sundews. All rights reserved.
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
        IReadOnlyList<ISymbol> caseTypes,
        IEnumerable<ISymbol> handledCaseTypes,
        IOperation? nullCase,
        SwitchNullability switchNullability,
        ITypeSymbol unionType,
        IOperation operation,
        Action<Diagnostic> reportDiagnostic)
    {
        var missingCaseTypes = new HashSet<ISymbol>(caseTypes, SymbolEqualityComparer.Default);
        foreach (var handledCaseType in handledCaseTypes)
        {
            missingCaseTypes.Remove(handledCaseType);
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