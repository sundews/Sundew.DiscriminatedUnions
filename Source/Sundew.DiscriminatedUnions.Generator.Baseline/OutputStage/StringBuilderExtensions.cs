// --------------------------------------------------------------------------------------------------------------------
// <copyright file="StringBuilderExtensions.cs" company="Hukano">
// Copyright (c) Hukano. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Sundew.DiscriminatedUnions.Generator.OutputStage;

using System;
using System.Linq;
using System.Reflection;
using System.Text;
using Sundew.Base;
using Sundew.DiscriminatedUnions.Generator.DeclarationStage;
using Sundew.DiscriminatedUnions.Generator.Model;
using static GeneratorConstants;
using Accessibility = Sundew.DiscriminatedUnions.Generator.Model.Accessibility;
using Type = Sundew.DiscriminatedUnions.Generator.Model.Type;

internal static class StringBuilderExtensions
{
    private const string DebuggerNonUserCode = "global::System.Diagnostics.DebuggerNonUserCode";
    private const string GeneratedCodeAttribute = "global::System.CodeDom.Compiler.GeneratedCodeAttribute";
    private const string SundewDiscriminateUnionsGenerator = "Sundew.DiscriminateUnions.Generator";

    public static StringBuilder AppendType(this StringBuilder stringBuilder, FullType fullType, bool fullyQualify = true, bool omitTypeParameters = false)
    {
        if (fullyQualify && fullType.Namespace != string.Empty)
        {
            stringBuilder
                .Append(fullType.AssemblyAlias)
                .Append(DoubleColon)
                .Append(fullType.Namespace)
                .Append('.');
        }

        stringBuilder.Append(fullType.Name);
        stringBuilder.TryAppendGenericQualifier(fullType, omitTypeParameters);
        if (fullType.IsArray)
        {
            stringBuilder.Append('[').Append(']');
        }

        return stringBuilder;
    }

    public static StringBuilder TryAppendGenericQualifier(this StringBuilder stringBuilder, FullType fullType, bool omitTypeParameters = false)
    {
        if (fullType.TypeMetadata.GenericQualifier != null)
        {
            if (!omitTypeParameters)
            {
                stringBuilder.Append(fullType.TypeMetadata.GenericQualifier);
            }
            else
            {
                stringBuilder
                    .Append('<')
                    .Append(',', fullType.TypeMetadata.TypeParameters.Count - 1)
                    .Append('>');
            }
        }

        return stringBuilder;
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

    public static StringBuilder TryAppendConstraints(this StringBuilder stringBuilder, ValueArray<TypeParameter> typeParameters, string indentation)
    {
        if (typeParameters.Any(x => x.Constraints.Any()))
        {
            return typeParameters.JoinToStringBuilder(
                stringBuilder,
                (builder, parameter) =>
                {
                    builder
                        .Append(indentation)
                        .Append(Where)
                        .Append(' ')
                        .Append(parameter.Name)
                        .Append(' ')
                        .Append(':')
                        .Append(' ');
                    parameter.Constraints.JoinToStringBuilder(
                            stringBuilder,
                            (builder1, constraint) => builder1.Append(constraint),
                            ListSeparator)
                        .AppendLine();
                },
                string.Empty);
        }

        return stringBuilder;
    }

    public static StringBuilder AppendTypeAttributes(this StringBuilder stringBuilder)
    {
        stringBuilder.Append(SpaceIndentedBy4)
            .Append('[')
            .Append(DebuggerNonUserCode)
            .Append(']')
            .AppendLine()
            .Append(SpaceIndentedBy4)
            .Append('[')
            .Append(GeneratedCodeAttribute)
            .Append('(')
            .Append('\"').Append(SundewDiscriminateUnionsGenerator).Append('\"')
            .Append(ListSeparator)
            .Append('\"').Append(Assembly.GetExecutingAssembly().GetName().Version).Append('\"')
            .Append(')')
            .Append(']')
            .AppendLine();
        return stringBuilder;
    }

    private static StringBuilder AppendUnderlyingTypeConstraint(this StringBuilder stringBuilder, UnderlyingTypeConstraint underlyingTypeConstraint)
    {
        switch (underlyingTypeConstraint)
        {
            case UnderlyingTypeConstraint.None:
                return stringBuilder;
            case UnderlyingTypeConstraint.Class:
                return stringBuilder.Append(Class).Append(' ');
            case UnderlyingTypeConstraint.Struct:
                return stringBuilder.Append(Struct).Append(' ');
            case UnderlyingTypeConstraint.NotNull:
                return stringBuilder.Append(Notnull).Append(' ');
            case UnderlyingTypeConstraint.Unmanaged:
                return stringBuilder.Append(Unmanaged).Append(' ');
            default:
                throw new ArgumentOutOfRangeException(nameof(underlyingTypeConstraint), underlyingTypeConstraint, null);
        }
    }
}