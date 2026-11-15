// --------------------------------------------------------------------------------------------------------------------
// <copyright file="StructUnionDiagnostic.cs" company="Sundews">
// Copyright (c) Sundews. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Sundew.DiscriminatedUnions.Generator.StructUnions;

/// <summary>
/// Represents a problem found in the declaration of a struct union.
/// </summary>
/// <param name="Problem">The problem.</param>
/// <param name="Argument">The name of the case or cases the problem is about, if any.</param>
internal readonly record struct StructUnionDiagnostic(StructUnionProblem Problem, string Argument);
