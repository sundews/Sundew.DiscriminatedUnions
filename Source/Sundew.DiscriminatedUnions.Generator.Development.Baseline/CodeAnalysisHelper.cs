// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CodeAnalysisHelper.cs" company="Sundews">
// Copyright (c) Sundews. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Sundew.DiscriminatedUnions.Generator;

using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;
using Microsoft.CodeAnalysis;
using Sundew.Base.Collections.Immutable;
using Sundew.DiscriminatedUnions.Generator.DeclarationStage;
using Sundew.DiscriminatedUnions.Generator.Model;
using Sundew.DiscriminatedUnions.Shared;
using static Sundew.DiscriminatedUnions.Generator.OutputStage.GeneratorConstants;
using Accessibility = Sundew.DiscriminatedUnions.Generator.Model.Accessibility;
using Type = Sundew.DiscriminatedUnions.Generator.Model.Type;

internal static class CodeAnalysisHelper
{
    private const string? OutText = "out";
    private const string? InText = "in";

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
                var (name, _, containingTypes, @namespace, isShortNameAlias) = GetNames(namedTypeSymbol);
                return new Type(
                    name,
                    isShortNameAlias ? string.Empty : @namespace,
                    isShortNameAlias ? string.Empty : GlobalAssemblyAlias,
                    containingTypes,
                    namedTypeSymbol.TypeParameters.Length,
                    false);
            case IArrayTypeSymbol arrayTypeSymbol:
                var (elementName, _, elementContainingTypes, @elementNamespace, elementIsShortNameAlias) = GetNames(arrayTypeSymbol.ElementType);
                return new Type(
                    elementName,
                    elementIsShortNameAlias || arrayTypeSymbol.ElementType is ITypeParameterSymbol ? string.Empty : @elementNamespace,
                    elementIsShortNameAlias ? string.Empty : GlobalAssemblyAlias,
                    elementContainingTypes,
                    0,
                    true);
            case ITypeParameterSymbol typeParameterSymbol:
                return new Type(typeParameterSymbol.MetadataName, string.Empty, string.Empty, ValueArray<ContainingType>.Empty, 0, false);
            default:
                throw new System.ArgumentOutOfRangeException(nameof(typeSymbol));
        }
    }

    public static FullType GetFullType(this ITypeSymbol typeSymbol, bool useFullyQualifiedFormat = false)
    {
        switch (typeSymbol)
        {
            case INamedTypeSymbol namedTypeSymbol:
                var (name, fullName, containingTypes, @namespace, isShortNameAlias) = GetNames(namedTypeSymbol);
                return new FullType(
                    name,
                    isShortNameAlias ? string.Empty : @namespace,
                    containingTypes,
                    isShortNameAlias ? string.Empty : GlobalAssemblyAlias,
                    false,
                    new TypeMetadata(
                        fullName,
                        namedTypeSymbol.TypeParameters.Select(x => new TypeParameter(
                            x.Name,
                            x.Variance,
                            GetUnderlyingTypeConstraint(x)
                                .Concat(x.ConstraintTypes.Select(x =>
                                    x.ToDisplayString(FullyQualifiedDisplayFormat)))
                                .Concat(GetNewConstraints(x)).ToImmutableArray())).ToImmutableArray()));
            case IArrayTypeSymbol arrayTypeSymbol:
                var (elementName, elementFullName, elementContainingTypes, elementNamespace, elementIsShortNameAlias) = GetNames(arrayTypeSymbol.ElementType);
                return new FullType(
                    elementName,
                    elementIsShortNameAlias || arrayTypeSymbol.ElementType is ITypeParameterSymbol ? string.Empty : elementNamespace,
                    elementContainingTypes,
                    elementIsShortNameAlias ? string.Empty : GlobalAssemblyAlias,
                    true,
                    new TypeMetadata(elementFullName, ImmutableArray<TypeParameter>.Empty));
            case ITypeParameterSymbol typeParameterSymbol:
                return new FullType(typeParameterSymbol.MetadataName, string.Empty, ValueArray<ContainingType>.Empty, string.Empty, false, new TypeMetadata(string.Empty, ImmutableArray<TypeParameter>.Empty));
            default:
                throw new System.ArgumentOutOfRangeException(nameof(typeSymbol));
        }
    }

    internal static UnderlyingType GetUnderlyingType(this ITypeSymbol typeSymbol)
    {
        return typeSymbol.TypeKind switch
        {
            TypeKind.Class => typeSymbol.IsRecord ? UnderlyingType.RecordClass : UnderlyingType.Class,
            TypeKind.Interface => UnderlyingType.Interface,
            TypeKind.Struct => typeSymbol.IsRecord ? UnderlyingType.RecordStruct : UnderlyingType.Struct,
            _ => throw new ArgumentOutOfRangeException(nameof(typeSymbol.TypeKind), typeSymbol.TypeKind, FormattableString.Invariant($"Unexpected TypeKind on {typeSymbol.Name}")),
        };
    }

    internal static bool TryGetSupportedAccessibility(this ITypeSymbol typeSymbol, out Accessibility accessibility)
    {
        switch (typeSymbol.DeclaredAccessibility)
        {
            case Microsoft.CodeAnalysis.Accessibility.Internal:
                accessibility = Accessibility.Internal;
                return true;
            case Microsoft.CodeAnalysis.Accessibility.Public:
                accessibility = Accessibility.Public;
                return true;
            default:
                accessibility = default;
                return false;
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

    private static (string Name, string FullName, ValueArray<ContainingType> ContainingTypes, string Namespace, bool IsShortNameAlias) GetNames(ITypeSymbol typeSymbol)
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
                return (fullName, fullName, ValueArray<ContainingType>.Empty, string.Empty, true);
            default:
                if (typeSymbol.OriginalDefinition.SpecialType == SpecialType.System_Nullable_T)
                {
                    return (fullName, fullName, ValueArray<ContainingType>.Empty, string.Empty, true);
                }

                return (typeSymbol.Name, fullName, GetContainingTypes(typeSymbol), @namespace, false);
        }
    }

    private static ValueArray<ContainingType> GetContainingTypes(ITypeSymbol typeSymbol)
    {
        static IEnumerable<INamedTypeSymbol> GetContainingTypes(INamedTypeSymbol? containingType)
        {
            while (containingType != null)
            {
                yield return containingType;
                containingType = containingType.ContainingType;
            }
        }

        return GetContainingTypes(typeSymbol.ContainingType).Reverse().Select(x =>
        {
            x.TryGetSupportedAccessibility(out var accessibility);
            return new ContainingType
            {
                Name = x.Name,
                Accessibility = accessibility,
                UnderlyingType = x.GetUnderlyingType(),
                TypeParameters = x.IsTypeGenericWithTypeParameters()
                        ? x.TypeParameters.Select(tp => tp.Name).ToValueArray()
                        : ValueArray<string>.Empty,
            };
        }).ToValueArray();
    }
}