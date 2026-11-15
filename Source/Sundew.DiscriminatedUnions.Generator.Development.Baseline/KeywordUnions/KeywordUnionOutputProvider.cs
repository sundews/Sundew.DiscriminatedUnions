// --------------------------------------------------------------------------------------------------------------------
// <copyright file="KeywordUnionOutputProvider.cs" company="Sundews">
// Copyright (c) Sundews. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Sundew.DiscriminatedUnions.Generator.KeywordUnions;

using Microsoft.CodeAnalysis;
using Sundew.DiscriminatedUnions.Generator.DeclarationStage;
using Sundew.DiscriminatedUnions.Generator.Unions;

/// <summary>
/// Produces the source for union declarations.
/// </summary>
internal static class KeywordUnionOutputProvider
{
    /// <summary>
    /// Generates the source for the specified union declaration.
    /// </summary>
    /// <param name="sourceProductionContext">The source production context.</param>
    /// <param name="context">The union declaration and the features supported by the target framework.</param>
    public static void Generate(SourceProductionContext sourceProductionContext, (KeywordUnionDeclaration KeywordUnion, FeatureSupport FeatureSupport) context)
    {
        var keywordUnion = context.KeywordUnion;
        if (keywordUnion.HasGeneratedPart)
        {
            sourceProductionContext.AddSource(keywordUnion.Type.GetHintName(), KeywordUnionSourceBuilder.Build(keywordUnion, context.FeatureSupport));
        }

        UnionSegregationOutputProvider.Generate(sourceProductionContext, keywordUnion.Type, keywordUnion.Segregation);
    }
}
