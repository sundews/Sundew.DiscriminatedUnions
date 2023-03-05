// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CodeAnalysisHelper.cs" company="Hukano">
// Copyright (c) Hukano. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Sundew.DiscriminatedUnions.Generator;

using System;
using System.Collections.Generic;
using Microsoft.CodeAnalysis;
using Type = Sundew.DiscriminatedUnions.Generator.Model.Type;

internal static class CodeAnalysisHelper
{
    private const string GlobalAssemblyAlias = "global";

    private static readonly HashSet<string> Keywords = new HashSet<string>
    {
        "abstract",
        "as",
        "base",
        "bool",
        "break",
        "byte",
        "case",
        "catch",
        "char",
        "checked",
        "class",
        "const",
        "continue",
        "decimal",
        "default",
        "delegate",
        "do",
        "double",
        "else",
        "enum",
        "event",
        "explicit",
        "extern",
        "false",
        "finally",
        "fixed",
        "float",
        "for",
        "foreach",
        "goto",
        "if",
        "implicit",
        "in",
        "int",
        "interface",
        "internal",
        "is",
        "lock",
        "long",
        "namespace",
        "new",
        "null",
        "object",
        "operator",
        "out",
        "override",
        "params",
        "private",
        "protected",
        "public",
        "readonly",
        "ref",
        "return",
        "sbyte",
        "sealed",
        "short",
        "sizeof",
        "stackalloc",
        "static",
        "string",
        "struct",
        "switch",
        "this",
        "throw",
        "true",
        "try",
        "typeof",
        "uint",
        "ulong",
        "unchecked",
        "unsafe",
        "ushort",
        "using",
        "virtual",
        "void",
        "volatile",
        "while",
    };

    public static SymbolDisplayFormat NamespaceQualifiedDisplayFormat { get; } = new SymbolDisplayFormat(
        globalNamespaceStyle: SymbolDisplayGlobalNamespaceStyle.Omitted,
        typeQualificationStyle: SymbolDisplayTypeQualificationStyle.NameAndContainingTypesAndNamespaces,
        genericsOptions: SymbolDisplayGenericsOptions.IncludeTypeParameters,
        miscellaneousOptions: SymbolDisplayMiscellaneousOptions.EscapeKeywordIdentifiers | SymbolDisplayMiscellaneousOptions.UseSpecialTypes);

    public static string AvoidKeywordCollision(this string value)
    {
        if (Keywords.Contains(value))
        {
            return '@' + value;
        }

        return value;
    }

    public static Type GetSourceType(this ITypeSymbol typeSymbol)
    {
        switch (typeSymbol)
        {
            case INamedTypeSymbol namedTypeSymbol:
                var @namespace = namedTypeSymbol.ContainingNamespace.ToDisplayString(NamespaceQualifiedDisplayFormat);
                return new Type(
                    namedTypeSymbol.Name,
                    @namespace,
                    namedTypeSymbol.IsGenericType ? namedTypeSymbol.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat).Substring(GlobalAssemblyAlias.Length + @namespace.Length + namedTypeSymbol.Name.Length + 3) : null,
                    GlobalAssemblyAlias,
                    namedTypeSymbol.TypeParameters.Length,
                    false);
            case IArrayTypeSymbol arrayTypeSymbol:
                return new Type(
                    arrayTypeSymbol.ElementType.Name,
                    arrayTypeSymbol.ElementType is ITypeParameterSymbol ? string.Empty : arrayTypeSymbol.ElementType.ContainingNamespace.ToDisplayString(NamespaceQualifiedDisplayFormat),
                    null,
                    GlobalAssemblyAlias,
                    0,
                    true);
            case ITypeParameterSymbol typeParameterSymbol:
                return new Type(typeParameterSymbol.MetadataName, string.Empty, null, GlobalAssemblyAlias, 0, false);
            default:
                throw new ArgumentOutOfRangeException(nameof(typeSymbol));
        }
    }
}