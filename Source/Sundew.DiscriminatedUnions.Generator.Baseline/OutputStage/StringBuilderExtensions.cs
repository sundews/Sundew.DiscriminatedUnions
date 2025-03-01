﻿// --------------------------------------------------------------------------------------------------------------------
// <copyright file="StringBuilderExtensions.cs" company="Sundews">
// Copyright (c) Sundews. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Sundew.DiscriminatedUnions.Generator.OutputStage;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using Microsoft.CodeAnalysis;
using Sundew.Base.Collections.Immutable;
using Sundew.Base.Text;
using Sundew.DiscriminatedUnions.Generator.DeclarationStage;
using Sundew.DiscriminatedUnions.Generator.Model;
using static GeneratorConstants;
using Accessibility = Sundew.DiscriminatedUnions.Generator.Model.Accessibility;

internal static class StringBuilderExtensions
{
    private const string DebuggerNonUserCode = "global::System.Diagnostics.DebuggerNonUserCode";
    private const string GeneratedCodeAttribute = "global::System.CodeDom.Compiler.GeneratedCodeAttribute";
    private const string SundewDiscriminateUnionsGenerator = "Sundew.DiscriminateUnions.Generator";
    private const string Pragma = "#pragma";
    private const string Warning = "warning";
    private const string Restore = "restore";
    private const string Disable = "disable";
    private const string TypeparamStart = "/// <typeparam name=";
    private const string TheTypeOfThe = "The type of the";
    private const string TypeparamEnd = "</typeparam>";
    private const string ParamEnd = "</param>";
    private const string ParamStart = "/// <param name=";
    private const string The = "The";
    private const string ReturnsEnd = "</returns>";
    private const string ReturnsStart = "/// <returns>";
    private const string SummaryStart = "/// <summary>";
    private const string SummaryEnd = "/// </summary>";
    private const string Documentation = "/// ";
    private const string OutText = "out";
    private const string InText = "in";

    public static StringBuilder AppendType(this StringBuilder stringBuilder, in FullType fullType, bool fullyQualify = true, bool isForAttribute = false, bool isForPartial = false)
    {
        if (fullyQualify && fullType.Namespace != string.Empty)
        {
            stringBuilder
                .Append(fullType.AssemblyAlias)
                .Append(DoubleColon)
                .Append(fullType.Namespace)
                .Append('.');
        }

        if (isForAttribute)
        {
            stringBuilder.Append(fullType.NameForTypeOfAttribute);
        }
        else if (isForPartial)
        {
            stringBuilder.Append(fullType.Name).TryAppendGenericQualifier(fullType, false, false);
        }
        else
        {
            stringBuilder.Append(fullType.TypeMetadata.FullName);
        }

        if (fullType.IsArray)
        {
            stringBuilder.Append('[').Append(']');
        }

        return stringBuilder;
    }

