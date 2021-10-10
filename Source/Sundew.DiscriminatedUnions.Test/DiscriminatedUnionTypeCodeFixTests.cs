// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DiscriminatedUnionTypeCodeFixTests.cs" company="Hukano">
// Copyright (c) Hukano. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Sundew.DiscriminatedUnions.Test
{
    using System.Threading.Tasks;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Sundew.DiscriminatedUnions.Analyzer;
    using VerifyCS = Sundew.DiscriminatedUnions.Test.CSharpCodeFixVerifier<
    Sundew.DiscriminatedUnions.Analyzer.SundewDiscriminatedUnionsAnalyzer,
    Sundew.DiscriminatedUnions.CodeFixes.SundewDiscriminatedUnionsCodeFixProvider,
    Sundew.DiscriminatedUnions.Analyzer.SundewDiscriminatedUnionSwitchWarningSuppressor>;

    [TestClass]
    public class DiscriminatedUnionTypeCodeFixTests
    {
        [TestMethod]
        public async Task Given_DiscriminatedUnion_When_CaseIsNotDeclaredSealed_Then_CaseShouldBeDeclaredSealed()
        {
            var test = $@"{TestData.Usings}

namespace ConsoleApplication1
{{
    [Sundew.DiscriminatedUnions.DiscriminatedUnion]
    public abstract record Option<T>
    {{
        private Option()
        {{
        }}

        public record Success(T Value) : Option<T>;

        public sealed record None : Option<T>;
    }}
}}";

            var fixtest = $@"{TestData.Usings}

namespace ConsoleApplication1
{{
    [Sundew.DiscriminatedUnions.DiscriminatedUnion]
    public abstract record Option<T>
    {{
        private Option()
        {{
        }}

        public sealed record Success(T Value) : Option<T>;

        public sealed record None : Option<T>;
    }}
}}";

            var expected = new[]
            {
                VerifyCS.Diagnostic(SundewDiscriminatedUnionsAnalyzer.CasesShouldBeSealedDiagnosticId)
                    .WithArguments("ConsoleApplication1.Option<T>.Success")
                    .WithSpan(19, 9, 19, 52),
            };
            await VerifyCS.VerifyCodeFixAsync(test, expected, fixtest);
        }

        [TestMethod]
        public async Task Given_DiscriminatedUnion_When_NoPrivateDefaultConstructorIsDeclared_Then_DefaultConstructorShouldBeDeclaredAfterLastConsecutiveField()
        {
            var test = $@"{TestData.Usings}

namespace ConsoleApplication1
{{
    [Sundew.DiscriminatedUnions.DiscriminatedUnion]
    public abstract record Option<T>
    {{
        public const string ConstField = ""Const"";
        
        public int Field;

        public sealed record Success(T Value) : Option<T>;

        public sealed record None : Option<T>;
    }}
}}";

            var fixtest = $@"{TestData.Usings}

namespace ConsoleApplication1
{{
    [Sundew.DiscriminatedUnions.DiscriminatedUnion]
    public abstract record Option<T>
    {{
        public const string ConstField = ""Const"";

        public int Field;

        private Option()
        {{
        }}

        public sealed record Success(T Value) : Option<T>;

        public sealed record None : Option<T>;
    }}
}}";

            var expected = new[]
            {
                VerifyCS.Diagnostic(SundewDiscriminatedUnionsAnalyzer.MustHavePrivateConstructorRule)
                    .WithArguments("ConsoleApplication1.Option<T>", "ConsoleApplication1.Option<T>.Option()")
                    .WithSpan(12, 5, 22, 6),
            };
            await VerifyCS.VerifyCodeFixAsync(test, expected, fixtest);
        }
    }
}
