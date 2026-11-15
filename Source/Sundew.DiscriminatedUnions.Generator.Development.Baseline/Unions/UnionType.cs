// --------------------------------------------------------------------------------------------------------------------
// <copyright file="UnionType.cs" company="Sundews">
// Copyright (c) Sundews. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Sundew.DiscriminatedUnions.Generator.Unions;

using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using Microsoft.CodeAnalysis;
using Sundew.Base.Collections.Immutable;
using Sundew.DiscriminatedUnions.Generator.Model;

/// <summary>
/// Identifies a union type and where it is declared, which is what it takes to declare another part of it.
/// </summary>
/// <param name="Namespace">The namespace, which is empty for the global namespace.</param>
/// <param name="ContainingTypes">The types the union is nested in, outermost first.</param>
/// <param name="Name">The name of the union.</param>
/// <param name="TypeParameters">The names of the type parameters of the union.</param>
internal sealed record UnionType(string Namespace, ValueArray<ContainingType> ContainingTypes, string Name, ValueArray<string> TypeParameters)
{
    private const string GeneratedSuffix = ".generated";
    private const string NameSeparator = ".";
    private const string TypeParameterSeparator = ",";

    /// <summary>
    /// Gets the name of the union followed by its type parameters, which is how the union refers to itself.
    /// </summary>
    public string NameWithTypeParameters => this.Name + UnionSourceHelper.GetTypeParameterList(this.TypeParameters);

    /// <summary>
    /// Creates a <see cref="UnionType"/> for the specified type.
    /// </summary>
    /// <param name="typeSymbol">The type symbol.</param>
    /// <returns>The union type.</returns>
    public static UnionType Create(INamedTypeSymbol typeSymbol)
    {
        var containingTypes = new Stack<ContainingType>();
        for (var containingType = typeSymbol.ContainingType; containingType != null; containingType = containingType.ContainingType)
        {
            containingTypes.Push(new ContainingType(containingType.Name, default, containingType.GetUnderlyingType(), containingType.TypeParameters.Select(x => x.Name).ToImmutableArray()));
        }

        return new UnionType(
            typeSymbol.ContainingNamespace.IsGlobalNamespace ? string.Empty : typeSymbol.ContainingNamespace.ToDisplayString(CodeAnalysisHelper.NamespaceQualifiedDisplayFormat),
            containingTypes.ToImmutableArray(),
            typeSymbol.Name,
            typeSymbol.TypeParameters.Select(x => x.Name).ToImmutableArray());
    }

    /// <summary>
    /// Gets the hint name for the source that is generated for the union, or for a type that is named after the union.
    /// </summary>
    /// <param name="nameSuffix">What follows the name of the union in the name of the type, if the source is for a type that is named after the union.</param>
    /// <returns>The hint name.</returns>
    public string GetHintName(string nameSuffix = "")
    {
        var names = new List<string>();
        if (this.Namespace.Length > 0)
        {
            names.Add(this.Namespace);
        }

        names.AddRange(this.ContainingTypes.Select(x => x.Name + GetTypeParameters(x.TypeParameters)));
        names.Add(this.Name + nameSuffix + GetTypeParameters(this.TypeParameters));
        return string.Join(NameSeparator, names) + GeneratedSuffix;
    }

    private static string GetTypeParameters(IReadOnlyCollection<string> typeParameters)
    {
        return typeParameters.Count > 0 ? '{' + string.Join(TypeParameterSeparator, typeParameters) + '}' : string.Empty;
    }
}
