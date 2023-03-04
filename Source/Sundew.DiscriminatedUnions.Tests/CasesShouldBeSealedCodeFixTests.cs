// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CasesShouldBeSealedCodeFixTests.cs" company="Hukano">
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
public class CasesShouldBeSealedCodeFixTests
{
    [TestMethod]
    public async Task Given_Union_When_CaseIsNotDeclaredSealed_Then_CaseShouldBeDeclaredSealed()
    {
        var test = $@"{TestData.Usings}

namespace Unions;

[Sundew.DiscriminatedUnions.DiscriminatedUnion]
public abstract record Option<T>
{{
    private Option()
    {{
    }}

    public record Success(T Value) : Option<T>;

    public sealed record None : Option<T>;
}}
";

        var fixtest = $@"{TestData.Usings}

namespace Unions;

[Sundew.DiscriminatedUnions.DiscriminatedUnion]
public abstract record Option<T>
{{
    private Option()
    {{
    }}

    public sealed record Success(T Value) : Option<T>;

    public sealed record None : Option<T>;
}}
";

        var expected = new[]
        {
            VerifyCS.Diagnostic(DiscriminatedUnionsAnalyzer.CasesShouldBeSealedRule)
                .WithArguments("Unions.Option<T>.Success")
                .WithSpan(20, 5, 20, 48),
        };
        await VerifyCS.VerifyCodeFixAsync(test, expected, fixtest);
    }
}