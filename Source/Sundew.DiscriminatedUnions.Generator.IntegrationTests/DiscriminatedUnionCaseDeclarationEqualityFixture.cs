﻿// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DiscriminatedUnionCaseDeclarationEqualityFixture.cs" company="Sundews">
// Copyright (c) Sundews. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Sundew.DiscriminatedUnions.Generator.IntegrationTests;

using FluentAssertions;
using Microsoft.CodeAnalysis;
using NUnit.Framework;
using Sundew.DiscriminatedUnions.Generator.DeclarationStage;
using Sundew.Testing.CodeAnalysis;
using Sundew.Testing.IO;

[TestFixture]
public class DiscriminatedUnionCaseDeclarationEqualityFixture
{
    [Test]
    public void TryGetDiscriminatedUnionCaseDeclaration_Then_ResultShouldNotBeNull()
    {
        var compilation = TestProjects.SundewDiscriminatedUnionsTester.Compilation;

        var namedType = compilation.GetNamedTypeSymbolSemanticModelAndSyntaxNode("Sundew.DiscriminatedUnions.Tester.NamedType");
        var result = DiscriminatedUnionCaseDeclarationProvider.TryGetDiscriminatedUnionCaseDeclaration(namedType.SyntaxNode, namedType.SemanticModel);

        result.Should().NotBeNull();
    }

    [Test]
    public void Equals_Then_ResultShouldBeTrue()
    {
        var compilation = TestProjects.SundewDiscriminatedUnionsTester.Compilation;

        var namedType = compilation.GetNamedTypeSymbolSemanticModelAndSyntaxNode("Sundew.DiscriminatedUnions.Tester.NamedType");
        var lhs = DiscriminatedUnionCaseDeclarationProvider.TryGetDiscriminatedUnionCaseDeclaration(namedType.SyntaxNode, namedType.SemanticModel);
        var rhs = DiscriminatedUnionCaseDeclarationProvider.TryGetDiscriminatedUnionCaseDeclaration(namedType.SyntaxNode, namedType.SemanticModel);

        lhs.Should().Be(rhs);
    }

    [Test]
    public void Equals_When_ValuesDiffer_Then_ResultShouldBeFalse()
    {
        var compilation = TestProjects.SundewDiscriminatedUnionsTester.Compilation;

        var namedType = compilation.GetNamedTypeSymbolSemanticModelAndSyntaxNode("Sundew.DiscriminatedUnions.Tester.NamedType");
        var lhs = DiscriminatedUnionCaseDeclarationProvider.TryGetDiscriminatedUnionCaseDeclaration(namedType.SyntaxNode, namedType.SemanticModel);
        var rhs = DiscriminatedUnionCaseDeclarationProvider.TryGetDiscriminatedUnionCaseDeclaration(namedType.SyntaxNode, namedType.SemanticModel).GetValueOrDefault();
        rhs = rhs with { CaseType = rhs.CaseType with { Name = "Different Name" } };

        lhs.Should().NotBe(rhs);
    }
}
