// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CompilationHelper.cs" company="Sundews">
// Copyright (c) Sundews. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Sundew.DiscriminatedUnions.Generator.IntegrationTests;

using System.Linq;
using Microsoft.CodeAnalysis;
using NUnit.Framework;

internal static class CompilationHelper
{
    public static (SyntaxNode SyntaxNode, SemanticModel SemanticModel) GetNamedTypeSymbolSemanticModelAndSyntaxNode(this Compilation compilation, string name)
    {
        var namedTypeSymbol = compilation.GetTypeByMetadataName(name);
        if (namedTypeSymbol == null)
        {
            Assert.Fail($"The symbol was not found: {name}");
        }

        var syntaxReference = namedTypeSymbol!.DeclaringSyntaxReferences.First();
        var definitionSemanticModel = compilation.GetSemanticModel(syntaxReference.SyntaxTree, true);

        return (syntaxReference.GetSyntax(), definitionSemanticModel);
    }
}