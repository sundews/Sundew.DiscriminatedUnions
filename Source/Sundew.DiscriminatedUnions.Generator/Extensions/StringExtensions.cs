// --------------------------------------------------------------------------------------------------------------------
// <copyright file="StringExtensions.cs" company="Hukano">
// Copyright (c) Hukano. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Sundew.DiscriminatedUnions.Generator.Extensions
{
    using System;

    internal static class StringExtensions
    {
        private const string S = "s";
        private const string Es = "es";
        private const string Ed = "ed";
        private const string All = "all";

        public static string Pluralize(this string text, bool force = false)
        {
            if (!force && IsInPluralForm(text))
            {
                return text;
            }

            if (text.EndsWith(S))
            {
                return text + Es;
            }

            return text + S;
        }

        private static bool IsInPluralForm(string text)
        {
            return text.EndsWith(Ed) || text.Equals(All, StringComparison.InvariantCultureIgnoreCase);
        }
    }
}
