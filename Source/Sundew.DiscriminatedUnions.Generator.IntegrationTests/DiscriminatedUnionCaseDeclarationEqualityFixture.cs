// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DiscriminatedUnionCaseDeclarationEqualityFixture.cs" company="Hukano">
// Copyright (c) Hukano. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Sundew.DiscriminatedUnions.Generator.IntegrationTests;

using FluentAssertions;
using NUnit.Framework;
using Sundew.CodeAnalysis.Testing;
using Sundew.DiscriminatedUnions.Generator.DeclarationStage;

[TestFixture]
public class DiscriminatedUnionCaseDeclarationEqualityFixture
{
    [Test]
    public void TryGetDiscriminatedUnionCaseDeclaration_Then_ResultShouldNotBeNull()
    {
        var project = new Project(DemoProjectInfo.GetPath("Sundew.DiscriminatedUnions.Tester"), new Paths(DemoProjectInfo.GetPath("Sundew.DiscriminatedUnions")), "bin", "obj");
        var compilation = project.Compile();
        var namedType = compilation.GetNamedTypeSymbolSemanticModelAndSyntaxNode("Sundew.DiscriminatedUnions.Tester.NamedType");
        var result = DiscriminatedUnionCaseDeclarationProvider.TryGetDiscriminatedUnionCaseDeclaration(namedType.SyntaxNode, namedType.SemanticModel);

        result.Should().NotBeNull();
    }

    [Test]
    public void Equals_Then_ResultShouldBeTrue()
    {
        var project = new Project(DemoProjectInfo.GetPath("Sundew.DiscriminatedUnions.Tester"), new Paths(DemoProjectInfo.GetPath("Sundew.DiscriminatedUnions")), "bin", "obj");
        var compilation = project.Compile();
        var namedType = compilation.GetNamedTypeSymbolSemanticModelAndSyntaxNode("Sundew.DiscriminatedUnions.Tester.NamedType");
        var lhs = DiscriminatedUnionCaseDeclarationProvider.TryGetDiscriminatedUnionCaseDeclaration(namedType.SyntaxNode, namedType.SemanticModel);
        var rhs = DiscriminatedUnionCaseDeclarationProvider.TryGetDiscriminatedUnionCaseDeclaration(namedType.SyntaxNode, namedType.SemanticModel);

        lhs.Should().Be(rhs);
    }

    [Test]
    public void Equals_When_ValuesDiffer_Then_ResultShouldBeFalse()
    {
        var project = new Project(DemoProjectInfo.GetPath("Sundew.DiscriminatedUnions.Tester"), new Paths(DemoProjectInfo.GetPath("Sundew.DiscriminatedUnions")), "bin", "obj");
        var compilation = project.Compile();
        var namedType = compilation.GetNamedTypeSymbolSemanticModelAndSyntaxNode("Sundew.DiscriminatedUnions.Tester.NamedType");
        var lhs = DiscriminatedUnionCaseDeclarationProvider.TryGetDiscriminatedUnionCaseDeclaration(namedType.SyntaxNode, namedType.SemanticModel);
        var rhs = DiscriminatedUnionCaseDeclarationProvider.TryGetDiscriminatedUnionCaseDeclaration(namedType.SyntaxNode, namedType.SemanticModel).GetValueOrDefault();
        rhs = rhs with { CaseType = rhs.CaseType with { Name = "Different Name" } };

        lhs.Should().NotBe(rhs);
    }
}
