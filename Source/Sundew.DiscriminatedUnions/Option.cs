// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Option.cs" company="Hukano">
// Copyright (c) Hukano. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Sundew
{
    /// <summary>
    /// An option type discriminated union that wither some a value or none.
    /// </summary>
    /// <typeparam name="TValue">The type of the value.</typeparam>
    [Sundew.DiscriminatedUnions.DiscriminatedUnion]
    public abstract record Option<TValue>
        where TValue : notnull
    {
        private Option()
        {
        }

        /// <summary>
        /// The case containing a value.
        /// </summary>
        public sealed record Some(TValue Value) : Option<TValue>
        {
            /// <summary>
            /// Performs an implicit conversion from <see cref="Option{TValue}.Some"/>.
            /// </summary>
            /// <param name="some">The some.</param>
            /// <returns>
            /// The result of the conversion.
            /// </returns>
            public static implicit operator TValue(Some some)
            {
                return some.Value;
            }
        }

        /// <summary>
        /// The none case.
        /// </summary>
        public sealed record None : Option<TValue>;
    }
}