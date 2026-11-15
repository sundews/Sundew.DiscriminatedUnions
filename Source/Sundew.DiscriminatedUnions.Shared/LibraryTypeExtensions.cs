// --------------------------------------------------------------------------------------------------------------------
// <copyright file="LibraryTypeExtensions.cs" company="Sundews">
// Copyright (c) Sundews. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Sundew.DiscriminatedUnions.Shared;

using System.Diagnostics.CodeAnalysis;
using Microsoft.CodeAnalysis;

/// <summary>
/// Recognizes the types of the Sundew.DiscriminatedUnions library.
/// A type is recognized by comparing its name and its namespaces, so no string is created for each type that is looked at.
/// That matters, because the analyzers and the generator ask for every attribute and interface of the types they come across.
/// </summary>
public static class LibraryTypeExtensions
{
    private const string RootNamespaceName = nameof(Sundew);
    private const string LibraryNamespaceName = nameof(Sundew.DiscriminatedUnions);

    /// <summary>
    /// Determines whether the type is the <see cref="DiscriminatedUnion"/> attribute.
    /// </summary>
    /// <param name="typeSymbol">The type.</param>
    /// <returns><c>true</c>, if the type is the attribute, otherwise <c>false</c>.</returns>
    public static bool IsDiscriminatedUnionAttribute([NotNullWhen(true)] this INamedTypeSymbol? typeSymbol)
    {
        return IsLibraryType(typeSymbol, nameof(Sundew.DiscriminatedUnions.DiscriminatedUnion), 0);
    }

    /// <summary>
    /// Determines whether the type is the <see cref="CaseTypeAttribute"/>.
    /// </summary>
    /// <param name="typeSymbol">The type.</param>
    /// <returns><c>true</c>, if the type is the attribute, otherwise <c>false</c>.</returns>
    public static bool IsCaseTypeAttribute([NotNullWhen(true)] this INamedTypeSymbol? typeSymbol)
    {
        return IsLibraryType(typeSymbol, nameof(Sundew.DiscriminatedUnions.CaseTypeAttribute), 0);
    }

    /// <summary>
    /// Determines whether the type is a construction of the <see cref="ICase{TCase}"/> interface.
    /// </summary>
    /// <param name="typeSymbol">The type.</param>
    /// <returns><c>true</c>, if the type is the interface, otherwise <c>false</c>.</returns>
    public static bool IsCaseInterface([NotNullWhen(true)] this INamedTypeSymbol? typeSymbol)
    {
        return IsLibraryType(typeSymbol, nameof(Sundew.DiscriminatedUnions.ICase<>), 1);
    }

    /// <summary>
    /// Determines whether the type is a construction of the <see cref="IDefaultCase{TCase}"/> interface.
    /// </summary>
    /// <param name="typeSymbol">The type.</param>
    /// <returns><c>true</c>, if the type is the interface, otherwise <c>false</c>.</returns>
    public static bool IsDefaultCaseInterface([NotNullWhen(true)] this INamedTypeSymbol? typeSymbol)
    {
        return IsLibraryType(typeSymbol, nameof(Sundew.DiscriminatedUnions.IDefaultCase<>), 1);
    }

    private static bool IsLibraryType([NotNullWhen(true)] INamedTypeSymbol? typeSymbol, string name, int arity)
    {
        // The name is compared first, because it tells nearly all types apart. The metadata name is not used: for a generic type it is put together on every call.
        return typeSymbol is { ContainingType: null }
               && typeSymbol.Arity == arity
               && typeSymbol.Name == name
               && typeSymbol.ContainingNamespace is { Name: LibraryNamespaceName, ContainingNamespace: { Name: RootNamespaceName, ContainingNamespace.IsGlobalNamespace: true } };
    }
}
