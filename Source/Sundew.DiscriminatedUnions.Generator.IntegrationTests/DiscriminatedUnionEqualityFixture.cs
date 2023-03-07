// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DiscriminatedUnionEqualityFixture.cs" company="Hukano">
// Copyright (c) Hukano. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Sundew.DiscriminatedUnions.Generator.IntegrationTests;

using System.Collections.Immutable;
using System.Linq;
using System.Threading;
using FluentAssertions;
using NUnit.Framework;
using Sundew.Base;
using Sundew.CodeAnalysis.Testing;
using Sundew.DiscriminatedUnions.Generator.DeclarationStage;
using Sundew.DiscriminatedUnions.Generator.Model;
using Sundew.DiscriminatedUnions.Generator.ModelStage;

[TestFixture]
public class DiscriminatedUnionEqualityFixture
{
    [Test]
    public void GetDiscriminatedUnionResults_Then_ResultShouldNotBeEmpty()
    {
        var project = new Project(DemoProjectInfo.GetPath("Sundew.DiscriminatedUnions.Tester"), new Paths(DemoProjectInfo.GetPath("Sundew.DiscriminatedUnions")), "bin", "obj");
        var compilation = project.Compile();
        var definiteType = compilation.GetNamedTypeSymbolSemanticModelAndSyntaxNode("Sundew.DiscriminatedUnions.Tester.DefiniteType");
        var namedType = compilation.GetNamedTypeSymbolSemanticModelAndSyntaxNode("Sundew.DiscriminatedUnions.Tester.NamedType");
        var definiteArrayType = compilation.GetNamedTypeSymbolSemanticModelAndSyntaxNode("Sundew.DiscriminatedUnions.Tester.DefiniteArrayType");
        var definiteTypeDeclaration = DiscriminatedUnionDeclarationProvider.TryGetDiscriminatedUnionDeclaration(definiteType.SyntaxNode, definiteType.SemanticModel).GetValueOrDefault();
        var namedTypeCaseDeclaration = DiscriminatedUnionCaseDeclarationProvider.TryGetDiscriminatedUnionCaseDeclaration(namedType.SyntaxNode, namedType.SemanticModel).GetValueOrDefault();
        var definiteArrayTypeCaseDeclaration = DiscriminatedUnionCaseDeclarationProvider.TryGetDiscriminatedUnionCaseDeclaration(definiteArrayType.SyntaxNode, definiteArrayType.SemanticModel).GetValueOrDefault();

        ValueArray<DiscriminatedUnionResult> result = DiscriminatedUnionProvider.GetDiscriminatedUnionResults(ImmutableArray.Create(definiteTypeDeclaration), ImmutableArray.Create(namedTypeCaseDeclaration, definiteArrayTypeCaseDeclaration), CancellationToken.None);

        result.Should().NotBeEmpty();
    }

    [Test]
    public void Equals_Then_ResultShouldBeTrue()
    {
        var project = new Project(DemoProjectInfo.GetPath("Sundew.DiscriminatedUnions.Tester"), new Paths(DemoProjectInfo.GetPath("Sundew.DiscriminatedUnions")), "bin", "obj");
        var compilation = project.Compile();
        var definiteType = compilation.GetNamedTypeSymbolSemanticModelAndSyntaxNode("Sundew.DiscriminatedUnions.Tester.DefiniteType");
        var namedType = compilation.GetNamedTypeSymbolSemanticModelAndSyntaxNode("Sundew.DiscriminatedUnions.Tester.NamedType");
        var definiteArrayType = compilation.GetNamedTypeSymbolSemanticModelAndSyntaxNode("Sundew.DiscriminatedUnions.Tester.DefiniteArrayType");
        var definiteTypeDeclaration = DiscriminatedUnionDeclarationProvider.TryGetDiscriminatedUnionDeclaration(definiteType.SyntaxNode, definiteType.SemanticModel).GetValueOrDefault();
        var namedTypeCaseDeclaration = DiscriminatedUnionCaseDeclarationProvider.TryGetDiscriminatedUnionCaseDeclaration(namedType.SyntaxNode,  namedType.SemanticModel).GetValueOrDefault();
        var definiteArrayTypeCaseDeclaration = DiscriminatedUnionCaseDeclarationProvider.TryGetDiscriminatedUnionCaseDeclaration(definiteArrayType.SyntaxNode, definiteArrayType.SemanticModel).GetValueOrDefault();

        ValueArray<DiscriminatedUnionResult> lhs = DiscriminatedUnionProvider.GetDiscriminatedUnionResults(ImmutableArray.Create(definiteTypeDeclaration), ImmutableArray.Create(namedTypeCaseDeclaration, definiteArrayTypeCaseDeclaration), CancellationToken.None);
        ValueArray<DiscriminatedUnionResult> rhs = DiscriminatedUnionProvider.GetDiscriminatedUnionResults(ImmutableArray.Create(definiteTypeDeclaration), ImmutableArray.Create(namedTypeCaseDeclaration, definiteArrayTypeCaseDeclaration), CancellationToken.None);

        ((object)lhs).Should().Be(rhs);
    }

    [Test]
    public void Equals_When_ValuesDiffer_Then_ResultShouldBeFalse()
    {
        var project = new Project(DemoProjectInfo.GetPath("Sundew.DiscriminatedUnions.Tester"), new Paths(DemoProjectInfo.GetPath("Sundew.DiscriminatedUnions")), "bin", "obj");
        var compilation = project.Compile();
        var definiteType = compilation.GetNamedTypeSymbolSemanticModelAndSyntaxNode("Sundew.DiscriminatedUnions.Tester.DefiniteType");
        var namedType = compilation.GetNamedTypeSymbolSemanticModelAndSyntaxNode("Sundew.DiscriminatedUnions.Tester.NamedType");
        var definiteArrayType = compilation.GetNamedTypeSymbolSemanticModelAndSyntaxNode("Sundew.DiscriminatedUnions.Tester.DefiniteArrayType");
        var definiteTypeDeclaration = DiscriminatedUnionDeclarationProvider.TryGetDiscriminatedUnionDeclaration(definiteType.SyntaxNode, definiteType.SemanticModel).GetValueOrDefault();
        var namedTypeCaseDeclaration = DiscriminatedUnionCaseDeclarationProvider.TryGetDiscriminatedUnionCaseDeclaration(namedType.SyntaxNode, namedType.SemanticModel).GetValueOrDefault();
        var definiteArrayTypeCaseDeclaration = DiscriminatedUnionCaseDeclarationProvider.TryGetDiscriminatedUnionCaseDeclaration(definiteArrayType.SyntaxNode, definiteArrayType.SemanticModel).GetValueOrDefault();

        ValueArray<DiscriminatedUnionResult> lhs = DiscriminatedUnionProvider.GetDiscriminatedUnionResults(ImmutableArray.Create(definiteTypeDeclaration), ImmutableArray.Create(namedTypeCaseDeclaration, definiteArrayTypeCaseDeclaration), CancellationToken.None);
        var item = DiscriminatedUnionProvider.GetDiscriminatedUnionResults(ImmutableArray.Create(definiteTypeDeclaration), ImmutableArray.Create(namedTypeCaseDeclaration, definiteArrayTypeCaseDeclaration), CancellationToken.None).First();
        ValueArray<DiscriminatedUnionResult> rhs = ImmutableArray.Create(item with { DiscriminatedUnion = item.DiscriminatedUnion with { Accessibility = Accessibility.Internal } });

        ((object)lhs).Should().NotBe(rhs);
    }
}
