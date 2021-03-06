// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PopulateUnionFactoryMethodsMarkerAnalyzer.cs" company="Hukano">
// Copyright (c) Hukano. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Sundew.DiscriminatedUnions.Analyzer;

using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;

/// <summary>
/// Marks all union types with a diagnostic, so that a code fix can be offered to generate factory methods.
/// </summary>
/// <seealso cref="Microsoft.CodeAnalysis.Diagnostics.DiagnosticAnalyzer" />
[DiagnosticAnalyzer(LanguageNames.CSharp)]
public class PopulateUnionFactoryMethodsMarkerAnalyzer : DiagnosticAnalyzer
{
    /// <summary>
    /// Diagnostic id  for populating factory methods in a union.
    /// </summary>
    public const string PopulateFactoryMethodsDiagnosticId = "PDU0001";

    /// <summary>
    /// The switch should throw in default case rule.
    /// </summary>
    public static readonly DiagnosticDescriptor PopulateFactoryMethodsRule =
        DiagnosticDescriptorHelper.Create(
            PopulateFactoryMethodsDiagnosticId,
            nameof(Resources.PopulateFactoryMethodsTitle),
            nameof(Resources.PopulateFactoryMethodsMessageFormat),
            Category,
            DiagnosticSeverity.Error,
            true,
            nameof(Resources.PopulateFactoryMethodsDescription));

    private const string Category = "CodeGeneration";

    /// <summary>
    /// Gets a set of descriptors for the diagnostics that this analyzer is capable of producing.
    /// </summary>
    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => ImmutableArray.Create(PopulateFactoryMethodsRule);

    /// <summary>
    /// Called once at session start to register actions in the analysis context.
    /// </summary>
    /// <param name="context">The context.</param>
    public override void Initialize(AnalysisContext context)
    {
        context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
        context.EnableConcurrentExecution();

        context.RegisterSymbolAction(
            symbolAnalysisContext =>
            {
                if (symbolAnalysisContext.Symbol is not INamedTypeSymbol namedTypeSymbol)
                {
                    return;
                }

                if (UnionHelper.IsDiscriminatedUnion(namedTypeSymbol) &&
                    (namedTypeSymbol.IsAbstract || namedTypeSymbol.TypeKind == TypeKind.Interface))
                {
                    foreach (var location in namedTypeSymbol.Locations)
                    {
                        symbolAnalysisContext.ReportDiagnostic(
                            Diagnostic.Create(
                                PopulateFactoryMethodsRule,
                                location,
                                DiagnosticSeverity.Hidden,
                                null,
                                null,
                                namedTypeSymbol));
                    }
                }
            },
            SymbolKind.NamedType);
    }
}