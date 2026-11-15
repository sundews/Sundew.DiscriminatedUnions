// --------------------------------------------------------------------------------------------------------------------
// <copyright file="StructUnionCase.cs" company="Sundews">
// Copyright (c) Sundews. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Sundew.DiscriminatedUnions.Generator.StructUnions;

using Sundew.Base.Collections.Immutable;

/// <summary>
/// Represents a case of a struct union.
/// </summary>
/// <param name="TypeName">The fully qualified name of the type the union stores for the case, as it is written in generated code. For a case that is declared as a nullable value type it is the type that is wrapped.</param>
/// <param name="DisplayName">The name of the case type, as it is written in documentation and diagnostics.</param>
/// <param name="Name">The simple name of the case type, which is used to name its storage.</param>
/// <param name="Kind">The kind that decides how the case is stored.</param>
/// <param name="IsDefault">A value indicating whether the case is what the default value of the union represents.</param>
/// <param name="CanOverlap">A value indicating whether the case can share its storage with the other cases that contain no references.</param>
/// <param name="IsNullable">A value indicating whether the case is declared as a nullable value type. The union is then created from the nullable type, and <c>null</c> creates it without a value.</param>
/// <param name="AssignableCases">The positions of the other cases whose values are this case as well, because their type converts to the type of this case: a class that derives from it or a type that implements it.</param>
/// <param name="CastableCases">The positions of the other cases whose values may be this case as well, which takes a type check: a base class of this case or an interface that a value of this case may implement.</param>
internal readonly record struct StructUnionCase(
    string TypeName,
    string DisplayName,
    string Name,
    StructUnionCaseKind Kind,
    bool IsDefault,
    bool CanOverlap,
    bool IsNullable,
    ValueArray<int> AssignableCases,
    ValueArray<int> CastableCases)
{
    private const string NullableSuffix = "?";

    /// <summary>
    /// Gets the fully qualified name of the case type as it is declared, which for a nullable value type includes that it is nullable.
    /// </summary>
    public string DeclaredTypeName => this.IsNullable ? this.TypeName + NullableSuffix : this.TypeName;

    /// <summary>
    /// Gets a value indicating whether the union can be created from <c>null</c> for this case.
    /// </summary>
    public bool AcceptsNull => this.IsNullable || this.Kind == StructUnionCaseKind.Reference;

    /// <summary>
    /// Gets a value indicating whether values of other cases are or may be this case as well.
    /// </summary>
    public bool IsMatchedByOtherCases => this.AssignableCases.Count > 0 || this.CastableCases.Count > 0;
}