    public static StringBuilder TryAppendGenericQualifier(this StringBuilder stringBuilder, in FullType fullType, bool omitTypeParameters = false, bool omitVariance = true)
    {
        string GetVariance(TypeParameter typeParameter)
        {
            return typeParameter.VarianceKind switch
            {
                VarianceKind.None => throw new ArgumentOutOfRangeException(),
                VarianceKind.Out => OutText,
                VarianceKind.In => InText,
                _ => throw new UnreachableCaseException(typeParameter.VarianceKind.GetType()),
            };
        }

        var typeParameterCount = fullType.TypeMetadata.TypeParameters.Count;
        if (typeParameterCount > 0)
        {
            if (!omitTypeParameters)
            {
                const string separator = ", ";
                stringBuilder
                    .Append('<')
                    .AppendItems(
                        fullType.TypeMetadata.TypeParameters,
                        (builder, typeParameter) =>
                        {
                            builder.If(!omitVariance && typeParameter.VarianceKind != VarianceKind.None, builder => builder.Append(GetVariance(typeParameter)).Append(' ')).Append(typeParameter.Name);
                        },
                        separator)
                    .Append('>');
            }
            else
            {
                stringBuilder.Append('<').Append(',', typeParameterCount - 1).Append('>');
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

    public static StringBuilder AppendPragmaWarning(this StringBuilder stringBuilder, bool restore, string id)
    {
        return stringBuilder.Append(Pragma)
            .Append(' ')
            .Append(Warning)
            .Append(' ')
            .Append(restore ? Restore : Disable)
            .Append(' ')
            .Append(id)
            .AppendLine();
    }

    public static StringBuilder AppendDocumentation(this StringBuilder stringBuilder, string indentation, string summaryFormat, object element, ValueArray<TypeParameter> typeParameters = default, IEnumerable<string>? parameters = default, string? returns = null)
    {
        stringBuilder
            .Append(indentation).Append(SummaryStart).AppendLine()
            .Append(indentation).Append(Documentation).AppendFormat(summaryFormat, element).Append('.').AppendLine()
            .Append(indentation).Append(SummaryEnd);

        if (!typeParameters.IsDefault)
        {
            stringBuilder.AppendItems(
                typeParameters,
                (builder, typeParameter) =>
                {
                    builder
                        .AppendLine()
                        .Append(indentation)
                        .Append(TypeparamStart).Append('\"').Append(typeParameter.Name).Append('\"').Append('>')
                        .Append(TheTypeOfThe).Append(' ')
                        .Append(typeParameter.Name.Length > 1
                            ? typeParameter.Name.Substring(1).ToLowerInvariant()
                            : typeParameter.Name.ToLowerInvariant())
                        .Append('.').Append(TypeparamEnd);
                },
                string.Empty);
        }

        if (parameters != null)
        {
            stringBuilder.AppendItems(
                parameters,
                (builder, parameter) =>
                {
                    var parameterName = parameter[0] == '@' ? parameter.Substring(1) : parameter;
                    builder
                        .AppendLine()
                        .Append(indentation)
                        .Append(ParamStart).Append('\"').Append(parameterName).Append('\"').Append('>')
                        .Append(The).Append(' ').Append(parameterName).Append('.').Append(ParamEnd);
                },
                string.Empty);
        }

        if (!string.IsNullOrEmpty(returns))
        {
            stringBuilder.AppendLine()
                .Append(indentation).Append(ReturnsStart).AppendFormat(returns, element).Append('.').Append(ReturnsEnd);
        }

        return stringBuilder.AppendLine();
    }

    public static StringBuilder TryAppendConstraints(this StringBuilder stringBuilder, ValueArray<TypeParameter> typeParameters, string indentation)
    {
        if (typeParameters.Any(x => x.Constraints.Any()))
        {
            return stringBuilder.AppendItems(
                typeParameters,
                (builder, parameter) =>
                {
                    builder.If(
                        !parameter.Constraints.IsEmpty,
                        builder1 => builder1.Append(indentation)
                            .Append(Where)
                            .Append(' ')
                            .Append(parameter.Name)
                            .Append(' ').Append(':').Append(' ')
                            .AppendItems(
                                parameter.Constraints,
                                (builder1, constraint) => builder1.Append(constraint),
                                ListSeparator)
                            .AppendLine());
                },
                string.Empty);
        }

        return stringBuilder;
    }

    public static StringBuilder AppendTypeAttributes(this StringBuilder stringBuilder, bool appendDebuggerAttribute)
    {
        stringBuilder
            .If(appendDebuggerAttribute, builder => builder.AppendDebuggerCodeAttribute(4))
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

    public static StringBuilder AppendDebuggerCodeAttribute(this StringBuilder stringBuilder, int indentation)
    {
        stringBuilder.Append(' ', indentation)
            .Append('[')
            .Append(DebuggerNonUserCode)
            .Append(']')
            .AppendLine();
        return stringBuilder;
    }
}