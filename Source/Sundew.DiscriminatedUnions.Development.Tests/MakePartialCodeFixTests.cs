// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MakePartialCodeFixTests.cs" company="Sundews">
// Copyright (c) Sundews. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Sundew.DiscriminatedUnions.Development.Tests;

using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Sundew.DiscriminatedUnions.Analyzer;
using VerifyCS = Sundew.DiscriminatedUnions.Development.Tests.Verifiers.CSharpCodeFixVerifier<
    Sundew.DiscriminatedUnions.Analyzer.MakePartialMarkerAnalyzer,
    Sundew.DiscriminatedUnions.CodeFixes.DiscriminatedUnionsCodeFixProvider,
    Sundew.DiscriminatedUnions.Analyzer.DiscriminatedUnionSwitchWarningSuppressor>;

[TestClass]
public class MakePartialCodeFixTests
{
    [TestMethod]
    public async Task Given_Union_When_ItIsNotDeclaredPartial_Then_ItShouldBeDeclaredPartial()
    {
        var test = $@"{TestData.Usings}

namespace Unions;

[Sundew.DiscriminatedUnions.DiscriminatedUnion]
public abstract record Result
{{
    public sealed record Success : Result;
}}
";

        var fixtest = $@"{TestData.Usings}

namespace Unions;

[Sundew.DiscriminatedUnions.DiscriminatedUnion]
public abstract partial record Result
{{
    public sealed record Success : Result;
}}
";

        var expected = new[]
        {
            VerifyCS.Diagnostic(MakePartialMarkerAnalyzer.MakeUnionPartialRule)
                .WithArguments("Unions.Result")
                .WithSpan(14, 24, 14, 30),
        };
        await VerifyCS.VerifyCodeFixAsync(test, expected, fixtest);
    }
}