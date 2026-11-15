// --------------------------------------------------------------------------------------------------------------------
// <copyright file="StructUnionCaseKind.cs" company="Sundews">
// Copyright (c) Sundews. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Sundew.DiscriminatedUnions.Generator.StructUnions;

/// <summary>
/// Describes how a struct union stores the value of a case.
/// </summary>
internal enum StructUnionCaseKind
{
    /// <summary>
    /// A struct without instance fields. It needs no storage, only a tag.
    /// </summary>
    PayloadFree,

    /// <summary>
    /// A struct that contains no references. It can share its storage with the other unmanaged cases.
    /// </summary>
    Unmanaged,

    /// <summary>
    /// A struct that may contain references. It is stored in a field of its own.
    /// </summary>
    Struct,

    /// <summary>
    /// A reference type. All reference cases share a single reference field.
    /// </summary>
    Reference,
}
