// --------------------------------------------------------------------------------------------------------------------
// <copyright file="StructUnionOutputProvider.cs" company="Sundews">
// Copyright (c) Sundews. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Sundew.DiscriminatedUnions.Generator.StructUnions;

using System;
using Microsoft.CodeAnalysis;
using Sundew.DiscriminatedUnions.Generator.DeclarationStage;
using Sundew.DiscriminatedUnions.Generator.Unions;

/// <summary>
/// Produces the source for struct unions, or reports why a struct union cannot be generated.
/// </summary>
internal static class StructUnionOutputProvider
{
    /// <summary>
    /// Generates the source for the specified struct union.
    /// </summary>
    /// <param name="sourceProductionContext">The source production context.</param>
    /// <param name="context">The struct union and the features supported by the target framework.</param>
    public static void Generate(SourceProductionContext sourceProductionContext, (StructUnionDeclaration StructUnion, FeatureSupport FeatureSupport) context)
    {
        var structUnion = context.StructUnion;
        if (structUnion.Diagnostics.Count > 0)
        {
            // Every problem is an error and a partly generated union only adds follow-up errors, so nothing is generated.
            var location = structUnion.Location?.ToLocation() ?? Location.None;
            foreach (var structUnionDiagnostic in structUnion.Diagnostics)
            {
                sourceProductionContext.ReportDiagnostic(Diagnostic.Create(GetRule(structUnionDiagnostic.Problem), location, structUnion.Type.Name, structUnionDiagnostic.Argument));
            }

            return;
        }

        sourceProductionContext.AddSource(structUnion.Type.GetHintName(), StructUnionSourceBuilder.Build(structUnion, context.FeatureSupport));
        UnionSegregationOutputProvider.Generate(sourceProductionContext, structUnion.Type, structUnion.Segregation);
    }

    private static DiagnosticDescriptor GetRule(StructUnionProblem structUnionProblem)
    {
        return structUnionProblem switch
        {
            StructUnionProblem.NotPartialStruct => Diagnostics.StructUnionMustBePartialStructRule,
            StructUnionProblem.AllCasesAreReferenceTypes => Diagnostics.StructUnionCasesAreAllReferenceTypesRule,
            StructUnionProblem.MultipleDefaultCases => Diagnostics.StructUnionHasMultipleDefaultCasesRule,
            StructUnionProblem.DefaultCaseHasPayload => Diagnostics.StructUnionDefaultCaseHasPayloadRule,
            StructUnionProblem.TypeParameterCase => Diagnostics.StructUnionCaseIsTypeParameterRule,
            _ => throw new ArgumentOutOfRangeException(nameof(structUnionProblem), structUnionProblem, null),
        };
    }
}
