// --------------------------------------------------------------------------------------------------------------------
// <copyright file="StructUnionDeclaration.cs" company="Sundews">
// Copyright (c) Sundews. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Sundew.DiscriminatedUnions.Generator.StructUnions;

using Sundew.Base.Collections.Immutable;
using Sundew.DiscriminatedUnions.Generator.Unions;

/// <summary>
/// Represents the declaration of a struct union: a partial struct that lists its cases as <c>ICase</c> interfaces.
/// </summary>
/// <param name="Type">The union type.</param>
/// <param name="IsReadOnly">A value indicating whether the union is declared readonly.</param>
/// <param name="CanUseExplicitLayout">A value indicating whether the union can contain a type with explicit layout, which generic types cannot. Otherwise that type is declared next to the union.</param>
/// <param name="Cases">The cases in the order they are declared.</param>
/// <param name="GeneratedMembers">The members that the union does not declare itself and that are generated for that reason.</param>
/// <param name="Factories">The factories, which create the union from a new instance of a case.</param>
/// <param name="Segregation">What is generated to segregate the union, or the reason it cannot be, if it asks for it.</param>
/// <param name="Diagnostics">The problems found in the declaration.</param>
/// <param name="Location">The location of the declaration, if there are problems to report at it.</param>
internal sealed record StructUnionDeclaration(
    UnionType Type,
    bool IsReadOnly,
    bool CanUseExplicitLayout,
    ValueArray<StructUnionCase> Cases,
    GeneratedMembers GeneratedMembers,
    ValueArray<UnionFactory> Factories,
    UnionSegregationResult Segregation,
    ValueArray<StructUnionDiagnostic> Diagnostics,
    DiagnosticLocation? Location);
