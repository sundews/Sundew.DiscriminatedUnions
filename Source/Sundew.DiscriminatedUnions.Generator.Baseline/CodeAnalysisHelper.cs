// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CodeAnalysisHelper.cs" company="Hukano">
// Copyright (c) Hukano. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Sundew.DiscriminatedUnions.Generator;

using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using Microsoft.CodeAnalysis;
using Sundew.DiscriminatedUnions.Generator.DeclarationStage;
using Sundew.DiscriminatedUnions.Generator.Model;
using Type = Sundew.DiscriminatedUnions.Generator.Model.Type;

internal static class CodeAnalysisHelper
{
    private const string GlobalAssemblyAlias = "global";
    private const string Notnull = "notnull";
    private const string Class = "class";
    private const string Unmanaged = "unmanaged";
    private const string Struct = "struct";
    private const string NewConstructor = "new()";

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
                        GlobalAssemblyAlias,
                        namedTypeSymbol.TypeParameters.Length,
                        false);
            case IArrayTypeSymbol arrayTypeSymbol:
                return new Type(
                        arrayTypeSymbol.ElementType.Name,
                        arrayTypeSymbol.ElementType is ITypeParameterSymbol ? string.Empty : arrayTypeSymbol.ElementType.ContainingNamespace.ToDisplayString(NamespaceQualifiedDisplayFormat),
                        GlobalAssemblyAlias,
                        0,
                        true);
            case ITypeParameterSymbol typeParameterSymbol:
                return new Type(typeParameterSymbol.MetadataName, string.Empty, GlobalAssemblyAlias, 0, false);
            default:
                throw new ArgumentOutOfRangeException(nameof(typeSymbol));
        }
    }

    public static FullType GetFullType(this ITypeSymbol typeSymbol)
    {
        switch (typeSymbol)
        {
            case INamedTypeSymbol namedTypeSymbol:
                var @namespace = namedTypeSymbol.ContainingNamespace.ToDisplayString(NamespaceQualifiedDisplayFormat);
                return new FullType(
                    namedTypeSymbol.Name,
                    @namespace,
                    GlobalAssemblyAlias,
                    false,
                    new TypeMetadata(
                        namedTypeSymbol.IsGenericType
                            ? namedTypeSymbol.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat)
                                .Substring(GlobalAssemblyAlias.Length + @namespace.Length +
                                           namedTypeSymbol.Name.Length + 3)
                            : null,
                        namedTypeSymbol.TypeParameters.Select(x => new TypeParameter(
                            x.Name,
                            GetUnderlyingTypeConstraint(x)
                                .Concat(x.ConstraintTypes.Select(x =>
                                    x.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat)))
                                .Concat(GetNewConstraints(x)).ToImmutableArray())).ToImmutableArray()));
            case IArrayTypeSymbol arrayTypeSymbol:
                return new FullType(
                    arrayTypeSymbol.ElementType.Name,
                    arrayTypeSymbol.ElementType is ITypeParameterSymbol ? string.Empty : arrayTypeSymbol.ElementType.ContainingNamespace.ToDisplayString(NamespaceQualifiedDisplayFormat),
                    GlobalAssemblyAlias,
                    true,
                    new TypeMetadata(null, ImmutableArray<TypeParameter>.Empty));
            case ITypeParameterSymbol typeParameterSymbol:
                return new FullType(typeParameterSymbol.MetadataName, string.Empty, GlobalAssemblyAlias, false, new TypeMetadata(null, ImmutableArray<TypeParameter>.Empty));
            default:
                throw new ArgumentOutOfRangeException(nameof(typeSymbol));
        }
    }

    private static IEnumerable<string> GetNewConstraints(ITypeParameterSymbol typeParameterSymbol)
    {
        if (typeParameterSymbol.HasConstructorConstraint)
        {
            yield return NewConstructor;
        }
    }

    private static IEnumerable<string> GetUnderlyingTypeConstraint(ITypeParameterSymbol typeParameterSymbol)
    {
        switch (typeParameterSymbol)
        {
            case { HasNotNullConstraint: true }:
                yield return Notnull;
                break;
            case { HasReferenceTypeConstraint: true }:
                yield return Class;
                break;
            case { HasUnmanagedTypeConstraint: true }:
                yield return Unmanaged;
                break;
            case { HasValueTypeConstraint: true }:
                yield return Struct;
                break;
        }
    }
}