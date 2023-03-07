// --------------------------------------------------------------------------------------------------------------------
// <copyright file="StringBuilderExtensions.AppendItems.cs" company="Hukano">
// Copyright (c) Hukano. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Sundew.Base.Text;

using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;

/// <summary>
/// Extends the string with extension methods.
/// </summary>
public static partial class StringBuilderExtensions
{
    private const string Format = "{0}";

    /// <summary>
    /// Joins the specified enumerable to string builder.
    /// </summary>
    /// <typeparam name="TItem">The type of the item.</typeparam>
    /// <param name="stringBuilder">The string builder.</param>
    /// <param name="enumerable">The enumerable.</param>
    /// <param name="separator">The separator.</param>
    /// <param name="formatProvider">The format provider.</param>
    /// <param name="skipNullValues">if set to <c>true</c> [skip null values].</param>
    /// <returns>
    /// The result of the result function.
    /// </returns>
    public static StringBuilder AppendItems<TItem>(this StringBuilder stringBuilder, IEnumerable<TItem> enumerable, char separator, IFormatProvider formatProvider, bool skipNullValues = true)
    {
        return InternalAppendItems(stringBuilder, enumerable, separator, formatProvider, skipNullValues);
    }

    /// <summary>
    /// Aggregates to string builder.
    /// </summary>
    /// <typeparam name="TItem">The type of the item.</typeparam>
    /// <param name="stringBuilder">The string builder.</param>
    /// <param name="enumerable">The enumerable.</param>
    /// <param name="separator">The separator.</param>
    /// <param name="formatProvider">The format provider.</param>
    /// <param name="skipNullValues">if set to <c>true</c> [skip null values].</param>
    /// <returns>
    /// The result of the result function.
    /// </returns>
    public static StringBuilder AppendItems<TItem>(this StringBuilder stringBuilder, IEnumerable<TItem> enumerable, string separator, IFormatProvider formatProvider, bool skipNullValues = true)
    {
        return InternalAppendItems(stringBuilder, enumerable, separator, formatProvider, skipNullValues);
    }

    /// <summary>
    /// Aggregates to string builder.
    /// </summary>
    /// <typeparam name="TItem">The type of the item.</typeparam>
    /// <param name="stringBuilder">The string builder.</param>
    /// <param name="enumerable">The enumerable.</param>
    /// <param name="appendAction">The append action.</param>
    /// <returns>
    /// The result of the result function.
    /// </returns>
    public static StringBuilder AppendItems<TItem>(
        this StringBuilder stringBuilder,
        IEnumerable<TItem> enumerable,
        Action<StringBuilder, TItem> appendAction)
    {
        return InternalAppendItems(stringBuilder, enumerable, appendAction, appendAction, null, string.Empty);
    }

    /// <summary>
    /// Aggregates to string builder.
    /// </summary>
    /// <typeparam name="TItem">The type of the item.</typeparam>
    /// <param name="stringBuilder">The string builder.</param>
    /// <param name="enumerable">The enumerable.</param>
    /// <param name="appendAction">The append action.</param>
    /// <param name="separator">The separator.</param>
    /// <returns>
    /// The result of the result function.
    /// </returns>
    public static StringBuilder AppendItems<TItem>(
        this StringBuilder stringBuilder,
        IEnumerable<TItem> enumerable,
        Action<StringBuilder, TItem> appendAction,
        char separator)
    {
        return InternalAppendItems(stringBuilder, enumerable, appendAction, appendAction, null, separator);
    }

    /// <summary>
    /// Aggregates to string builder.
    /// </summary>
    /// <typeparam name="TItem">The type of the item.</typeparam>
    /// <param name="stringBuilder">The string builder.</param>
    /// <param name="enumerable">The enumerable.</param>
    /// <param name="appendAction">The append action.</param>
    /// <param name="separator">The separator.</param>
    /// <returns>
    /// The result of the result function.
    /// </returns>
    public static StringBuilder AppendItems<TItem>(
        this StringBuilder stringBuilder,
        IEnumerable<TItem> enumerable,
        Action<StringBuilder, TItem> appendAction,
        string separator)
    {
        return InternalAppendItems(stringBuilder, enumerable, appendAction, appendAction, null, separator);
    }

