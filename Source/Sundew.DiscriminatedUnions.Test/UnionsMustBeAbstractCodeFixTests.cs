// --------------------------------------------------------------------------------------------------------------------
// <copyright file="UnionsMustBeAbstractCodeFixTests.cs" company="Hukano">
// Copyright (c) Hukano. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Sundew.DiscriminatedUnions.Test;

using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Sundew.DiscriminatedUnions.Analyzer;
using VerifyCS = Sundew.DiscriminatedUnions.Test.CSharpCodeFixVerifier<
    Sundew.DiscriminatedUnions.Analyzer.DiscriminatedUnionsAnalyzer,
    Sundew.DiscriminatedUnions.CodeFixes.DiscriminatedUnionsCodeFixProvider,
    Sundew.DiscriminatedUnions.Analyzer.DiscriminatedUnionSwitchWarningSuppressor>;

[TestClass]
public class UnionsMustBeAbstractCodeFixTests
{
    [TestMethod]
    public async Task Given_Union_When_ItIsNotDeclaredAbstract_Then_ItShouldBeDeclaredAbstract()
    {
        var test = $@"{TestData.Usings}

namespace Unions;

[Sundew.DiscriminatedUnions.DiscriminatedUnion]
public record Result
{{
    private Result()
    {{ }}

    public sealed record Success : Result;

    public sealed record Warning(string Message) : Result;

    public sealed record Error(int Code) : Result;
}}
";

        var fixtest = $@"{TestData.Usings}

namespace Unions;

[Sundew.DiscriminatedUnions.DiscriminatedUnion]
public abstract record Result
{{
    private Result()
    {{ }}

    public sealed record Success : Result;

    public sealed record Warning(string Message) : Result;

    public sealed record Error(int Code) : Result;
}}
";

        var expected = new[]
        {
            VerifyCS.Diagnostic(DiscriminatedUnionsAnalyzer.ClassUnionsMustBeAbstractRule)
                .WithArguments("Unions.Result")
                .WithSpan(13, 1, 24, 2),
        };
        await VerifyCS.VerifyCodeFixAsync(test, expected, fixtest);
    }
}