// --------------------------------------------------------------------------------------------------------------------
// <copyright file="UnionTypeParameter.cs" company="Sundews">
// Copyright (c) Sundews. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Sundew.DiscriminatedUnions.Generator.Unions;

using Sundew.Base.Collections.Immutable;

/// <summary>
/// Represents a type parameter of a union or of a type the union is nested in.
/// </summary>
/// <param name="Name">The name of the type parameter.</param>
/// <param name="Constraints">The constraints of the type parameter, as they are written in a where clause.</param>
internal readonly record struct UnionTypeParameter(string Name, ValueArray<string> Constraints);