    /// <summary>
    /// Aggregates to string builder.
    /// </summary>
    /// <typeparam name="TItem">The type of the item.</typeparam>
    /// <param name="stringBuilder">The string builder.</param>
    /// <param name="enumerable">The enumerable.</param>
    /// <param name="firstAppendAction">The first append action.</param>
    /// <param name="appendAction">The append action.</param>
    /// <param name="separator">The separator.</param>
    /// <returns>
    /// The result of the result function.
    /// </returns>
    public static StringBuilder AppendItems<TItem>(
        this StringBuilder stringBuilder,
        IEnumerable<TItem> enumerable,
        Action<StringBuilder, TItem> firstAppendAction,
        Action<StringBuilder, TItem> appendAction,
        char separator)
    {
        return InternalAppendItems(stringBuilder, enumerable, firstAppendAction, appendAction, null, separator);
    }

    /// <summary>
    /// Aggregates to string builder.
    /// </summary>
    /// <typeparam name="TItem">The type of the item.</typeparam>
    /// <param name="stringBuilder">The string builder.</param>
    /// <param name="enumerable">The enumerable.</param>
    /// <param name="firstAppendAction">The first append action.</param>
    /// <param name="appendAction">The append action.</param>
    /// <param name="separator">The separator.</param>
    /// <returns>
    /// The result of the result function.
    /// </returns>
    public static StringBuilder AppendItems<TItem>(
        this StringBuilder stringBuilder,
        IEnumerable<TItem> enumerable,
        Action<StringBuilder, TItem> firstAppendAction,
        Action<StringBuilder, TItem> appendAction,
        string separator)
    {
        return InternalAppendItems(stringBuilder, enumerable, firstAppendAction, appendAction, null, separator);
    }

    /// <summary>
    /// Aggregates to string builder.
    /// </summary>
    /// <typeparam name="TItem">The type of the item.</typeparam>
    /// <param name="stringBuilder">The string builder.</param>
    /// <param name="enumerable">The enumerable.</param>
    /// <param name="appendAction">The append action.</param>
    /// <param name="finalAppendAction">The final append action.</param>
    /// <param name="separator">The separator.</param>
    /// <returns>
    /// The result of the result function.
    /// </returns>
    public static StringBuilder AppendItems<TItem>(
        this StringBuilder stringBuilder,
        IEnumerable<TItem> enumerable,
        Action<StringBuilder, TItem> appendAction,
        Action<StringBuilder> finalAppendAction,
        char separator)
    {
        return InternalAppendItems(stringBuilder, enumerable, appendAction, appendAction, finalAppendAction, separator);
    }

    /// <summary>
    /// Aggregates to string builder.
    /// </summary>
    /// <typeparam name="TItem">The type of the item.</typeparam>
    /// <param name="stringBuilder">The string builder.</param>
    /// <param name="enumerable">The enumerable.</param>
    /// <param name="appendAction">The append action.</param>
    /// <param name="finalAppendAction">The final append action.</param>
    /// <param name="separator">The separator.</param>
    /// <returns>
    /// The result of the result function.
    /// </returns>
    public static StringBuilder AppendItems<TItem>(
        this StringBuilder stringBuilder,
        IEnumerable<TItem> enumerable,
        Action<StringBuilder, TItem> appendAction,
        Action<StringBuilder> finalAppendAction,
        string separator)
    {
        return InternalAppendItems(stringBuilder, enumerable, appendAction, appendAction, finalAppendAction, separator);
    }

    /// <summary>
    /// Aggregates to string builder.
    /// </summary>
    /// <typeparam name="TItem">The type of the item.</typeparam>
    /// <param name="stringBuilder">The string builder.</param>
    /// <param name="enumerable">The enumerable.</param>
    /// <param name="firstAppendAction">The first append action.</param>
    /// <param name="appendAction">The append action.</param>
    /// <param name="finalAppendAction">The final append action.</param>
    /// <param name="separator">The separator.</param>
    /// <returns>
    /// The result of the result function.
    /// </returns>
    public static StringBuilder AppendItems<TItem>(
        this StringBuilder stringBuilder,
        IEnumerable<TItem> enumerable,
        Action<StringBuilder, TItem> firstAppendAction,
        Action<StringBuilder, TItem> appendAction,
        Action<StringBuilder> finalAppendAction,
        char separator)
    {
        return InternalAppendItems(stringBuilder, enumerable, firstAppendAction, appendAction, finalAppendAction, separator);
    }

