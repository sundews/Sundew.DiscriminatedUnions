// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Diagnostics.cs" company="Hukano">
// Copyright (c) Hukano. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Sundew.DiscriminatedUnions.Generator;

using Microsoft.CodeAnalysis;
using Sundew.DiscriminatedUnions.Analyzer;

internal static class Diagnostics
{
    /// <summary>
    /// Diagnostic id indicating that all cases are not handled diagnostic.
    /// </summary>
    public const string DiscriminatedUnionDeclarationNotFoundDiagnosticId = "GDU0001";

    /// <summary>
    /// All cases not handled rule.
    /// </summary>
    public static readonly DiagnosticDescriptor DiscriminatedUnionDeclarationNotFoundRule = DiagnosticDescriptorHelper.Create(
        DiscriminatedUnionDeclarationNotFoundDiagnosticId,
        nameof(Resources.DiscriminatedUnionDeclarationNotFoundTitle),
        nameof(Resources.DiscriminatedUnionDeclarationNotFoundMessageFormat),
        Category,
        DiagnosticSeverity.Error,
        true,
        nameof(Resources.DiscriminatedUnionDeclarationNotFoundDescription));

    private const string Category = "SourceGenerator";
}