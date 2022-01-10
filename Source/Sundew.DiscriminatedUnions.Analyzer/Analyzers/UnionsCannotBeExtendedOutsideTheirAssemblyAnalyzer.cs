// --------------------------------------------------------------------------------------------------------------------
// <copyright file="UnionsCannotBeExtendedOutsideTheirAssemblyAnalyzer.cs" company="Hukano">
// Copyright (c) Hukano. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Sundew.DiscriminatedUnions.Analyzer.Analyzers;

using System;
using Microsoft.CodeAnalysis;

internal class UnionsCannotBeExtendedOutsideTheirAssemblyAnalyzer : IUnionSymbolAnalyzer
{
    public void AnalyzeSymbol(INamedTypeSymbol namedTypeSymbol, Action<Diagnostic> reportDiagnostic)
    {
        if (!UnionHelper.IsDiscriminatedUnion(namedTypeSymbol))
        {
            return;
        }

        var baseType = namedTypeSymbol.BaseType;
        if (baseType != null &&
            namedTypeSymbol.TypeKind == TypeKind.Class &&
            namedTypeSymbol.IsAbstract &&
            UnionHelper.IsDiscriminatedUnion(baseType) &&
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
            if (UnionHelper.IsDiscriminatedUnion(@interface) && !SymbolEqualityComparer.Default.Equals(namedTypeSymbol.ContainingAssembly, @interface.ContainingAssembly))
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