// --------------------------------------------------------------------------------------------------------------------
// <copyright file="StructUnionDefaultValueAnalyzer.cs" company="Sundews">
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
/// Reports where the default value of a struct union is created, when the union does not declare a default case.
/// Such a value contains no case, and the compiler does not require a switch to handle it:
/// a switch that handles every case compiles without a warning and throws at runtime when it is given the default value.
/// </summary>
/// <seealso cref="Microsoft.CodeAnalysis.Diagnostics.DiagnosticAnalyzer" />
[DiagnosticAnalyzer(LanguageNames.CSharp)]
public class StructUnionDefaultValueAnalyzer : DiagnosticAnalyzer
{
    /// <summary>
    /// Diagnostic id indicating that the default value of a struct union without a default case is created.
    /// </summary>
    public const string StructUnionDefaultValueHasNoCaseDiagnosticId = "SDU0013";

    /// <summary>
    /// The struct union default value has no case rule.
    /// </summary>
    public static readonly DiagnosticDescriptor StructUnionDefaultValueHasNoCaseRule =
        Sundew.DiscriminatedUnions.Analyzer.DiagnosticDescriptorHelper.Create(
            StructUnionDefaultValueHasNoCaseDiagnosticId,
            nameof(Resources.StructUnionDefaultValueHasNoCaseTitle),
            nameof(Resources.StructUnionDefaultValueHasNoCaseMessageFormat),
            Category,
            DiagnosticSeverity.Warning,
            true,
            nameof(Resources.StructUnionDefaultValueHasNoCaseDescription));

    private const string Category = "ControlFlow";

    /// <summary>
    /// Gets a set of descriptors for the diagnostics that this analyzer is capable of producing.
    /// </summary>
    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => ImmutableArray.Create(StructUnionDefaultValueHasNoCaseRule);

    /// <summary>
    /// Called once at session start to register actions in the analysis context.
    /// </summary>
    /// <param name="context">The context.</param>
    public override void Initialize(AnalysisContext context)
    {
        context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
        context.EnableConcurrentExecution();
        context.RegisterOperationAction(AnalyzeOperation, OperationKind.DefaultValue, OperationKind.ObjectCreation);
    }

    private static void AnalyzeOperation(OperationAnalysisContext context)
    {
        var operation = context.Operation;
        if (operation.Type is not INamedTypeSymbol { TypeKind: TypeKind.Struct } type
            || (operation is IObjectCreationOperation objectCreationOperation && !IsDefaultConstruction(objectCreationOperation))
            || !IsStructUnionWithoutDefaultCase(type)
            || SymbolEqualityComparer.Default.Equals(context.ContainingSymbol.ContainingType?.OriginalDefinition, type.OriginalDefinition)
            || IsComparedForEquality(operation))
        {
            return;
        }

        context.ReportDiagnostic(Diagnostic.Create(StructUnionDefaultValueHasNoCaseRule, operation.Syntax.GetLocation(), type.Name));
    }

    private static bool IsDefaultConstruction(IObjectCreationOperation objectCreationOperation)
    {
        return objectCreationOperation.Arguments.IsEmpty && objectCreationOperation.Constructor is null or { IsImplicitlyDeclared: true };
    }

    private static bool IsStructUnionWithoutDefaultCase(INamedTypeSymbol type)
    {
        var isStructUnion = false;

        // This runs for every default value in the code, most of which are of types like int that implement dozens of interfaces.
        // So the interfaces are recognized by their names and namespaces, which creates no string for each of them.
        foreach (var interfaceSymbol in type.AllInterfaces)
        {
            if (interfaceSymbol.IsDefaultCaseInterface())
            {
                return false;
            }

            isStructUnion |= interfaceSymbol.IsCaseInterface();
        }

        return isStructUnion;
    }

    private static bool IsComparedForEquality(IOperation operation)
    {
        // Comparing to the default value is how code checks for a union without a case, which is not what the rule is about.
        var parent = operation.Parent;
        while (parent is IConversionOperation or IParenthesizedOperation)
        {
            parent = parent.Parent;
        }

        return parent is IBinaryOperation { OperatorKind: BinaryOperatorKind.Equals or BinaryOperatorKind.NotEquals };
    }
}
