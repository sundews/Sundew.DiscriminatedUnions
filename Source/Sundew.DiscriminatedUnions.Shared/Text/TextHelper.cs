// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TextHelper.cs" company="Sundews">
// Copyright (c) Sundews. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Sundew.DiscriminatedUnions.Text;

using System;

/// <summary>
/// Contains string extension methods.
/// </summary>
public static class TextHelper
{
    /// <summary>
    /// Converts the first character to lower case.
    /// </summary>
    /// <param name="text">The text.</param>
    /// <returns>The new string.</returns>
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