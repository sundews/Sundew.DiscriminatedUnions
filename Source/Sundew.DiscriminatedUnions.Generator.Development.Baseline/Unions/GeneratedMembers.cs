// --------------------------------------------------------------------------------------------------------------------
// <copyright file="GeneratedMembers.cs" company="Sundews">
// Copyright (c) Sundews. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Sundew.DiscriminatedUnions.Generator.Unions;

using System;

/// <summary>
/// The members of a union that are only generated when the union does not declare them itself.
/// </summary>
[Flags]
internal enum GeneratedMembers
{
    /// <summary>
    /// None of the members are generated.
    /// </summary>
    None = 0,

    /// <summary>
    /// The list of cases, and with it the interface that gives access to the list.
    /// </summary>
    Cases = 1,

    /// <summary>
    /// The equality members: the equatable interface, the Equals methods, GetHashCode and the equality operators.
    /// </summary>
    Equality = 2,

    /// <summary>
    /// The ToString method.
    /// </summary>
    ToString = 4,

    /// <summary>
    /// All of the members are generated.
    /// </summary>
    All = Cases | Equality | ToString,
}