    /// <summary>
    /// Aggregates to string builder.
    /// </summary>
    /// <typeparam name="TItem">The type of the item.</typeparam>
    /// <param name="stringBuilder">The string builder.</param>
    /// <param name="enumerable">The enumerable.</param>
    /// <param name="firstAppendAction">The first append action.</param>
    /// <param name="appendAction">The append action.</param>
    /// <param name="finalAppendAction">The final append action.</param>
    /// <param name="separator">The separator.</param>
    /// <returns>
    /// The result of the result function.
    /// </returns>
    public static StringBuilder AppendItems<TItem>(
        this StringBuilder stringBuilder,
        IEnumerable<TItem> enumerable,
        Action<StringBuilder, TItem> firstAppendAction,
        Action<StringBuilder, TItem> appendAction,
        Action<StringBuilder> finalAppendAction,
        string separator)
    {
        return InternalAppendItems(stringBuilder, enumerable, firstAppendAction, appendAction, finalAppendAction, separator);
    }

    /// <summary>
    /// Appends the specified value.
    /// </summary>
    /// <param name="stringBuilder">The string builder.</param>
    /// <param name="value">The value.</param>
    /// <param name="formatProvider">The format provider.</param>
    /// <returns>
    /// The <see cref="StringBuilder" />.
    /// </returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static StringBuilder Append(this StringBuilder stringBuilder, object value, IFormatProvider formatProvider)
    {
        return stringBuilder.AppendFormat(formatProvider, Format, value);
    }

    internal static StringBuilder InternalAppendItems<TItem>(StringBuilder stringBuilder, IEnumerable<TItem> enumerable, Action<StringBuilder, TItem> firstAppendAction, Action<StringBuilder, TItem> successiveAppendAction, Action<StringBuilder>? finalAppendAction, string separator)
    {
        using (IEnumerator<TItem> enumerator = enumerable.GetEnumerator())
        {
            if (!enumerator.MoveNext())
            {
                return stringBuilder;
            }

            var value = enumerator.Current;
            if (value != null)
            {
                firstAppendAction(stringBuilder, value);
            }

            while (enumerator.MoveNext())
            {
                stringBuilder.Append(separator);
                value = enumerator.Current;
                if (value != null)
                {
                    successiveAppendAction(stringBuilder, value);
                }
            }
        }

        finalAppendAction?.Invoke(stringBuilder);
        return stringBuilder;
    }

    internal static StringBuilder InternalAppendItems<TItem>(StringBuilder stringBuilder, IEnumerable<TItem> enumerable, Action<StringBuilder, TItem> firstAppendAction, Action<StringBuilder, TItem> successiveAppendAction, Action<StringBuilder>? finalAppendAction, char separator)
    {
        using (IEnumerator<TItem> enumerator = enumerable.GetEnumerator())
        {
            if (!enumerator.MoveNext())
            {
                return stringBuilder;
            }

            var value = enumerator.Current;
            if (value != null)
            {
                firstAppendAction(stringBuilder, value);
            }

            while (enumerator.MoveNext())
            {
                stringBuilder.Append(separator);
                value = enumerator.Current;
                if (value != null)
                {
                    successiveAppendAction(stringBuilder, value);
                }
            }
        }

        finalAppendAction?.Invoke(stringBuilder);
        return stringBuilder;
    }

    internal static StringBuilder InternalAppendItems<TItem>(StringBuilder stringBuilder, IEnumerable<TItem> enumerable, string separator, IFormatProvider formatProvider, bool skipNullValues)
    {
        using (IEnumerator<TItem> enumerator = enumerable.GetEnumerator())
        {
            if (!enumerator.MoveNext())
            {
                return stringBuilder;
            }

            var value = enumerator.Current;
            var previousWasSet = value != null;
            if (value != null)
            {
                stringBuilder.Append(value, formatProvider);
            }

            while (enumerator.MoveNext())
            {
                if (previousWasSet || !skipNullValues)
                {
                    stringBuilder.Append(separator);
                }

                value = enumerator.Current;
                previousWasSet = value != null;
                if (value != null)
                {
                    stringBuilder.Append(value, formatProvider);
                }
            }
        }

        return stringBuilder;
    }

    internal static StringBuilder InternalAppendItems<TItem>(StringBuilder stringBuilder, IEnumerable<TItem> enumerable, char separator, IFormatProvider formatProvider, bool skipNullValues)
    {
        using (IEnumerator<TItem> enumerator = enumerable.GetEnumerator())
        {
            if (!enumerator.MoveNext())
            {
                return stringBuilder;
            }

            var value = enumerator.Current;
            var previousWasSet = value != null;
            if (value != null)
            {
                stringBuilder.Append(value, formatProvider);
            }

            while (enumerator.MoveNext())
            {
                if (previousWasSet || !skipNullValues)
                {
                    stringBuilder.Append(separator);
                }

                value = enumerator.Current;
                previousWasSet = value != null;
                if (value != null)
                {
                    stringBuilder.Append(value, formatProvider);
                }
            }
        }

        return stringBuilder;
    }
}