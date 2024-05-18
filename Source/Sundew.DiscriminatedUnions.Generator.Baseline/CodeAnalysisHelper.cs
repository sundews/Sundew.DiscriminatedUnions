// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CodeAnalysisHelper.cs" company="Sundews">
// Copyright (c) Sundews. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Sundew.DiscriminatedUnions.Generator;

using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;
using Microsoft.CodeAnalysis;
using Sundew.DiscriminatedUnions.Generator.DeclarationStage;
using Sundew.DiscriminatedUnions.Generator.Model;
using static Sundew.DiscriminatedUnions.Generator.OutputStage.GeneratorConstants;

internal static class CodeAnalysisHelper
{
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

    public static SymbolDisplayFormat FullyQualifiedDisplayFormat { get; } = new SymbolDisplayFormat(
        globalNamespaceStyle: SymbolDisplayGlobalNamespaceStyle.Included,
        typeQualificationStyle: SymbolDisplayTypeQualificationStyle.NameAndContainingTypesAndNamespaces,
        genericsOptions: SymbolDisplayGenericsOptions.IncludeTypeParameters,
        miscellaneousOptions: SymbolDisplayMiscellaneousOptions.EscapeKeywordIdentifiers | SymbolDisplayMiscellaneousOptions.UseSpecialTypes | SymbolDisplayMiscellaneousOptions.IncludeNullableReferenceTypeModifier);

    public static SymbolDisplayFormat NamespaceQualifiedDisplayFormat { get; } = new SymbolDisplayFormat(
        globalNamespaceStyle: SymbolDisplayGlobalNamespaceStyle.Omitted,
        typeQualificationStyle: SymbolDisplayTypeQualificationStyle.NameAndContainingTypesAndNamespaces,
        genericsOptions: SymbolDisplayGenericsOptions.IncludeTypeParameters,
        miscellaneousOptions: SymbolDisplayMiscellaneousOptions.EscapeKeywordIdentifiers | SymbolDisplayMiscellaneousOptions.UseSpecialTypes | SymbolDisplayMiscellaneousOptions.IncludeNullableReferenceTypeModifier | SymbolDisplayMiscellaneousOptions.EscapeKeywordIdentifiers);

    public static SymbolDisplayFormat FullyQualifiedParameterTypeFormat { get; } =
        new SymbolDisplayFormat(
            globalNamespaceStyle: SymbolDisplayGlobalNamespaceStyle.Included,
            typeQualificationStyle: SymbolDisplayTypeQualificationStyle.NameAndContainingTypesAndNamespaces,
            genericsOptions: SymbolDisplayGenericsOptions.IncludeTypeParameters,
            memberOptions: SymbolDisplayMemberOptions.IncludeParameters | SymbolDisplayMemberOptions.IncludeType | SymbolDisplayMemberOptions.IncludeRef | SymbolDisplayMemberOptions.IncludeContainingType,
            kindOptions: SymbolDisplayKindOptions.IncludeMemberKeyword,
            parameterOptions: SymbolDisplayParameterOptions.IncludeType | SymbolDisplayParameterOptions.IncludeParamsRefOut | SymbolDisplayParameterOptions.IncludeDefaultValue,
            localOptions: SymbolDisplayLocalOptions.IncludeType,
            miscellaneousOptions: SymbolDisplayMiscellaneousOptions.EscapeKeywordIdentifiers | SymbolDisplayMiscellaneousOptions.UseSpecialTypes | SymbolDisplayMiscellaneousOptions.IncludeNullableReferenceTypeModifier);

