// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DiscriminatedUnionGenerator.cs" company="Hukano">
// Copyright (c) Hukano. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Sundew.DiscriminatedUnions.Generator;

using Microsoft.CodeAnalysis;
using Sundew.DiscriminatedUnions.Generator.DeclarationStage;
using Sundew.DiscriminatedUnions.Generator.ModelStage;
using Sundew.DiscriminatedUnions.Generator.OutputStage;

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
        var discriminatedUnionDeclarationProvider = context.SyntaxProvider.SetupDiscriminatedUnionDeclarationStage();

        var discriminatedUnionCaseProvider = context.SyntaxProvider.SetupDiscriminatedUnionCaseDeclarationStage();

        var unionsAndCasesProvider = discriminatedUnionDeclarationProvider.Collect().Combine(discriminatedUnionCaseProvider.Collect());

        var discriminatedUnionResultsProvider = unionsAndCasesProvider.SetupDiscriminatedUnionStage();

        context.RegisterSourceOutput(discriminatedUnionResultsProvider, DiscriminatedUnionOutputProvider.Generate);
    }
}