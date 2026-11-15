// --------------------------------------------------------------------------------------------------------------------
// <copyright file="UnionSegregationOutputProvider.cs" company="Sundews">
// Copyright (c) Sundews. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Sundew.DiscriminatedUnions.Generator.Unions;

using Microsoft.CodeAnalysis;

/// <summary>
/// Produces the sources that segregate a union.
/// </summary>
internal static class UnionSegregationOutputProvider
{
    /// <summary>
    /// Generates the sources that segregate the specified union, if it asks to be segregated, or reports why it cannot be.
    /// </summary>
    /// <param name="sourceProductionContext">The source production context.</param>
    /// <param name="unionType">The union type.</param>
    /// <param name="segregationResult">The outcome for the union with regard to being segregated.</param>
    public static void Generate(SourceProductionContext sourceProductionContext, UnionType unionType, UnionSegregationResult segregationResult)
    {
        if (segregationResult.Problem is { } problem)
        {
            sourceProductionContext.ReportDiagnostic(Diagnostic.Create(Diagnostics.UnionCannotBeSegregatedRule, problem.Location.ToLocation(), unionType.Name, problem.TypeName));
            return;
        }

        if (segregationResult.Segregation is not { } segregation)
        {
            return;
        }

        sourceProductionContext.AddSource(unionType.GetHintName(UnionSegregationSourceBuilder.SegregationSuffix), UnionSegregationSourceBuilder.BuildSegregation(unionType, segregation));
        sourceProductionContext.AddSource(unionType.GetHintName(UnionSegregationSourceBuilder.ExtensionsSuffix), UnionSegregationSourceBuilder.BuildExtensions(unionType, segregation));
    }
}
