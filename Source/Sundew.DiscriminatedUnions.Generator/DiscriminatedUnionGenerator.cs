// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DiscriminatedUnionGenerator.cs" company="Sundews">
// Copyright (c) Sundews. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Sundew.DiscriminatedUnions.Generator;

using Microsoft.CodeAnalysis;
using Sundew.DiscriminatedUnions.Generator.ClosedTypes;
using Sundew.DiscriminatedUnions.Generator.DeclarationStage;
using Sundew.DiscriminatedUnions.Generator.KeywordUnions;
using Sundew.DiscriminatedUnions.Generator.ModelStage;
using Sundew.DiscriminatedUnions.Generator.OutputStage;
using Sundew.DiscriminatedUnions.Generator.StructUnions;

/// <summary>
/// Generate assisting functionality around discriminated unions.
/// </summary>
[Generator]
public class DiscriminatedUnionGenerator : IIncrementalGenerator
{
    /// <summary>
    /// Initializes the generator.
    /// </summary>
    /// <param name="context">The context.</param>
    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        var targetFramework = context.AnalyzerConfigOptionsProvider.SetupFeatureSupportStage();

        var discriminatedUnionDeclarationProvider = context.SyntaxProvider.SetupDiscriminatedUnionDeclarationStage();

        var discriminatedUnionCaseProvider = context.SyntaxProvider.SetupDiscriminatedUnionCaseDeclarationStage();

        var unionsAndCasesProvider = discriminatedUnionDeclarationProvider.Collect().Combine(discriminatedUnionCaseProvider.Collect());

        var discriminatedUnionResultsProvider = unionsAndCasesProvider.SetupDiscriminatedUnionStage();

        var outputProvider = discriminatedUnionResultsProvider.Combine(targetFramework);

        context.RegisterSourceOutput(outputProvider, DiscriminatedUnionOutputProvider.Generate);

        var structUnionProvider = context.SyntaxProvider.SetupStructUnionDeclarationStage();

        context.RegisterSourceOutput(structUnionProvider.Combine(targetFramework), StructUnionOutputProvider.Generate);

        var keywordUnionProvider = context.SyntaxProvider.SetupKeywordUnionDeclarationStage();

        context.RegisterSourceOutput(keywordUnionProvider.Combine(targetFramework), KeywordUnionOutputProvider.Generate);

        var closedTypeProvider = context.SyntaxProvider.SetupClosedTypeDeclarationStage();

        context.RegisterSourceOutput(closedTypeProvider.Combine(targetFramework), ClosedTypeOutputProvider.Generate);

        var closedTypeCaseProvider = context.SyntaxProvider.SetupClosedTypeCaseDeclarationStage();

        context.RegisterSourceOutput(closedTypeCaseProvider, ClosedTypeOutputProvider.GenerateCase);
    }
}