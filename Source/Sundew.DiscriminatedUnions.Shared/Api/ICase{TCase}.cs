// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ICase{TCase}.cs" company="Sundews">
// Copyright (c) Sundews. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Sundew.DiscriminatedUnions
{
    /// <summary>
    /// Declares a case of a struct union.
    /// A readonly partial struct that lists one <see cref="ICase{TCase}"/> per case gets its storage, constructors and union members generated,
    /// so that matching and converting to the union does not allocate.
    /// </summary>
    /// <typeparam name="TCase">The case type.</typeparam>
    internal interface ICase<TCase>
    {
    }
}
