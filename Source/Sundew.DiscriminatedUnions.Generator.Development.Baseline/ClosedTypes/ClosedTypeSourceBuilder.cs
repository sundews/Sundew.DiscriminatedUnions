// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ClosedTypeSourceBuilder.cs" company="Sundews">
// Copyright (c) Sundews. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Sundew.DiscriminatedUnions.Generator.ClosedTypes;

using Sundew.DiscriminatedUnions.Generator.DeclarationStage;
using Sundew.DiscriminatedUnions.Generator.OutputStage;
using Sundew.DiscriminatedUnions.Generator.Unions;
using static Sundew.DiscriminatedUnions.Generator.Unions.UnionSourceHelper;

/// <summary>
/// Builds the source of the generated part of a partial closed type.
/// The compiler knows the cases of a closed type and checks that a switch handles them all. What it does not give is a way to create a case
/// that is known as the closed type and not as the case, which is what the factories in the generated part are for, nor a list of the cases at runtime.
/// </summary>
internal static class ClosedTypeSourceBuilder
{
    private const string HidesInheritedMember = "CS0108";

    /// <summary>
    /// Builds the source for the specified closed type.
    /// </summary>
    /// <param name="closedType">The closed type.</param>
    /// <param name="featureSupport">The features supported by the target framework.</param>
    /// <returns>The source.</returns>
    public static string Build(ClosedTypeDeclaration closedType, FeatureSupport featureSupport)
    {
        var typeName = closedType.Type.NameWithTypeParameters;
        var generatesCases = closedType.GeneratedMembers.HasFlag(GeneratedMembers.Cases);
        var implementsInterface = generatesCases && featureSupport.StaticsMembersInInterfaceSupport == StaticsMembersInInterfaceSupport.StaticAbstractInterfaceMember;
        var sourceWriter = new SourceWriter();
        WriteStart(sourceWriter, closedType.Type);
        sourceWriter
            .Line($"partial {(closedType.IsRecord ? GeneratorConstants.Record : GeneratorConstants.Class)} {typeName}{(implementsInterface ? " : " + DiscriminatedUnionInterface : string.Empty)}")
            .OpenBlock();
        if (closedType.HidesInheritedMembers)
        {
            // The cases of this type are also cases of the closed type it derives from, which has members of the same names for them.
            // Hiding those is intended: the members here are about this type.
            sourceWriter.Line($"#pragma warning disable {HidesInheritedMember}");
        }

        WriteFactories(sourceWriter, typeName, closedType.Factories, false);
        if (generatesCases)
        {
            WriteCases(sourceWriter, closedType.CaseTypeNames);
        }

        if (closedType.HidesInheritedMembers)
        {
            sourceWriter.Line($"#pragma warning restore {HidesInheritedMember}");
        }

        sourceWriter.CloseBlock();
        WriteEnd(sourceWriter, closedType.Type);
        return sourceWriter.ToString();
    }

    /// <summary>
    /// Builds the source for the specified case of a closed type: the list of cases of the case, which lists the case itself.
    /// </summary>
    /// <param name="closedTypeCase">The case.</param>
    /// <returns>The source.</returns>
    public static string BuildCase(ClosedTypeCaseDeclaration closedTypeCase)
    {
        var sourceWriter = new SourceWriter();
        WriteStart(sourceWriter, closedTypeCase.Type);

        // The case inherits the list of cases of the closed type. Hiding that one is intended: a case is the only case it can be.
        sourceWriter
            .Line($"partial {(closedTypeCase.IsRecord ? GeneratorConstants.Record : GeneratorConstants.Class)} {closedTypeCase.Type.NameWithTypeParameters}")
            .OpenBlock()
            .Line($"#pragma warning disable {HidesInheritedMember}");
        WriteCases(sourceWriter, [closedTypeCase.TypeName]);
        sourceWriter
            .Line($"#pragma warning restore {HidesInheritedMember}")
            .CloseBlock();
        WriteEnd(sourceWriter, closedTypeCase.Type);
        return sourceWriter.ToString();
    }
}
