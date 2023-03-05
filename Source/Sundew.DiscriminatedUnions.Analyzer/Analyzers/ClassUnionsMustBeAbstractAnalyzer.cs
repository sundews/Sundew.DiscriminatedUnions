// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ClassUnionsMustBeAbstractAnalyzer.cs" company="Hukano">
// Copyright (c) Hukano. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Sundew.DiscriminatedUnions.Analyzer.Analyzers;

using System;
using Microsoft.CodeAnalysis;
using Sundew.DiscriminatedUnions.Shared;

internal class ClassUnionsMustBeAbstractAnalyzer : IUnionSymbolAnalyzer
{
    public void AnalyzeSymbol(INamedTypeSymbol namedTypeSymbol, Action<Diagnostic> reportDiagnostic)
    {
        if (!namedTypeSymbol.IsDiscriminatedUnion())
        {
            return;
        }

        if (namedTypeSymbol.TypeKind == TypeKind.Class && !namedTypeSymbol.IsAbstract)
        {
            foreach (var declaringSyntaxReference in namedTypeSymbol.DeclaringSyntaxReferences)
            {
                reportDiagnostic(
                    Diagnostic.Create(
                        DiscriminatedUnionsAnalyzer.ClassUnionsMustBeAbstractRule,
                        declaringSyntaxReference.GetSyntax().GetLocation(),
                        namedTypeSymbol));
            }
        }
    }
}