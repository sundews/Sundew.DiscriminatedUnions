// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MakePartialCodeFixTests.cs" company="Sundews">
// Copyright (c) Sundews. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Sundew.DiscriminatedUnions.Development.Tests;

using System.Threading.Tasks;
using Sundew.DiscriminatedUnions.Analyzer;
using VerifyCS = Sundew.DiscriminatedUnions.Development.Tests.Verifiers.CSharpCodeFixVerifier<
    Sundew.DiscriminatedUnions.Analyzer.MakePartialMarkerAnalyzer,
    Sundew.DiscriminatedUnions.CodeFixes.DiscriminatedUnionsCodeFixProvider,
    Sundew.DiscriminatedUnions.Analyzer.DiscriminatedUnionSwitchWarningSuppressor>;

public class MakePartialCodeFixTests
{
    [Test]
    public async Task Given_Union_When_ItIsNotDeclaredPartial_Then_ItShouldBeDeclaredPartial()
    {
        var test = $@"{TestData.Usings}

namespace Unions;

[Sundew.DiscriminatedUnions.DiscriminatedUnion]
public abstract record Result
{{
    public sealed record Success : Result;
}}

public sealed record Error : Result;
";

        var fixtest = $@"{TestData.Usings}

namespace Unions;

[Sundew.DiscriminatedUnions.DiscriminatedUnion]
public abstract partial record Result
{{
    public sealed partial record Success : Result;
}}

public sealed partial record Error : Result;
";

        var expected = new[]
        {
            VerifyCS.Diagnostic(MakePartialMarkerAnalyzer.MakePartialRule)
                .WithArguments("Unions.Result")
                .WithSpan(14, 24, 14, 30),
            VerifyCS.Diagnostic(MakePartialMarkerAnalyzer.MakePartialRule)
                .WithArguments("Unions.Result.Success")
                .WithSpan(16, 26, 16, 33),
            VerifyCS.Diagnostic(MakePartialMarkerAnalyzer.MakePartialRule)
                .WithArguments("Unions.Error")
                .WithSpan(19, 22, 19, 27),
        };
        await VerifyCS.VerifyCodeFixAsync(test, expected, fixtest);
    }
}