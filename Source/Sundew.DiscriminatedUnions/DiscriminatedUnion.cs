// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DiscriminatedUnion.cs" company="Hukano">
// Copyright (c) Hukano. All rights reserved.
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
#pragma warning disable SA1649 // File header file name documentation should match file name
    internal class DiscriminatedUnion : Attribute
#pragma warning restore SA1649 // File header file name documentation should match file name
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