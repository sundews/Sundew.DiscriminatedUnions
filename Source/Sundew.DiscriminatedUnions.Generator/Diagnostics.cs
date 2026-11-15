// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Diagnostics.cs" company="Sundews">
// Copyright (c) Sundews. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Sundew.DiscriminatedUnions.Generator;

using Microsoft.CodeAnalysis;

internal static class Diagnostics
{
    /// <summary>
    /// Diagnostic id indicating that all cases are not handled diagnostic.
    /// </summary>
    public const string DiscriminatedUnionDeclarationNotFoundDiagnosticId = "GDU0001";

    /// <summary>
    /// Diagnostic id indicating: Struct unions must be partial structs.
    /// </summary>
    public const string StructUnionMustBePartialStructDiagnosticId = "GDU0002";

    /// <summary>
    /// Diagnostic id indicating: A union declaration is the better choice when all cases are reference types.
    /// </summary>
    public const string StructUnionCasesAreAllReferenceTypesDiagnosticId = "GDU0003";

    /// <summary>
    /// Diagnostic id indicating: Struct unions can only have one default case.
    /// </summary>
    public const string StructUnionHasMultipleDefaultCasesDiagnosticId = "GDU0004";

    /// <summary>
    /// Diagnostic id indicating: The default case of a struct union must be a struct without instance fields.
    /// </summary>
    public const string StructUnionDefaultCaseHasPayloadDiagnosticId = "GDU0005";

    /// <summary>
    /// Diagnostic id indicating: A type parameter cannot be a case of a struct union.
    /// </summary>
    public const string StructUnionCaseIsTypeParameterDiagnosticId = "GDU0006";

    /// <summary>
    /// Diagnostic id indicating: A union that is segregated and its cases must be accessible from the namespace of the union.
    /// </summary>
    public const string UnionCannotBeSegregatedDiagnosticId = "GDU0007";

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

    /// <summary>
    /// Struct unions must be partial structs.
    /// </summary>
    public static readonly DiagnosticDescriptor StructUnionMustBePartialStructRule = DiagnosticDescriptorHelper.Create(
        StructUnionMustBePartialStructDiagnosticId,
        nameof(Resources.StructUnionMustBePartialStructTitle),
        nameof(Resources.StructUnionMustBePartialStructMessageFormat),
        Category,
        DiagnosticSeverity.Error,
        true,
        nameof(Resources.StructUnionMustBePartialStructDescription));

    /// <summary>
    /// A union declaration is the better choice when all cases are reference types.
    /// </summary>
    public static readonly DiagnosticDescriptor StructUnionCasesAreAllReferenceTypesRule = DiagnosticDescriptorHelper.Create(
        StructUnionCasesAreAllReferenceTypesDiagnosticId,
        nameof(Resources.StructUnionCasesAreAllReferenceTypesTitle),
        nameof(Resources.StructUnionCasesAreAllReferenceTypesMessageFormat),
        Category,
        DiagnosticSeverity.Error,
        true,
        nameof(Resources.StructUnionCasesAreAllReferenceTypesDescription));

    /// <summary>
    /// Struct unions can only have one default case.
    /// </summary>
    public static readonly DiagnosticDescriptor StructUnionHasMultipleDefaultCasesRule = DiagnosticDescriptorHelper.Create(
        StructUnionHasMultipleDefaultCasesDiagnosticId,
        nameof(Resources.StructUnionHasMultipleDefaultCasesTitle),
        nameof(Resources.StructUnionHasMultipleDefaultCasesMessageFormat),
        Category,
        DiagnosticSeverity.Error,
        true,
        nameof(Resources.StructUnionHasMultipleDefaultCasesDescription));

    /// <summary>
    /// The default case of a struct union must be a struct without instance fields.
    /// </summary>
    public static readonly DiagnosticDescriptor StructUnionDefaultCaseHasPayloadRule = DiagnosticDescriptorHelper.Create(
        StructUnionDefaultCaseHasPayloadDiagnosticId,
        nameof(Resources.StructUnionDefaultCaseHasPayloadTitle),
        nameof(Resources.StructUnionDefaultCaseHasPayloadMessageFormat),
        Category,
        DiagnosticSeverity.Error,
        true,
        nameof(Resources.StructUnionDefaultCaseHasPayloadDescription));

    /// <summary>
    /// A type parameter cannot be a case of a struct union.
    /// </summary>
    public static readonly DiagnosticDescriptor StructUnionCaseIsTypeParameterRule = DiagnosticDescriptorHelper.Create(
        StructUnionCaseIsTypeParameterDiagnosticId,
        nameof(Resources.StructUnionCaseIsTypeParameterTitle),
        nameof(Resources.StructUnionCaseIsTypeParameterMessageFormat),
        Category,
        DiagnosticSeverity.Error,
        true,
        nameof(Resources.StructUnionCaseIsTypeParameterDescription));

    /// <summary>
    /// A union that is segregated and its cases must be accessible from the namespace of the union.
    /// </summary>
    public static readonly DiagnosticDescriptor UnionCannotBeSegregatedRule = DiagnosticDescriptorHelper.Create(
        UnionCannotBeSegregatedDiagnosticId,
        nameof(Resources.UnionCannotBeSegregatedTitle),
        nameof(Resources.UnionCannotBeSegregatedMessageFormat),
        Category,
        DiagnosticSeverity.Warning,
        true,
        nameof(Resources.UnionCannotBeSegregatedDescription));

    private const string Category = "SourceGenerator";
}