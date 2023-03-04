// --------------------------------------------------------------------------------------------------------------------
// <copyright file="StringBuilderExtensions.cs" company="Hukano">
// Copyright (c) Hukano. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Sundew.DiscriminatedUnions.Generator.OutputStage;

using System;
using System.Text;
using Sundew.Base;
using Sundew.DiscriminatedUnions.Generator.Model;
using static GeneratorConstants;
using Type = Sundew.DiscriminatedUnions.Generator.Model.Type;

internal static class StringBuilderExtensions
{
    public static StringBuilder AppendType(this StringBuilder stringBuilder, Type type, bool fullyQualify = true, bool omitTypeParameters = false)
    {
        if (fullyQualify && type.Namespace != string.Empty)
        {
            stringBuilder.Append(type.Namespace);
            stringBuilder.Append('.');
        }

        stringBuilder.Append(type.Name);

        TryAppendTypeParameters(stringBuilder, type, omitTypeParameters);
        if (type.IsArray)
        {
            stringBuilder.Append('[').Append(']');
        }

        return stringBuilder;
    }

    public static void TryAppendTypeParameters(this StringBuilder stringBuilder, Type type, bool omitTypeParameters = false)
    {
        if (type.TypeParameters.Count > 0)
        {
            stringBuilder.Append('<');
            if (!omitTypeParameters)
            {
                type.TypeParameters.JoinToStringBuilder(stringBuilder, (builder, parameter) => builder.Append(parameter.Name), ListSeparator);
            }
            else
            {
                stringBuilder.Append(',', type.TypeParameters.Count - 1);
            }

            stringBuilder.Append('>');
        }
    }

    public static StringBuilder AppendUnderlyingType(this StringBuilder stringBuilder, UnderlyingType underlyingType)
    {
        stringBuilder.Append(underlyingType switch
        {
            UnderlyingType.Class => Class,
            UnderlyingType.Record => Record,
            UnderlyingType.Interface => Interface,
            _ => throw new ArgumentOutOfRangeException(nameof(underlyingType), underlyingType, null),
        });
        return stringBuilder;
    }

    public static StringBuilder AppendAccessibility(this StringBuilder stringBuilder, Accessibility accessibility)
    {
        stringBuilder.Append(accessibility switch
        {
            Accessibility.Internal => Internal,
            Accessibility.Public => Public,
            _ => throw new ArgumentOutOfRangeException(nameof(accessibility), accessibility, null),
        });
        return stringBuilder;
    }
}