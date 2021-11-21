// --------------------------------------------------------------------------------------------------------------------
// <copyright file="UnionBaseAnalyzer.cs" company="Hukano">
// Copyright (c) Hukano. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Sundew.DiscriminatedUnions.Analyzer;

using System;
using System.Linq;
using Microsoft.CodeAnalysis;

internal class UnionBaseAnalyzer
{
    public void AnalyzeSymbol(INamedTypeSymbol namedTypeSymbol, Action<Diagnostic> reportDiagnostic)
    {
        if (!UnionHelper.IsDiscriminatedUnion(namedTypeSymbol))
        {
            return;
        }

        if (namedTypeSymbol.TypeKind == TypeKind.Class && !namedTypeSymbol.IsAbstract)
        {
            foreach (var declaringSyntaxReference in namedTypeSymbol.DeclaringSyntaxReferences)
            {
                reportDiagnostic(
                    Diagnostic.Create(
                        DimensionalUnionsAnalyzer.ClassUnionsMustBeAbstractRule,
                        declaringSyntaxReference.GetSyntax().GetLocation(),
                        namedTypeSymbol));
            }
        }

        foreach (var @interface in namedTypeSymbol.AllInterfaces)
        {
            if (UnionHelper.IsDiscriminatedUnion(@interface) && !SymbolEqualityComparer.Default.Equals(namedTypeSymbol.ContainingAssembly, @interface.ContainingAssembly))
            {
                foreach (var declaringSyntaxReference in namedTypeSymbol.DeclaringSyntaxReferences)
                {
                    reportDiagnostic(Diagnostic.Create(
                        DimensionalUnionsAnalyzer.UnionsCannotBeExtendedOutsideItsAssemblyRule,
                        declaringSyntaxReference.GetSyntax().GetLocation(),
                        namedTypeSymbol,
                        @interface));
                }
            }
        }
    }
}