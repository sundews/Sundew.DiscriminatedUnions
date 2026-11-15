// --------------------------------------------------------------------------------------------------------------------
// <copyright file="KeywordUnionSourceBuilder.cs" company="Sundews">
// Copyright (c) Sundews. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Sundew.DiscriminatedUnions.Generator.KeywordUnions;

using System.Collections.Generic;
using Sundew.DiscriminatedUnions.Generator.DeclarationStage;
using Sundew.DiscriminatedUnions.Generator.Unions;
using static Sundew.DiscriminatedUnions.Generator.Unions.UnionSourceHelper;

/// <summary>
/// Builds the source of the generated part of a partial union declaration.
/// A union declaration comes with a single member, the value it contains. The generated part adds the list of cases,
/// which a union does not have at runtime, as well as equality and a description of the contained case.
/// </summary>
internal static class KeywordUnionSourceBuilder
{
    /// <summary>
    /// Builds the source for the specified union declaration.
    /// </summary>
    /// <param name="keywordUnion">The union declaration.</param>
    /// <param name="featureSupport">The features supported by the target framework.</param>
    /// <returns>The source.</returns>
    public static string Build(KeywordUnionDeclaration keywordUnion, FeatureSupport featureSupport)
    {
        var unionName = keywordUnion.Type.NameWithTypeParameters;
        var generatedMembers = keywordUnion.GeneratedMembers;
        var interfaces = new List<string>();
        if (generatedMembers.HasFlag(GeneratedMembers.Equality))
        {
            interfaces.Add(GetEquatableInterface(unionName));
        }

        if (generatedMembers.HasFlag(GeneratedMembers.Cases) && featureSupport.StaticsMembersInInterfaceSupport == StaticsMembersInInterfaceSupport.StaticAbstractInterfaceMember)
        {
            interfaces.Add(DiscriminatedUnionInterface);
        }

        var sourceWriter = new SourceWriter();
        WriteStart(sourceWriter, keywordUnion.Type);
        sourceWriter
            .Line($"partial union {unionName}{(interfaces.Count > 0 ? " : " + string.Join(ListSeparator, interfaces) : string.Empty)}")
            .OpenBlock();
        WriteFactories(sourceWriter, unionName, keywordUnion.Factories, true);
        if (generatedMembers.HasFlag(GeneratedMembers.Cases))
        {
            WriteCases(sourceWriter, keywordUnion.CaseTypeNames);
        }

        if (generatedMembers.HasFlag(GeneratedMembers.Equality))
        {
            WriteOperators(sourceWriter, unionName);
            WriteEquals(sourceWriter, unionName);
            WriteEqualsObject(sourceWriter, unionName);
            WriteGetHashCode(sourceWriter);
        }

        if (generatedMembers.HasFlag(GeneratedMembers.ToString))
        {
            WriteToString(sourceWriter);
        }

        sourceWriter.CloseBlock();
        WriteEnd(sourceWriter, keywordUnion.Type);
        return sourceWriter.ToString();
    }

    private static void WriteEquals(SourceWriter sourceWriter, string unionName)
    {
        // The value of a union is the case it was created from, boxed when the case is a struct. Comparing the values compares the cases.
        sourceWriter
            .Line("/// <summary>")
            .Line("/// Indicates whether this instance and the other contain equal cases.")
            .Line("/// </summary>")
            .Line("/// <param name=\"other\">The other.</param>")
            .Line("/// <returns><c>true</c>, if this instance and the other are equal, otherwise <c>false</c>.</returns>")
            .Line(GeneratedCodeAttribute)
            .Line(DebuggerNonUserCodeAttribute)
            .Line($"public bool Equals({unionName} other)")
            .OpenBlock()
            .Line("return global::System.Object.Equals(this.Value, other.Value);")
            .CloseBlock()
            .Line();
    }

    private static void WriteGetHashCode(SourceWriter sourceWriter)
    {
        sourceWriter
            .Line("/// <inheritdoc/>")
            .Line(GeneratedCodeAttribute)
            .Line(DebuggerNonUserCodeAttribute)
            .Line("public override int GetHashCode()")
            .OpenBlock()
            .Line("return this.Value?.GetHashCode() ?? 0;")
            .CloseBlock()
            .Line();
    }

    private static void WriteToString(SourceWriter sourceWriter)
    {
        sourceWriter
            .Line("/// <inheritdoc/>")
            .Line(GeneratedCodeAttribute)
            .Line(DebuggerNonUserCodeAttribute)
            .Line("public override string ToString()")
            .OpenBlock()
            .Line("return this.Value?.ToString() ?? string.Empty;")
            .CloseBlock()
            .Line();
    }
}
