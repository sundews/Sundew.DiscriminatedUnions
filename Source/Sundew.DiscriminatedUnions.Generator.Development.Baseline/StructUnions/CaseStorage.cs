// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CaseStorage.cs" company="Sundews">
// Copyright (c) Sundews. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Sundew.DiscriminatedUnions.Generator.StructUnions;

/// <summary>
/// The storage a generated struct union keeps a case in.
/// </summary>
internal enum CaseStorage
{
    /// <summary>
    /// The case has no storage, because it has no payload.
    /// </summary>
    None,

    /// <summary>
    /// The case is stored in a field of its own.
    /// </summary>
    Field,

    /// <summary>
    /// The case shares its storage with the other unmanaged cases, in a struct with explicit layout.
    /// </summary>
    UnmanagedField,

    /// <summary>
    /// The case shares a single reference field with the other reference cases.
    /// </summary>
    ReferenceField,
}
