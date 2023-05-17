// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DiscriminatedUnionDeclarationEqualityFixture.cs" company="Hukano">
// Copyright (c) Hukano. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Sundew.DiscriminatedUnions.Generator.IntegrationTests;

using FluentAssertions;
using NUnit.Framework;
using Sundew.DiscriminatedUnions.Generator.DeclarationStage;

[TestFixture]
public class DiscriminatedUnionDeclarationEqualityFixture
{
    [Test]
    public void TryGetDiscriminatedUnionDeclaration_Then_ResultShouldNotBeEmpty()
    {
        var compilation = TestProjects.SundewDiscriminatedUnionsTester.Compilation;

        var definiteType = compilation.GetNamedTypeSymbolSemanticModelAndSyntaxNode("Sundew.DiscriminatedUnions.Tester.DefiniteType");
        var result = DiscriminatedUnionDeclarationProvider.TryGetDiscriminatedUnionDeclaration(definiteType.SyntaxNode, definiteType.SemanticModel);

        result.Should().NotBeNull();
    }

    [Test]
    public void Equals_Then_ResultShouldBeTrue()
    {
        var compilation = TestProjects.SundewDiscriminatedUnionsTester.Compilation;

        var definiteType = compilation.GetNamedTypeSymbolSemanticModelAndSyntaxNode("Sundew.DiscriminatedUnions.Tester.DefiniteType");
        var lhs = DiscriminatedUnionDeclarationProvider.TryGetDiscriminatedUnionDeclaration(definiteType.SyntaxNode, definiteType.SemanticModel);
        var rhs = DiscriminatedUnionDeclarationProvider.TryGetDiscriminatedUnionDeclaration(definiteType.SyntaxNode, definiteType.SemanticModel);

        lhs.Should().Be(rhs);
    }

    [Test]
    public void Equals_When_ValuesDiffer_Then_ResultShouldBeFalse()
    {
        var compilation = TestProjects.SundewDiscriminatedUnionsTester.Compilation;

        var definiteType = compilation.GetNamedTypeSymbolSemanticModelAndSyntaxNode("Sundew.DiscriminatedUnions.Tester.DefiniteType");
        var lhs = DiscriminatedUnionDeclarationProvider.TryGetDiscriminatedUnionDeclaration(definiteType.SyntaxNode, definiteType.SemanticModel);
        var rhs = DiscriminatedUnionDeclarationProvider.TryGetDiscriminatedUnionDeclaration(definiteType.SyntaxNode, definiteType.SemanticModel).GetValueOrDefault();
        rhs = rhs with { IsPartial = false };

        lhs.Should().NotBe(rhs);
    }
}
