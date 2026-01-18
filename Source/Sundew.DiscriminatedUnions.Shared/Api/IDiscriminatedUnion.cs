// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IDiscriminatedUnion.cs" company="Sundews">
// Copyright (c) Sundews. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

#if NET8_0_OR_GREATER
namespace Sundew.DiscriminatedUnions
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Interface to enable reflection on unions.
    /// </summary>
    public interface IDiscriminatedUnion
    {
        /// <summary>
        /// Gets all cases in the union.
        /// </summary>
        static abstract IReadOnlyList<Type> Cases { get; }
    }
}
#endif