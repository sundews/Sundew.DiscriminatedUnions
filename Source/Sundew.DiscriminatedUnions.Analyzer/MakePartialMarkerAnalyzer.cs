// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MakePartialMarkerAnalyzer.cs" company="Sundews">
// Copyright (c) Sundews. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Sundew.DiscriminatedUnions.Analyzer;

using System.Collections.Immutable;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using Sundew.DiscriminatedUnions.Shared;

/// <summary>
/// Marks all union types with a diagnostic, so that a code fix can be offered to generate factory methods.
/// </summary>
/// <seealso cref="Microsoft.CodeAnalysis.Diagnostics.DiagnosticAnalyzer" />
[DiagnosticAnalyzer(LanguageNames.CSharp)]
public class MakePartialMarkerAnalyzer : DiagnosticAnalyzer
{
    /// <summary>
    /// Diagnostic id  for a union partial.
    /// </summary>
    public const string MakeUnionPartialDiagnosticId = "PDU0001";

    /// <summary>
    /// The switch should throw in default case rule.
    /// </summary>
    public static readonly DiagnosticDescriptor MakeUnionPartialRule =
        Sundew.DiscriminatedUnions.Analyzer.DiagnosticDescriptorHelper.Create(
            MakeUnionPartialDiagnosticId,
            nameof(Resources.MakeUnionPartialTitle),
            nameof(Resources.MakeUnionPartialMessageFormat),
            Category,
            DiagnosticSeverity.Info,
            true,
            nameof(Resources.MakeUnionPartialDescription));

    private const string Category = "CodeGeneration";

    /// <summary>
    /// Gets a set of descriptors for the diagnostics that this analyzer is capable of producing.
    /// </summary>
    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => ImmutableArray.Create(MakeUnionPartialRule);

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

                if (namedTypeSymbol.IsDiscriminatedUnion() &&
                    (namedTypeSymbol.IsAbstract || namedTypeSymbol.TypeKind == TypeKind.Interface) && !IsPartial(namedTypeSymbol))
                {
                    foreach (var location in namedTypeSymbol.Locations)
                    {
                        symbolAnalysisContext.ReportDiagnostic(
                            Diagnostic.Create(
                                MakeUnionPartialRule,
                                location,
                                DiagnosticSeverity.Info,
                                null,
                                null,
                                namedTypeSymbol));
                    }
                }
            },
            SymbolKind.NamedType);
    }

    private static bool IsPartial(INamedTypeSymbol namedTypeSymbol)
    {
        var syntax = namedTypeSymbol.DeclaringSyntaxReferences.FirstOrDefault()?.GetSyntax();
        if (syntax is TypeDeclarationSyntax typeDeclarationSyntax)
        {
            return typeDeclarationSyntax.Modifiers.Any(m => m.IsKind(SyntaxKind.PartialKeyword));
        }

        return false;
    }
}