// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DiscriminatedUnionEqualityFixture.cs" company="Sundews">
// Copyright (c) Sundews. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Sundew.DiscriminatedUnions.Generator.Development.IntegrationTests;

using System.Collections.Immutable;
using System.Linq;
using System.Threading;
using AwesomeAssertions;
using Sundew.Base.Collections.Immutable;
using Sundew.DiscriminatedUnions.Generator.DeclarationStage;
using Sundew.DiscriminatedUnions.Generator.ModelStage;
using Accessibility = Sundew.DiscriminatedUnions.Generator.Model.Accessibility;

public class DiscriminatedUnionEqualityFixture
{
    [Test]
    public void GetDiscriminatedUnionResults_Then_ResultShouldNotBeEmpty()
    {
        var compilation = TestProjects.SundewDiscriminatedUnionsTester.Compilation;
        var definiteType = compilation.GetNamedTypeSymbolSemanticModelAndSyntaxNode("Sundew.DiscriminatedUnions.Development.Tester.DefiniteType");
        var namedType = compilation.GetNamedTypeSymbolSemanticModelAndSyntaxNode("Sundew.DiscriminatedUnions.Development.Tester.NamedType");
        var definiteArrayType = compilation.GetNamedTypeSymbolSemanticModelAndSyntaxNode("Sundew.DiscriminatedUnions.Development.Tester.DefiniteArrayType");
        var definiteTypeDeclaration = DiscriminatedUnionDeclarationProvider.TryGetDiscriminatedUnionDeclaration(definiteType.SyntaxNode, definiteType.SemanticModel).GetValueOrDefault();
        var namedTypeCaseDeclaration = DiscriminatedUnionCaseDeclarationProvider.TryGetDiscriminatedUnionCaseDeclaration(namedType.SyntaxNode, namedType.SemanticModel).GetValueOrDefault();
        var definiteArrayTypeCaseDeclaration = DiscriminatedUnionCaseDeclarationProvider.TryGetDiscriminatedUnionCaseDeclaration(definiteArrayType.SyntaxNode, definiteArrayType.SemanticModel).GetValueOrDefault();

        ValueArray<DiscriminatedUnionResult> result = DiscriminatedUnionProvider.GetDiscriminatedUnionResults(ImmutableArray.Create(definiteTypeDeclaration), ImmutableArray.Create(namedTypeCaseDeclaration, definiteArrayTypeCaseDeclaration), CancellationToken.None);

        result.Should().NotBeEmpty();
    }

    [Test]
    public void Equals_Then_ResultShouldBeTrue()
    {
        var compilation = TestProjects.SundewDiscriminatedUnionsTester.Compilation;
        var definiteType = compilation.GetNamedTypeSymbolSemanticModelAndSyntaxNode("Sundew.DiscriminatedUnions.Development.Tester.DefiniteType");
        var namedType = compilation.GetNamedTypeSymbolSemanticModelAndSyntaxNode("Sundew.DiscriminatedUnions.Development.Tester.NamedType");
        var definiteArrayType = compilation.GetNamedTypeSymbolSemanticModelAndSyntaxNode("Sundew.DiscriminatedUnions.Development.Tester.DefiniteArrayType");
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
        var compilation = TestProjects.SundewDiscriminatedUnionsTester.Compilation;
        var definiteType = compilation.GetNamedTypeSymbolSemanticModelAndSyntaxNode("Sundew.DiscriminatedUnions.Development.Tester.DefiniteType");
        var namedType = compilation.GetNamedTypeSymbolSemanticModelAndSyntaxNode("Sundew.DiscriminatedUnions.Development.Tester.NamedType");
        var definiteArrayType = compilation.GetNamedTypeSymbolSemanticModelAndSyntaxNode("Sundew.DiscriminatedUnions.Development.Tester.DefiniteArrayType");
        var definiteTypeDeclaration = DiscriminatedUnionDeclarationProvider.TryGetDiscriminatedUnionDeclaration(definiteType.SyntaxNode, definiteType.SemanticModel).GetValueOrDefault();
        var namedTypeCaseDeclaration = DiscriminatedUnionCaseDeclarationProvider.TryGetDiscriminatedUnionCaseDeclaration(namedType.SyntaxNode, namedType.SemanticModel).GetValueOrDefault();
        var definiteArrayTypeCaseDeclaration = DiscriminatedUnionCaseDeclarationProvider.TryGetDiscriminatedUnionCaseDeclaration(definiteArrayType.SyntaxNode, definiteArrayType.SemanticModel).GetValueOrDefault();

        ValueArray<DiscriminatedUnionResult> lhs = DiscriminatedUnionProvider.GetDiscriminatedUnionResults(ImmutableArray.Create(definiteTypeDeclaration), ImmutableArray.Create(namedTypeCaseDeclaration, definiteArrayTypeCaseDeclaration), CancellationToken.None);
        var item = DiscriminatedUnionProvider.GetDiscriminatedUnionResults(ImmutableArray.Create(definiteTypeDeclaration), ImmutableArray.Create(namedTypeCaseDeclaration, definiteArrayTypeCaseDeclaration), CancellationToken.None).First();
        ValueArray<DiscriminatedUnionResult> rhs = ImmutableArray.Create(item with { DiscriminatedUnion = item.DiscriminatedUnion with { Accessibility = Accessibility.Internal } });

        ((object)lhs).Should().NotBe(rhs);
    }

    [Test]
    public void GetDiscriminatedUnionResults_When__Then_ResultShouldNotBeEmpty()
    {
        var compilation = TestProjects.SundewDiscriminatedUnionsTester.Compilation;

        var scope = compilation.GetNamedTypeSymbolSemanticModelAndSyntaxNode("Sundew.DiscriminatedUnions.Development.Tester.Scope");
        var autoScope = compilation.GetNamedTypeSymbolSemanticModelAndSyntaxNode("Sundew.DiscriminatedUnions.Development.Tester.Scope+AutoScope");
        var singleInstancePerFuncResultScope = compilation.GetNamedTypeSymbolSemanticModelAndSyntaxNode("Sundew.DiscriminatedUnions.Development.Tester.Scope+SingleInstancePerFuncResultScope");
        var scopeDeclaration = DiscriminatedUnionDeclarationProvider.TryGetDiscriminatedUnionDeclaration(scope.SyntaxNode, scope.SemanticModel).GetValueOrDefault();
        var autoScopeCaseDeclaration = DiscriminatedUnionCaseDeclarationProvider.TryGetDiscriminatedUnionCaseDeclaration(autoScope.SyntaxNode, autoScope.SemanticModel).GetValueOrDefault();
        var singleInstancePerFuncResultScopeDeclaration = DiscriminatedUnionCaseDeclarationProvider.TryGetDiscriminatedUnionCaseDeclaration(singleInstancePerFuncResultScope.SyntaxNode, singleInstancePerFuncResultScope.SemanticModel).GetValueOrDefault();

        ValueArray<DiscriminatedUnionResult> result = DiscriminatedUnionProvider.GetDiscriminatedUnionResults(ImmutableArray.Create(scopeDeclaration), ImmutableArray.Create(autoScopeCaseDeclaration, singleInstancePerFuncResultScopeDeclaration), CancellationToken.None);

        result.Should().NotBeEmpty();
    }
}
