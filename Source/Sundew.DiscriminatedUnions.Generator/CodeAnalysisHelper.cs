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
using static Sundew.DiscriminatedUnions.Generator.OutputStage.GeneratorConstants;
using Type = Sundew.DiscriminatedUnions.Generator.Model.Type;

internal static class CodeAnalysisHelper
{
    private const string Bool = "bool";
    private const string Char = "char";
    private const string SByte = "sbyte";
    private const string Byte = "byte";
    private const string Short = "short";
    private const string Ushort = "ushort";
    private const string Int = "int";
    private const string Uint = "uint";
    private const string Long = "long";
    private const string Ulong = "ulong";
    private const string Decimal = "decimal";
    private const string Float = "float";
    private const string Double = "double";
    private const string String = "string";

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
                var (name, isPrimitive) = GetName(namedTypeSymbol);
                var @namespace = namedTypeSymbol.ContainingNamespace.ToDisplayString(NamespaceQualifiedDisplayFormat);
                return new Type(
                    name,
                    isPrimitive ? string.Empty : @namespace,
                    isPrimitive ? string.Empty : GlobalAssemblyAlias,
                    namedTypeSymbol.TypeParameters.Length,
                    false);
            case IArrayTypeSymbol arrayTypeSymbol:
                var (elementName, elementIsPrimitive) = GetName(arrayTypeSymbol.ElementType);
                return new Type(
                    elementName,
                    elementIsPrimitive || arrayTypeSymbol.ElementType is ITypeParameterSymbol ? string.Empty : arrayTypeSymbol.ElementType.ContainingNamespace.ToDisplayString(NamespaceQualifiedDisplayFormat),
                    elementIsPrimitive ? string.Empty : GlobalAssemblyAlias,
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
                var (name, isPrimitive) = GetName(namedTypeSymbol);
                var @namespace = namedTypeSymbol.ContainingNamespace.ToDisplayString(NamespaceQualifiedDisplayFormat);
                return new FullType(
                    name,
                    isPrimitive ? string.Empty : @namespace,
                    isPrimitive ? string.Empty : GlobalAssemblyAlias,
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
                var (elementName, elementIsPrimitive) = GetName(arrayTypeSymbol.ElementType);
                return new FullType(
                    elementName,
                    elementIsPrimitive || arrayTypeSymbol.ElementType is ITypeParameterSymbol ? string.Empty : arrayTypeSymbol.ElementType.ContainingNamespace.ToDisplayString(NamespaceQualifiedDisplayFormat),
                    elementIsPrimitive ? string.Empty : GlobalAssemblyAlias,
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

    private static (string Name, bool IsPrimitive) GetName(ITypeSymbol namedTypeSymbol)
    {
        return namedTypeSymbol.SpecialType switch
        {
            SpecialType.System_Boolean => (Bool, true),
            SpecialType.System_Char => (Char, true),
            SpecialType.System_SByte => (SByte, true),
            SpecialType.System_Byte => (Byte, true),
            SpecialType.System_Int16 => (Short, true),
            SpecialType.System_UInt16 => (Ushort, true),
            SpecialType.System_Int32 => (Int, true),
            SpecialType.System_UInt32 => (Uint, true),
            SpecialType.System_Int64 => (Long, true),
            SpecialType.System_UInt64 => (Ulong, true),
            SpecialType.System_Decimal => (Decimal, true),
            SpecialType.System_Single => (Float, true),
            SpecialType.System_Double => (Double, true),
            SpecialType.System_String => (String, true),
            _ => (namedTypeSymbol.Name, false),
        };
    }
}