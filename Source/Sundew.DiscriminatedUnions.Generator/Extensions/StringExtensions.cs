// --------------------------------------------------------------------------------------------------------------------
// <copyright file="StringExtensions.cs" company="Hukano">
// Copyright (c) Hukano. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Sundew.DiscriminatedUnions.Generator.Extensions
{
    internal static class StringExtensions
    {
        private const string S = "s";
        private const string Es = "es";

        public static string Pluralize(this string text)
        {
            if (text.EndsWith(S))
            {
                return text + Es;
            }

            return text + S;
        }
    }
}
