// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DiscriminatedUnionsCanOnlyHavePrivateProtectedConstructorsAnalyzerTests.cs" company="Hukano">
// Copyright (c) Hukano. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Sundew.DiscriminatedUnions.Test;

using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Sundew.DiscriminatedUnions.Analyzer;
using VerifyCS = Sundew.DiscriminatedUnions.Test.CSharpCodeFixVerifier<
    Sundew.DiscriminatedUnions.Analyzer.SundewDiscriminatedUnionsAnalyzer,
    Sundew.DiscriminatedUnions.CodeFixes.SundewDiscriminatedUnionsCodeFixProvider,
    Sundew.DiscriminatedUnions.Analyzer.SundewDiscriminatedUnionSwitchWarningSuppressor>;

[TestClass]
public class DiscriminatedUnionsCanOnlyHavePrivateProtectedConstructorsAnalyzerTests
{
    [TestMethod]
    public async Task Given_DiscriminatedUnion_When_ConstructorIsNotPrivateProtected_Then_DiscriminatedUnionCanOnlyHavePrivateProtectedConstructorsIsReported()
    {
        var test = $@"#nullable enable
{TestData.Usings}
namespace ConsoleApplication1
{{
    [Sundew.DiscriminatedUnions.DiscriminatedUnion]
    public abstract record Result
    {{
        protected Result()
        {{ }}

        public sealed record Success : Result;

        public sealed record Warning(string Message) : Result;

        public sealed record Error(int Code) : Result;
    }}
}}";

        await VerifyCS.VerifyAnalyzerAsync(
            test,
            VerifyCS.Diagnostic(SundewDiscriminatedUnionsAnalyzer.DiscriminatedUnionsCanOnlyHavePrivateProtectedConstructorsRule)
                .WithArguments(TestData.ConsoleApplication1Result)
                .WithSpan(15, 9, 16, 12));
    }
}