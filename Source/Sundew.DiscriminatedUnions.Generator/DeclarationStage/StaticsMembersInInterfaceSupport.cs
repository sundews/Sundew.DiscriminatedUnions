// --------------------------------------------------------------------------------------------------------------------
// <copyright file="StaticsMembersInInterfaceSupport.cs" company="Sundews">
// Copyright (c) Sundews. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Sundew.DiscriminatedUnions.Generator.DeclarationStage;

using System;

/// <summary>
/// Represents the support for static members in interfaces in the runtime.
/// </summary>
[Flags]
public enum StaticsMembersInInterfaceSupport
{
    /// <summary>
    /// Runtime has no support for static members in interfaces. This is the default value.
    /// </summary>
    None = 0,

    /// <summary>
    /// Runtime supports default interface members.
    /// </summary>
    DefaultInterfaceMember = 1,

    /// <summary>
    /// Runtime supports static abstract members.
    /// </summary>
    StaticAbstractInterfaceMember = DefaultInterfaceMember | 2,
}