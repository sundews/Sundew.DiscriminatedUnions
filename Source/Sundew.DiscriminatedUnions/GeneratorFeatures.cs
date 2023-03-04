// --------------------------------------------------------------------------------------------------------------------
// <copyright file="GeneratorFeatures.cs" company="Hukano">
// Copyright (c) Hukano. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Sundew.DiscriminatedUnions
{
    using System;

    /// <summary>
    /// Enum for configuring the source generator.
    /// </summary>
    [Flags]
#pragma warning disable SA1649 // File header file name documentation should match file name
    internal enum GeneratorFeatures
#pragma warning restore SA1649 // File header file name documentation should match file name
    {
        /// <summary>
        /// Instructs the generator not to generate any sources.
        /// </summary>
        None,

        /// <summary>
        /// Instructs the generator to generate a segregate extension method for the discriminated union.
        /// </summary>
        Segregate,
    }
}