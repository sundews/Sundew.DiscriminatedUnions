// --------------------------------------------------------------------------------------------------------------------
// <copyright file="LanguageUnionExtensions.cs" company="Sundews">
// Copyright (c) Sundews. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Sundew.DiscriminatedUnions.Shared;

using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics.CodeAnalysis;
using System.Threading;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;

/// <summary>
/// Reads the unions of the language: closed types and union types.
/// The compiler knows their cases, where the cases of a class union are found through the <see cref="DiscriminatedUnion"/> attribute.
/// </summary>
public static class LanguageUnionExtensions
{
    /// <summary>
    /// Determines whether the type is a closed type or a union type.
    /// </summary>
    /// <param name="typeSymbol">The type.</param>
    /// <returns><c>true</c>, if the type is a closed type or a union type, otherwise <c>false</c>.</returns>
    public static bool IsLanguageUnion([NotNullWhen(true)] this ITypeSymbol? typeSymbol)
    {
        return typeSymbol is { IsClosed: true } or { IsUnion: true };
    }

    /// <summary>
    /// Gets the cases of a closed type or a union type: the case types of a union, and for a closed type the types that derive from it.
    /// </summary>
    /// <param name="unionType">The closed type or union type.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The case types in the order they are declared.</returns>
    public static ImmutableArray<ITypeSymbol> GetLanguageUnionCases(this INamedTypeSymbol unionType, CancellationToken cancellationToken)
    {
        return unionType.IsUnion ? unionType.UnionCaseTypes : unionType.GetClosedTypeCases(cancellationToken);
    }

    /// <summary>
    /// Gets the cases of the specified closed type: the types that derive from it. Where one of those is a closed type itself,
    /// the types that derive from that one take its place, because they are what a switch has to handle.
    /// </summary>
    /// <param name="closedTypeSymbol">The closed type.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The case types, in the order the compiler lists the derived types.</returns>
    public static ImmutableArray<ITypeSymbol> GetClosedTypeCases(this INamedTypeSymbol closedTypeSymbol, CancellationToken cancellationToken)
    {
        var caseTypes = ImmutableArray.CreateBuilder<ITypeSymbol>();
        AddClosedTypeCases(closedTypeSymbol, caseTypes, new HashSet<ISymbol>(SymbolEqualityComparer.Default), cancellationToken);
        return caseTypes.ToImmutable();
    }

    private static void AddClosedTypeCases(INamedTypeSymbol closedTypeSymbol, ImmutableArray<ITypeSymbol>.Builder caseTypes, HashSet<ISymbol> visitedTypes, CancellationToken cancellationToken)
    {
        if (!visitedTypes.Add(closedTypeSymbol))
        {
            return;
        }

        foreach (var derivedType in closedTypeSymbol.GetClosedDerivedTypeInfo(cancellationToken).ClosedDerivedTypes)
        {
            if (derivedType is INamedTypeSymbol { IsClosed: true } closedDerivedType)
            {
                AddClosedTypeCases(closedDerivedType, caseTypes, visitedTypes, cancellationToken);
            }
            else
            {
                caseTypes.Add(derivedType);
            }
        }
    }
}
