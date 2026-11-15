// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ClosedTypeDeclaration.cs" company="Sundews">
// Copyright (c) Sundews. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Sundew.DiscriminatedUnions.Generator.ClosedTypes;

using Sundew.Base.Collections.Immutable;
using Sundew.DiscriminatedUnions.Generator.Unions;

/// <summary>
/// Represents a closed class or record that something is generated for. A partial one gets the factories that create its cases and the list of its cases,
/// and one that asks to be segregated gets the types that do so.
/// </summary>
/// <param name="Type">The closed type.</param>
/// <param name="IsRecord">A value indicating whether the closed type is a record.</param>
/// <param name="HidesInheritedMembers">A value indicating whether the closed type derives from another type, whose members the generated ones may hide.</param>
/// <param name="CaseTypeNames">The fully qualified names of the case types, in the order the compiler lists them.</param>
/// <param name="GeneratedMembers">The members that are generated in the closed type, because it is partial and does not declare them itself.</param>
/// <param name="Factories">The factories, which create a case and return it as the closed type.</param>
/// <param name="Segregation">What is generated to segregate the closed type, or the reason it cannot be, if it asks for it.</param>
internal sealed record ClosedTypeDeclaration(
    UnionType Type,
    bool IsRecord,
    bool HidesInheritedMembers,
    ValueArray<string> CaseTypeNames,
    GeneratedMembers GeneratedMembers,
    ValueArray<UnionFactory> Factories,
    UnionSegregationResult Segregation)
{
    /// <summary>
    /// Gets a value indicating whether a part of the closed type is generated.
    /// </summary>
    public bool HasGeneratedPart => this.GeneratedMembers != GeneratedMembers.None || this.Factories.Count > 0;
}
