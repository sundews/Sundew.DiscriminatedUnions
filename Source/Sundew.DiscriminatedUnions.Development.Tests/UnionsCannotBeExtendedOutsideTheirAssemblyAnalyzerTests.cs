// --------------------------------------------------------------------------------------------------------------------
// <copyright file="UnionsCannotBeExtendedOutsideTheirAssemblyAnalyzerTests.cs" company="Sundews">
// Copyright (c) Sundews. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Sundew.DiscriminatedUnions.Development.Tests;

using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Sundew.DiscriminatedUnions.Analyzer;
using VerifyCS = Sundew.DiscriminatedUnions.Development.Tests.Verifiers.CSharpCodeFixVerifier<
    Sundew.DiscriminatedUnions.Analyzer.DiscriminatedUnionsAnalyzer,
    Sundew.DiscriminatedUnions.CodeFixes.DiscriminatedUnionsCodeFixProvider,
    Sundew.DiscriminatedUnions.Analyzer.DiscriminatedUnionSwitchWarningSuppressor>;

[TestClass]
public class UnionsCannotBeExtendedOutsideTheirAssemblyAnalyzerTests
{
    [TestMethod]
    public async Task Given_Union_When_UnionIsExtendedInDifferentAssembly_Then_UnionsCannotBeExtendedOutsideTheirAssemblyRuleIsReported()
    {
        var test = $@"#nullable enable
{TestData.Usings}
namespace Unions;

[Sundew.DiscriminatedUnions.DiscriminatedUnion]
public interface IExtraExpression : IExpression
{{
}}
";

        await VerifyCS.VerifyAnalyzerAsync(
            test,
            VerifyCS.Diagnostic(DiscriminatedUnionsAnalyzer.UnionsCannotBeExtendedOutsideTheirAssemblyRule)
                .WithArguments("Unions.IExtraExpression", "Sundew.DiscriminatedUnions.Development.TestData")
                .WithSpan(13, 1, 16, 2));
    }
}