// --------------------------------------------------------------------------------------------------------------------
// <copyright file="UnionsCannotBeExtendedOutsideTheirAssemblyAnalyzer.cs" company="Sundews">
// Copyright (c) Sundews. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Sundew.DiscriminatedUnions.Analyzer.Analyzers;

using System;
using Microsoft.CodeAnalysis;
using Sundew.DiscriminatedUnions.Shared;

internal class UnionsCannotBeExtendedOutsideTheirAssemblyAnalyzer : IUnionSymbolAnalyzer
{
    public void AnalyzeSymbol(INamedTypeSymbol namedTypeSymbol, Action<Diagnostic> reportDiagnostic)
    {
        if (!namedTypeSymbol.IsDiscriminatedUnion())
        {
            return;
        }

        var baseType = namedTypeSymbol.BaseType;
        if (baseType != null &&
            namedTypeSymbol.TypeKind == TypeKind.Class &&
            namedTypeSymbol.IsAbstract &&
            baseType.IsDiscriminatedUnion() &&
            !SymbolEqualityComparer.Default.Equals(namedTypeSymbol.ContainingAssembly, baseType.ContainingAssembly))
        {
            foreach (var declaringSyntaxReference in namedTypeSymbol.DeclaringSyntaxReferences)
            {
                reportDiagnostic(Diagnostic.Create(
                    DiscriminatedUnionsAnalyzer.UnionsCannotBeExtendedOutsideTheirAssemblyRule,
                    declaringSyntaxReference.GetSyntax().GetLocation(),
                    namedTypeSymbol,
                    baseType.ContainingAssembly.Name));
            }
        }

        foreach (var @interface in namedTypeSymbol.Interfaces)
        {
            if (@interface.IsDiscriminatedUnion() && !SymbolEqualityComparer.Default.Equals(namedTypeSymbol.ContainingAssembly, @interface.ContainingAssembly))
            {
                foreach (var declaringSyntaxReference in namedTypeSymbol.DeclaringSyntaxReferences)
                {
                    reportDiagnostic(Diagnostic.Create(
                        DiscriminatedUnionsAnalyzer.UnionsCannotBeExtendedOutsideTheirAssemblyRule,
                        declaringSyntaxReference.GetSyntax().GetLocation(),
                        namedTypeSymbol,
                        @interface.ContainingAssembly.Name));
                }
            }
        }
    }
}