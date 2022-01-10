// --------------------------------------------------------------------------------------------------------------------
// <copyright file="OnlyUnionsCanExtendOtherUnionsAnalyzer.cs" company="Hukano">
// Copyright (c) Hukano. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Sundew.DiscriminatedUnions.Analyzer.Analyzers;

using System;
using Microsoft.CodeAnalysis;

internal class OnlyUnionsCanExtendOtherUnionsAnalyzer : IUnionSymbolAnalyzer
{
    public void AnalyzeSymbol(INamedTypeSymbol namedTypeSymbol, Action<Diagnostic> reportDiagnostic)
    {
        if (UnionHelper.IsDiscriminatedUnion(namedTypeSymbol))
        {
            return;
        }

        var baseType = namedTypeSymbol.BaseType;
        if (baseType != null &&
            namedTypeSymbol.TypeKind == TypeKind.Class &&
            namedTypeSymbol.IsAbstract &&
            UnionHelper.IsDiscriminatedUnion(baseType))
        {
            foreach (var declaringSyntaxReference in namedTypeSymbol.DeclaringSyntaxReferences)
            {
                reportDiagnostic(Diagnostic.Create(
                    DiscriminatedUnionsAnalyzer.OnlyUnionsCanExtendOtherUnionsRule,
                    declaringSyntaxReference.GetSyntax().GetLocation(),
                    namedTypeSymbol,
                    baseType));
            }
        }

        if (!namedTypeSymbol.IsSealed)
        {
            foreach (var @interface in namedTypeSymbol.Interfaces)
            {
                if (UnionHelper.IsDiscriminatedUnion(@interface))
                {
                    foreach (var declaringSyntaxReference in namedTypeSymbol.DeclaringSyntaxReferences)
                    {
                        reportDiagnostic(Diagnostic.Create(
                            DiscriminatedUnionsAnalyzer.OnlyUnionsCanExtendOtherUnionsRule,
                            declaringSyntaxReference.GetSyntax().GetLocation(),
                            namedTypeSymbol,
                            @interface));
                    }
                }
            }
        }
    }
}