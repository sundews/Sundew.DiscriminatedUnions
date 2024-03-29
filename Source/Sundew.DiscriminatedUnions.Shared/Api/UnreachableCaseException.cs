﻿// --------------------------------------------------------------------------------------------------------------------
// <copyright file="UnreachableCaseException.cs" company="Sundews">
// Copyright (c) Sundews. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Sundew.DiscriminatedUnions
{
    using System;

    /// <summary>
    /// Exception used to silence CS8509 by throwing it in the default case of switch expression.
    /// </summary>
    /// <seealso cref="System.Exception" />
    internal class UnreachableCaseException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="UnreachableCaseException"/> class.
        /// </summary>
        /// <param name="enumType">Type of the enum.</param>
        public UnreachableCaseException(Type enumType)
            : base($"{enumType.Name} is not a valid discriminated union.")
        {
        }
    }
}