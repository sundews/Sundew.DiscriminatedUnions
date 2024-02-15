// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DiscriminatedUnionSwitchWarningSuppressor.cs" company="Sundews">
// Copyright (c) Sundews. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Sundew.DiscriminatedUnions.Analyzer;

using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis.Operations;
using Sundew.DiscriminatedUnions.Shared;

/// <summary>
/// Suppressor for CS8509 in switch expressions that are checked by <see cref="DiscriminatedUnionsAnalyzer"/>.
/// </summary>
/// <seealso cref="Microsoft.CodeAnalysis.Diagnostics.DiagnosticSuppressor" />
[DiagnosticAnalyzer(LanguageNames.CSharp)]
public class DiscriminatedUnionSwitchWarningSuppressor : DiagnosticSuppressor
{
    private static readonly SuppressionDescriptor SuppressSwitchExpressionNotExhaustiveForUnion =
        new(
            "SNE0001",
            "CS8509",
            Resources.SuppressCS8509Justification);

    private static readonly SuppressionDescriptor SuppressSwitchExpressionNotExhaustiveForEnumUnion =
        new(
            "SNE0002",
            "CS8524",
            Resources.SuppressCS8509Justification);

    /// <summary>
    /// Gets a set of descriptors for the suppressions that this suppressor is capable of producing.
    /// </summary>
    public override ImmutableArray<SuppressionDescriptor> SupportedSuppressions { get; }
        = ImmutableArray.Create(SuppressSwitchExpressionNotExhaustiveForUnion, SuppressSwitchExpressionNotExhaustiveForEnumUnion);

    /// <summary>
    /// Suppress analyzer and/or compiler non-error diagnostics reported for the compilation.
    /// This may be a subset of the full set of reported diagnostics, as an optimization for
    /// supporting incremental and partial analysis scenarios.
    /// A diagnostic is considered suppressible by a DiagnosticSuppressor if *all* of the following conditions are met:
    /// 1. Diagnostic is not already suppressed in source via pragma/suppress message attribute.
    /// 2. Diagnostic's <see cref="P:Microsoft.CodeAnalysis.Diagnostic.DefaultSeverity" /> is not <see cref="F:Microsoft.CodeAnalysis.DiagnosticSeverity.Error" />.
    /// 3. Diagnostic is not tagged with <see cref="F:Microsoft.CodeAnalysis.WellKnownDiagnosticTags.NotConfigurable" /> custom tag.
    /// </summary>
    /// <param name="context">The context.</param>
    public override void ReportSuppressions(SuppressionAnalysisContext context)
    {
        foreach (var diagnostic in context.ReportedDiagnostics)
        {
            var node = diagnostic.Location.SourceTree?.GetRoot(context.CancellationToken)
                .FindNode(diagnostic.Location.SourceSpan);
            if (node != null)
            {
                var semanticModel = context.GetSemanticModel(node.SyntaxTree);
                var operation = semanticModel.GetOperation(node);
                if (operation is ISwitchExpressionOperation switchExpressionOperation)
                {
                    SuppressIfDiscriminatedUnion(context, switchExpressionOperation.Value.Type, diagnostic);
                }
            }
        }
    }

    private static void SuppressIfDiscriminatedUnion(SuppressionAnalysisContext context, ITypeSymbol? switchType, Diagnostic diagnostic)
    {
        if (switchType is { TypeKind: TypeKind.Enum })
        {
            if (switchType.IsDiscriminatedUnion())
            {
                context.ReportSuppression(
                    Suppression.Create(
                        SuppressSwitchExpressionNotExhaustiveForEnumUnion,
                        diagnostic));
            }
        }
        else
        {
            if (switchType.IsDiscriminatedUnion())
            {
                context.ReportSuppression(
                    Suppression.Create(
                        SuppressSwitchExpressionNotExhaustiveForUnion,
                        diagnostic));
            }
        }
    }
}