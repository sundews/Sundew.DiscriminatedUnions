// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TextHelper.cs" company="Hukano">
// Copyright (c) Hukano. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Sundew.DiscriminatedUnions.CodeFixes
{
    using System;

    internal static class TextHelper
    {
        public static string Uncapitalize(this string text)
        {
            var span = new Span<char>(text.ToCharArray());
            if (span.Length > 0)
            {
                span[0] = char.ToLowerInvariant(span[0]);
            }

            return span.ToString();
        }
    }
}