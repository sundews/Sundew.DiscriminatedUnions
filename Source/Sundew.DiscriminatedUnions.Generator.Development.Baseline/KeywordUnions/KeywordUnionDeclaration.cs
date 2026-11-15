// --------------------------------------------------------------------------------------------------------------------
// <copyright file="KeywordUnionDeclaration.cs" company="Sundews">
// Copyright (c) Sundews. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Sundew.DiscriminatedUnions.Generator.KeywordUnions;

using Sundew.Base.Collections.Immutable;
using Sundew.DiscriminatedUnions.Generator.Unions;

/// <summary>
/// Represents a union declaration that something is generated for. A partial one gets the members that a union declaration does not come with,
/// and one that asks to be segregated gets the types that do so.
/// </summary>
/// <param name="Type">The union type.</param>
/// <param name="CaseTypeNames">The fully qualified names of the case types, in the order they are declared.</param>
/// <param name="GeneratedMembers">The members that are generated in the union, because it is partial and does not declare them itself.</param>
/// <param name="Factories">The factories, which create the union from a new instance of a case.</param>
/// <param name="Segregation">What is generated to segregate the union, or the reason it cannot be, if it asks for it.</param>
internal sealed record KeywordUnionDeclaration(UnionType Type, ValueArray<string> CaseTypeNames, GeneratedMembers GeneratedMembers, ValueArray<UnionFactory> Factories, UnionSegregationResult Segregation)
{
    /// <summary>
    /// Gets a value indicating whether a part of the union is generated.
    /// </summary>
    public bool HasGeneratedPart => this.GeneratedMembers != GeneratedMembers.None || this.Factories.Count > 0;
}
