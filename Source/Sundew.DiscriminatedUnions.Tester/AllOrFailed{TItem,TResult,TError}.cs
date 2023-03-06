// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AllOrFailed{TItem,TResult,TError}.cs" company="Hukano">
// Copyright (c) Hukano. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Sundew.Base.Collections;

using System;
using Sundew.DiscriminatedUnions;

/// <summary>
/// Represents the result of an ensured select.
/// </summary>
/// <typeparam name="TItem">The item type.</typeparam>
/// <typeparam name="TResult">The result type.</typeparam>
/// <typeparam name="TError">The error type.</typeparam>
[Sundew.DiscriminatedUnions.DiscriminatedUnion(GeneratorFeatures.Segregate)]
public abstract partial class AllOrFailed<TItem, TResult, TError>
    where TItem : class, IEquatable<TItem>
    where TResult : struct
    where TError : Exception, TItem
{
}