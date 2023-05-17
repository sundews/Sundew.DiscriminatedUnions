// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IUnionSymbolAnalyzer.cs" company="Sundews">
// Copyright (c) Sundews. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Sundew.DiscriminatedUnions.Analyzer.Analyzers;

using System;
using Microsoft.CodeAnalysis;

internal interface IUnionSymbolAnalyzer
{
    void AnalyzeSymbol(INamedTypeSymbol namedTypeSymbol, Action<Diagnostic> reportDiagnostic);
}