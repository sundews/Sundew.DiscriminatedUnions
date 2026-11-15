// --------------------------------------------------------------------------------------------------------------------
// <copyright file="StructUnionCaseClassifier.cs" company="Sundews">
// Copyright (c) Sundews. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Sundew.DiscriminatedUnions.Generator.StructUnions;

using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;

/// <summary>
/// Decides how a struct union can store a case.
/// The decision is based on the fields the compiler sees, which are not always the fields the runtime has:
/// the reference assemblies of older frameworks, such as .NET Standard 2.0, leave out the fields of structs.
/// A struct from such an assembly can neither be assumed to be empty nor to be free of references, so it is stored in a field of its own.
/// </summary>
internal sealed class StructUnionCaseClassifier
{
    private const string CanaryTypeMetadataName = "System.Guid";
    private const string SystemAssemblyNamePrefix = "System.";
    private static readonly string[] FrameworkAssemblyNames = ["mscorlib", "netstandard", "System"];

    private readonly IAssemblySymbol? fieldStrippedAssembly;

    /// <summary>
    /// Initializes a new instance of the <see cref="StructUnionCaseClassifier"/> class.
    /// </summary>
    /// <param name="compilation">The compilation.</param>
    public StructUnionCaseClassifier(Compilation compilation)
    {
        // A Guid always has fields at runtime, so when the compiler sees none the framework assemblies were stripped of them.
        var canaryType = compilation.GetTypeByMetadataName(CanaryTypeMetadataName);
        if (canaryType != null && !GetInstanceFields(canaryType).Any())
        {
            this.fieldStrippedAssembly = canaryType.ContainingAssembly;
        }
    }

    /// <summary>
    /// Gets the kind that decides how the specified case type is stored.
    /// </summary>
    /// <param name="type">The case type.</param>
    /// <returns>The kind.</returns>
    public StructUnionCaseKind GetKind(ITypeSymbol type)
    {
        if (type.IsReferenceType)
        {
            return StructUnionCaseKind.Reference;
        }

        if (type is INamedTypeSymbol { TypeKind: TypeKind.Struct, SpecialType: SpecialType.None } namedType
            && namedType.OriginalDefinition.SpecialType == SpecialType.None
            && !GetInstanceFields(namedType).Any()
            && !this.AreFieldsUnknown(namedType))
        {
            return StructUnionCaseKind.PayloadFree;
        }

        return this.IsUnmanaged(type, new HashSet<ITypeSymbol>(SymbolEqualityComparer.Default)) ? StructUnionCaseKind.Unmanaged : StructUnionCaseKind.Struct;
    }

    private static IEnumerable<IFieldSymbol> GetInstanceFields(INamedTypeSymbol type)
    {
        return type.GetMembers().OfType<IFieldSymbol>().Where(x => !x.IsStatic);
    }

    private static bool IsPrimitive(SpecialType specialType)
    {
        // The compiler does not show the field of a primitive type, so primitive types are recognized by their special type.
        switch (specialType)
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
            case SpecialType.System_Single:
            case SpecialType.System_Double:
            case SpecialType.System_Decimal:
            case SpecialType.System_IntPtr:
            case SpecialType.System_UIntPtr:
            case SpecialType.System_DateTime:
                return true;
            default:
                return false;
        }
    }

    private bool IsUnmanaged(ITypeSymbol type, HashSet<ITypeSymbol> typesBeingChecked)
    {
        switch (type.TypeKind)
        {
            case TypeKind.Enum:
            case TypeKind.Pointer:
            case TypeKind.FunctionPointer:
                return true;
            case TypeKind.Struct when type is INamedTypeSymbol namedType:
                if (IsPrimitive(namedType.SpecialType))
                {
                    return true;
                }

                if (!namedType.IsUnmanagedType || !typesBeingChecked.Add(namedType))
                {
                    return false;
                }

                var fields = GetInstanceFields(namedType).ToArray();
                var isUnmanaged = fields.Length == 0 ? !this.AreFieldsUnknown(namedType) : fields.All(x => this.IsUnmanaged(x.Type, typesBeingChecked));
                typesBeingChecked.Remove(namedType);
                return isUnmanaged;
            default:
                return false;
        }
    }

    private bool AreFieldsUnknown(INamedTypeSymbol type)
    {
        if (this.fieldStrippedAssembly == null || type.Locations.Any(x => x.IsInSource))
        {
            return false;
        }

        var assembly = type.ContainingAssembly;
        return SymbolEqualityComparer.Default.Equals(assembly, this.fieldStrippedAssembly)
               || Array.IndexOf(FrameworkAssemblyNames, assembly.Name) >= 0
               || assembly.Name.StartsWith(SystemAssemblyNamePrefix, StringComparison.Ordinal);
    }
}
