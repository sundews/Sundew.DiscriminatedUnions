// --------------------------------------------------------------------------------------------------------------------
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
using Sundew.Base;
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

    public static StringBuilder AppendType(this StringBuilder stringBuilder, in FullType fullType, bool fullyQualify = true, bool omitTypeParameters = false)
    {
        if (fullyQualify && fullType.Namespace != string.Empty)
        {
            stringBuilder
                .Append(fullType.AssemblyAlias)
                .Append(DoubleColon)
                .Append(fullType.Namespace)
                .Append('.');
        }

        stringBuilder.Append(fullType.NestedTypeQualifier);
        stringBuilder.Append(fullType.Name);
        stringBuilder.TryAppendGenericQualifier(fullType, omitTypeParameters);
        if (fullType.IsArray)
        {
            stringBuilder.Append('[').Append(']');
        }

        return stringBuilder;
    }

    public static StringBuilder TryAppendGenericQualifier(this StringBuilder stringBuilder, in FullType fullType, bool omitTypeParameters = false)
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
                    builder
                        .Append(indentation)
                        .Append(Where)
                        .Append(' ')
                        .Append(parameter.Name)
                        .Append(' ')
                        .Append(':')
                        .Append(' ');
                    stringBuilder.AppendItems(
                            parameter.Constraints,
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
}