// --------------------------------------------------------------------------------------------------------------------
// <copyright file="UnionSegregationProblem.cs" company="Sundews">
// Copyright (c) Sundews. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Sundew.DiscriminatedUnions.Generator.Unions;

/// <summary>
/// Describes why a union that asks to be segregated cannot be.
/// </summary>
/// <param name="TypeName">The name of the type that the types that segregate the union could not refer to.</param>
/// <param name="Location">The location of the declaration of the union.</param>
internal sealed record UnionSegregationProblem(string TypeName, DiagnosticLocation Location);
