// --------------------------------------------------------------------------------------------------------------------
// <copyright file="EnumExtensions.cs" company="Sundews">
// Copyright (c) Sundews. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Sundew.DiscriminatedUnions.Generator.Extensions;

using System;
using System.Globalization;

/// <summary>
/// Extends enumerations with easy to use methods.
/// </summary>
public static class EnumExtensions
{
    /// <summary>
    /// Converts an object to an enumeration member.
    /// </summary>
    /// <typeparam name="TEnum">The type of the enumeration.</typeparam>
    /// <param name="value">The value.</param>
    /// <returns>The enumeration.</returns>
    public static TEnum ToEnum<TEnum>(this object value)
        where TEnum : Enum
    {
        return (TEnum)Enum.ToObject(typeof(TEnum), value);
    }

    /// <summary>
    /// Converts an object to an enumeration member.
    /// </summary>
    /// <typeparam name="TEnum">The type of the enumeration.</typeparam>
    /// <param name="value">The value.</param>
    /// <returns>The enumeration.</returns>
    public static TEnum ToEnumOrDefault<TEnum>(this object? value)
        where TEnum : Enum
    {
        if (value == null)
        {
            return default!;
        }

        return value.ToEnum<TEnum>();
    }

    /// <summary>
    /// Converts an object to an enumeration member.
    /// </summary>
    /// <typeparam name="TEnum">The type of the enumeration.</typeparam>
    /// <param name="value">The value.</param>
    /// <param name="defaultValue">The default value.</param>
    /// <returns>
    /// The enumeration.
    /// </returns>
    public static TEnum ToEnumOrDefault<TEnum>(this object? value, TEnum defaultValue)
        where TEnum : Enum
    {
        if (value == null)
        {
            return defaultValue;
        }

        return value.ToEnum<TEnum>();
    }

    /// <summary>
    /// Converts a string to an enumeration member.
    /// </summary>
    /// <typeparam name="TEnum">The type of the enumeration.</typeparam>
    /// <param name="value">The value.</param>
    /// <param name="ignoreCase">A value indicating whether character casing should be ignored.</param>
    /// <returns>The enumeration.</returns>
    public static TEnum ParseEnum<TEnum>(this string value, bool ignoreCase = false)
        where TEnum : Enum
    {
        if (value.StartsWith(typeof(TEnum).Name + '.'))
        {
            value = value.Substring(typeof(TEnum).Name.Length + 1);
        }

        return (TEnum)Enum.Parse(typeof(TEnum), value, ignoreCase);
    }

    /// <summary>Converts a string to an enumeration member.</summary>
    /// <typeparam name="TEnum">The type of the enumeration.</typeparam>
    /// <param name="value">The value.</param>
    /// <param name="result">The resulting enum.</param>
    /// <param name="ignoreCase">A value indicating whether character casing should be ignored.</param>
    /// <returns><c>true</c> if the specified value could be successfully parsed..</returns>
    public static bool TryParseEnum<TEnum>(this string value, out TEnum result, bool ignoreCase = false)
        where TEnum : Enum
    {
#if NETSTANDARD2_1
        if (Enum.TryParse(typeof(TEnum), value, ignoreCase, out var actualValue))
        {
            result = (TEnum)actualValue;
            return true;
        }
#else
        try
        {
            result = value.ParseEnum<TEnum>(ignoreCase);
            return true;
        }
        catch (ArgumentException argumentException)
        {
            if (argumentException.ParamName == nameof(TEnum))
            {
                throw;
            }
        }
        catch (OverflowException)
        {
        }
        catch (FormatException)
        {
        }
#endif

        result = default!;
        return false;
    }

    /// <summary>Parses the value string to an enum with multiple values (flags).</summary>
    /// <typeparam name="TEnum">The type of the enum.</typeparam>
    /// <param name="value">The value.</param>
    /// <param name="cultureInfo">The culture info.</param>
    /// <param name="separator">The separator.</param>
    /// <param name="ignoreCase">if set to <c>true</c> [ignore case].</param>
    /// <returns>The parsed enum.</returns>
    /// <exception cref="ArgumentNullException">value.</exception>
    public static TEnum ParseFlagsEnum<TEnum>(
        this string value,
        CultureInfo cultureInfo,
        char separator = ',',
        bool ignoreCase = false)
        where TEnum : Enum
    {
        value.TryParseFlagsEnum(cultureInfo, out TEnum result, true, separator, ignoreCase);
        return result;
    }

    /// <summary>Parses the value string to an enum with multiple values (flags).</summary>
    /// <typeparam name="TEnum">The type of the enum.</typeparam>
    /// <param name="value">The value.</param>
    /// <param name="cultureInfo">The culture info.</param>
    /// <param name="result">The resulting enum.</param>
    /// <param name="separator">The separator.</param>
    /// <param name="ignoreCase">if set to <c>true</c> [ignore case].</param>
    /// <returns>The parsed enum.</returns>
    /// <exception cref="ArgumentNullException">value.</exception>
    public static bool TryParseFlagsEnum<TEnum>(
        this string value,
        CultureInfo cultureInfo,
        out TEnum result,
        char separator = ',',
        bool ignoreCase = false)
        where TEnum : Enum
    {
        return value.TryParseFlagsEnum(cultureInfo, out result, false, separator, ignoreCase);
    }

    private static bool TryParseFlagsEnum<TEnum>(
        this string value,
        CultureInfo cultureInfo,
        out TEnum result,
        bool throwOnError,
        char separator = ',',
        bool ignoreCase = false)
        where TEnum : Enum
    {
        if (value == null)
        {
            if (throwOnError)
            {
                throw new ArgumentNullException(nameof(value));
            }

            result = default!;
            return false;
        }

        var enumNames = value.Split(separator);
        ulong enumValue = 0;
        foreach (var enumName in enumNames)
        {
            if (throwOnError)
            {
                enumValue |= Convert.ToUInt64(enumName.Trim().ParseEnum<TEnum>(ignoreCase), cultureInfo);
            }
            else
            {
                if (enumName.Trim().TryParseEnum(out TEnum temporaryEnum, ignoreCase))
                {
                    enumValue |= Convert.ToUInt64(temporaryEnum, cultureInfo);
                }
                else
                {
                    result = default!;
                    return false;
                }
            }
        }

        result = enumValue.ToEnum<TEnum>();
        return true;
    }
}