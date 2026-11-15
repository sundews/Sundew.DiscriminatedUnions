// --------------------------------------------------------------------------------------------------------------------
// <copyright file="UnionSegregation.cs" company="Sundews">
// Copyright (c) Sundews. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Sundew.DiscriminatedUnions.Generator.Unions;

using Sundew.Base.Collections.Immutable;

/// <summary>
/// Represents what is generated for a union that asks to be segregated: a type with a list per case
/// and an extension method that distributes the unions in an enumerable to those lists.
/// </summary>
/// <param name="IsPublic">A value indicating whether the union and all of its cases are public. Otherwise what is generated is internal.</param>
/// <param name="UnionTypeName">The fully qualified name of the union, as it is written in generated code.</param>
/// <param name="TypeParameters">The type parameters of the types the union is nested in followed by those of the union.</param>
/// <param name="CaseMatching">The way to find out which case a union is.</param>
/// <param name="Cases">The cases, in the order they are declared.</param>
/// <param name="MatchOrder">The indices of the cases in the order they are matched, which is the most specific type first where the type tells the case.</param>
internal sealed record UnionSegregation(
    bool IsPublic,
    string UnionTypeName,
    ValueArray<UnionTypeParameter> TypeParameters,
    UnionCaseMatching CaseMatching,
    ValueArray<UnionSegregationCase> Cases,
    ValueArray<int> MatchOrder);
