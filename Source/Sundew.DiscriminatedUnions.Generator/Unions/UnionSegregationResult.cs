// --------------------------------------------------------------------------------------------------------------------
// <copyright file="UnionSegregationResult.cs" company="Sundews">
// Copyright (c) Sundews. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Sundew.DiscriminatedUnions.Generator.Unions;

/// <summary>
/// The outcome for a union with regard to being segregated. It is empty when the union does not ask for it.
/// Otherwise it either has what is generated to segregate the union or the reason the union cannot be segregated.
/// </summary>
/// <param name="Segregation">What is generated to segregate the union, if it asks for it and can be segregated.</param>
/// <param name="Problem">The reason the union cannot be segregated, if it asks for it and cannot be.</param>
internal readonly record struct UnionSegregationResult(UnionSegregation? Segregation, UnionSegregationProblem? Problem)
{
    /// <summary>
    /// Gets a value indicating whether the union does not ask to be segregated, so that there is nothing to generate and nothing to report.
    /// </summary>
    public bool IsEmpty => this.Segregation == null && this.Problem == null;
}
