// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ClosedTypeOutputProvider.cs" company="Sundews">
// Copyright (c) Sundews. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Sundew.DiscriminatedUnions.Generator.ClosedTypes;

using Microsoft.CodeAnalysis;
using Sundew.DiscriminatedUnions.Generator.DeclarationStage;
using Sundew.DiscriminatedUnions.Generator.Unions;

/// <summary>
/// Produces the source for closed types and for their cases.
/// </summary>
internal static class ClosedTypeOutputProvider
{
    /// <summary>
    /// Generates the source for the specified closed type.
    /// </summary>
    /// <param name="sourceProductionContext">The source production context.</param>
    /// <param name="context">The closed type and the features supported by the target framework.</param>
    public static void Generate(SourceProductionContext sourceProductionContext, (ClosedTypeDeclaration ClosedType, FeatureSupport FeatureSupport) context)
    {
        var closedType = context.ClosedType;
        if (closedType.HasGeneratedPart)
        {
            sourceProductionContext.AddSource(closedType.Type.GetHintName(), ClosedTypeSourceBuilder.Build(closedType, context.FeatureSupport));
        }

        UnionSegregationOutputProvider.Generate(sourceProductionContext, closedType.Type, closedType.Segregation);
    }

    /// <summary>
    /// Generates the source for the specified case of a closed type.
    /// </summary>
    /// <param name="sourceProductionContext">The source production context.</param>
    /// <param name="closedTypeCase">The case.</param>
    public static void GenerateCase(SourceProductionContext sourceProductionContext, ClosedTypeCaseDeclaration closedTypeCase)
    {
        sourceProductionContext.AddSource(closedTypeCase.Type.GetHintName(), ClosedTypeSourceBuilder.BuildCase(closedTypeCase));
    }
}
