// --------------------------------------------------------------------------------------------------------------------
// <copyright file="UnionSegregationCase.cs" company="Sundews">
// Copyright (c) Sundews. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Sundew.DiscriminatedUnions.Generator.Unions;

/// <summary>
/// Represents a case of a union that is segregated: the case gets a list of its own.
/// </summary>
/// <param name="TypeName">The fully qualified name of the case type, as it is written in generated code.</param>
/// <param name="PatternTypeName">The name of the case type as it is written in a type pattern, which differs for the types that have a syntax of their own.</param>
/// <param name="DisplayName">The name of the case type, as it is written in documentation.</param>
/// <param name="Name">The name that identifies the case among the cases of the union, which the list of the case is named after.</param>
internal readonly record struct UnionSegregationCase(string TypeName, string PatternTypeName, string DisplayName, string Name);
