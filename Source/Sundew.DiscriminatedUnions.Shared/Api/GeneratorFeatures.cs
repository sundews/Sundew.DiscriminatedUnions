﻿// --------------------------------------------------------------------------------------------------------------------
// <copyright file="GeneratorFeatures.cs" company="Sundews">
// Copyright (c) Sundews. All rights reserved.
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
    internal enum GeneratorFeatures
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