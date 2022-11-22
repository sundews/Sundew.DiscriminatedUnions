// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DiscriminatedUnion.cs" company="Hukano">
// Copyright (c) Hukano. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Sundew.DiscriminatedUnions;

using System;

/// <summary>
/// Attribute for indicating a discriminated union.
/// </summary>
/// <seealso cref="System.Attribute" />
[AttributeUsage(AttributeTargets.Class | AttributeTargets.Interface)]
internal class DiscriminatedUnion : Attribute
{
}