    public static SymbolDisplayFormat NameQualifiedTypeFormat { get; } =
        new SymbolDisplayFormat(
            globalNamespaceStyle: SymbolDisplayGlobalNamespaceStyle.Included,
            typeQualificationStyle: SymbolDisplayTypeQualificationStyle.NameAndContainingTypes,
            genericsOptions: SymbolDisplayGenericsOptions.IncludeTypeParameters,
            localOptions: SymbolDisplayLocalOptions.IncludeType,
            miscellaneousOptions: SymbolDisplayMiscellaneousOptions.EscapeKeywordIdentifiers | SymbolDisplayMiscellaneousOptions.UseSpecialTypes | SymbolDisplayMiscellaneousOptions.IncludeNullableReferenceTypeModifier);

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
                var (name, _, attributeName, @namespace, isShortNameAlias) = GetNames(namedTypeSymbol);
                return new Type(
                    name,
                    isShortNameAlias ? string.Empty : @namespace,
                    attributeName,
                    isShortNameAlias ? string.Empty : GlobalAssemblyAlias,
                    namedTypeSymbol.TypeParameters.Length,
                    false);
            case IArrayTypeSymbol arrayTypeSymbol:
                var (elementName, _, elementAttributeName, @elementNamespace, elementIsShortNameAlias) = GetNames(arrayTypeSymbol.ElementType);
                return new Type(
                    elementName,
                    elementIsShortNameAlias || arrayTypeSymbol.ElementType is ITypeParameterSymbol ? string.Empty : @elementNamespace,
                    elementAttributeName,
                    elementIsShortNameAlias ? string.Empty : GlobalAssemblyAlias,
                    0,
                    true);
            case ITypeParameterSymbol typeParameterSymbol:
                return new Type(typeParameterSymbol.MetadataName, string.Empty, string.Empty, string.Empty, 0, false);
            default:
                throw new System.ArgumentOutOfRangeException(nameof(typeSymbol));
        }
    }

    public static FullType GetFullType(this ITypeSymbol typeSymbol, bool useFullyQualifiedFormat = false)
    {
        string? GetGenericQualifier(INamedTypeSymbol namedTypeSymbol)
        {
            var name = namedTypeSymbol.ToDisplayString(useFullyQualifiedFormat ? FullyQualifiedDisplayFormat : NameQualifiedTypeFormat);
            var index = name.IndexOf('<');
            if (index > -1)
            {
                return name.Substring(index);
            }

            return null;
        }

        switch (typeSymbol)
        {
            case INamedTypeSymbol namedTypeSymbol:
                var (name, fullName, attributeName, @namespace, isShortNameAlias) = GetNames(namedTypeSymbol);
                return new FullType(
                    name,
                    isShortNameAlias ? string.Empty : @namespace,
                    attributeName,
                    isShortNameAlias ? string.Empty : GlobalAssemblyAlias,
                    false,
                    new TypeMetadata(
                        !isShortNameAlias && namedTypeSymbol.IsGenericType && namedTypeSymbol.TypeParameters.Length > 0
                            ? GetGenericQualifier(namedTypeSymbol)
                            : null,
                        fullName,
                        namedTypeSymbol.TypeParameters.Select(x => new TypeParameter(
                            x.Name,
                            GetUnderlyingTypeConstraint(x)
                                .Concat(x.ConstraintTypes.Select(x =>
                                    x.ToDisplayString(FullyQualifiedDisplayFormat)))
                                .Concat(GetNewConstraints(x)).ToImmutableArray())).ToImmutableArray()));
            case IArrayTypeSymbol arrayTypeSymbol:
                var (elementName, elementFullName, elementAttributeName, elementNamespace, elementIsShortNameAlias) = GetNames(arrayTypeSymbol.ElementType);
                return new FullType(
                    elementName,
                    elementIsShortNameAlias || arrayTypeSymbol.ElementType is ITypeParameterSymbol ? string.Empty : elementNamespace,
                    elementAttributeName,
                    elementIsShortNameAlias ? string.Empty : GlobalAssemblyAlias,
                    true,
                    new TypeMetadata(null, elementFullName, ImmutableArray<TypeParameter>.Empty));
            case ITypeParameterSymbol typeParameterSymbol:
                return new FullType(typeParameterSymbol.MetadataName, string.Empty, string.Empty, string.Empty, false, new TypeMetadata(null, string.Empty, ImmutableArray<TypeParameter>.Empty));
            default:
                throw new System.ArgumentOutOfRangeException(nameof(typeSymbol));
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

    private static (string Name, string FullName, string AttributeName, string Namespace, bool IsShortNameAlias) GetNames(ITypeSymbol typeSymbol)
    {
        var @namespace = typeSymbol.ContainingNamespace.ToDisplayString(NamespaceQualifiedDisplayFormat);
        var fullName = typeSymbol.ToDisplayString(FullyQualifiedParameterTypeFormat).Substring(GlobalAssemblyAlias.Length + 3 + @namespace.Length);
        switch (typeSymbol.SpecialType)
        {
            case SpecialType.System_Boolean:
            case SpecialType.System_Char:
            case SpecialType.System_SByte:
            case SpecialType.System_Byte:
            case SpecialType.System_Int16:
            case SpecialType.System_UInt16:
            case SpecialType.System_Int32:
            case SpecialType.System_UInt32:
            case SpecialType.System_Int64:
            case SpecialType.System_UInt64:
            case SpecialType.System_Decimal:
            case SpecialType.System_Single:
            case SpecialType.System_Double:
            case SpecialType.System_String:
                return (fullName, fullName, string.Empty, string.Empty, true);
            default:
                if (typeSymbol.OriginalDefinition.SpecialType == SpecialType.System_Nullable_T)
                {
                    return (fullName, fullName, string.Empty, string.Empty, true);
                }

                return (typeSymbol.Name, fullName, GetAttributeName(typeSymbol), @namespace, false);
        }
    }

    private static string GetAttributeName(ITypeSymbol typeSymbol)
    {
        var stringBuilder = new StringBuilder();
        var containingType = typeSymbol.ContainingType;
        while (containingType != null)
        {
            stringBuilder.Append(containingType.Name);
            if (containingType.IsGenericType && containingType.TypeParameters.Length > 0)
            {
                stringBuilder.Append('<').Append(',', containingType.TypeParameters.Length - 1).Append('>');
            }

            stringBuilder.Append('.');
            containingType = containingType.ContainingType;
        }

        stringBuilder.Append(typeSymbol.Name);
        if (typeSymbol is INamedTypeSymbol { IsGenericType: true, TypeParameters.Length: > 0 } namedTypeSymbol)
        {
            stringBuilder.Append('<').Append(',', namedTypeSymbol.TypeParameters.Length - 1).Append('>');
        }

        return stringBuilder.ToString();
    }
}