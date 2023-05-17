// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DiscriminatedUnion.cs" company="Sundews">
// Copyright (c) Sundews. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Sundew.DiscriminatedUnions
{
    using System;

    /// <summary>
    /// Attribute for indicating a discriminated union.
    /// </summary>
    /// <seealso cref="System.Attribute" />
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Interface)]
    internal class DiscriminatedUnion : Attribute
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DiscriminatedUnion"/> class.
        /// </summary>
        public DiscriminatedUnion()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DiscriminatedUnion"/> class.
        /// </summary>
        /// <param name="generatorFeatures">The generator features.</param>
        public DiscriminatedUnion(GeneratorFeatures generatorFeatures)
        {
            this.GeneratorFeatures = generatorFeatures;
        }

        /// <summary>
        /// Gets the generator features.
        /// </summary>
        public GeneratorFeatures GeneratorFeatures { get; }
    }
}