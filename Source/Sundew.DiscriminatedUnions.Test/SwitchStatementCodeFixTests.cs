// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SwitchStatementCodeFixTests.cs" company="Hukano">
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
    public class SwitchStatementCodeFixTests
    {
        [TestMethod]
        public async Task Given_SwitchStatement_When_NullableContextIsEnabledAndMultipleCasesAreNotHandled_Then_RemainingCasesWithoutNullShouldBeHandled()
        {
            var test = $@"{TestData.Usings}

namespace ConsoleApplication1
{{
    public class DiscriminatedUnionSymbolAnalyzerTests
    {{   
        public void Switch(Result result)
        {{
            switch(result)
            {{
                case Result.Success success:
                    break;
                default:
                    throw new Sundew.DiscriminatedUnions.UnreachableCaseException(typeof(Result));
            }}
        }}
    }}
{TestData.ValidResultDiscriminatedUnion}
}}";

            var fixtest = $@"{TestData.Usings}

namespace ConsoleApplication1
{{
    public class DiscriminatedUnionSymbolAnalyzerTests
    {{   
        public void Switch(Result result)
        {{
            switch (result)
            {{
                case Result.Success success:
                    break;
                case Result.Warning warning:
                    throw new System.NotImplementedException();
                case Result.Error error:
                    throw new System.NotImplementedException();
                case null:
                    throw new System.NotImplementedException();
                default:
                    throw new Sundew.DiscriminatedUnions.UnreachableCaseException(typeof(Result));
            }}
        }}
    }}
{TestData.ValidResultDiscriminatedUnion}
}}";
            var expected = new[]
            {
                VerifyCS.Diagnostic(SundewDiscriminatedUnionsAnalyzer.AllCasesNotHandledRule)
                    .WithArguments("'Warning', 'Error', 'null'", Resources.Cases, TestData.ConsoleApplication1Result, Resources.Are)
                    .WithSpan(16, 13, 22, 14),
            };

            await VerifyCS.VerifyCodeFixAsync(test, expected, fixtest);
        }

        [TestMethod]
        public async Task Given_SwitchStatement_When_NullableContextIsEnabledAndCasesInBetweenIsNotHandled_Then_RemainingCaseShouldBeHandledInBetween()
        {
            var test = $@"{TestData.Usings}

namespace ConsoleApplication1
{{
    public class DiscriminatedUnionSymbolAnalyzerTests
    {{   
        public void Switch(Result result)
        {{
            switch(result)
            {{
                case Result.Success success:
                    break;
                case Result.Error:
                    break;
                default:
                    throw new Sundew.DiscriminatedUnions.UnreachableCaseException(typeof(Result));
            }}
        }}
    }}
{TestData.ValidResultDiscriminatedUnion}
}}";

            var fixtest = $@"{TestData.Usings}

namespace ConsoleApplication1
{{
    public class DiscriminatedUnionSymbolAnalyzerTests
    {{   
        public void Switch(Result result)
        {{
            switch (result)
            {{
                case Result.Success success:
                    break;
                case Result.Warning warning:
                    throw new System.NotImplementedException();
                case Result.Error:
                    break;
                case null:
                    throw new System.NotImplementedException();
                default:
                    throw new Sundew.DiscriminatedUnions.UnreachableCaseException(typeof(Result));
            }}
        }}
    }}
{TestData.ValidResultDiscriminatedUnion}
}}";
            var expected = new[]
            {
                VerifyCS.Diagnostic(SundewDiscriminatedUnionsAnalyzer.AllCasesNotHandledRule)
                    .WithArguments("'Warning', 'null'", Resources.Cases, TestData.ConsoleApplication1Result, Resources.Are)
                    .WithSpan(16, 13, 24, 14),
            };

            await VerifyCS.VerifyCodeFixAsync(test, expected, fixtest);
        }

        [TestMethod]
        public async Task Given_SwitchStatement_When_NullableContextIsEnabledAndNoCasesAreHandled_Then_AllCaseShouldBeHandled()
        {
            var test = $@"{TestData.Usings}

namespace ConsoleApplication1
{{
    public class DiscriminatedUnionSymbolAnalyzerTests
    {{   
        public void Switch(Result result)
        {{
            switch(result)
            {{
                case Result.Warning warning:
                    break;
                case Result.Error error:
                    break;
                default:
                    throw new Sundew.DiscriminatedUnions.UnreachableCaseException(typeof(Result));
            }}
        }}
    }}
{TestData.ValidResultDiscriminatedUnion}
}}";

            var fixtest = $@"{TestData.Usings}

namespace ConsoleApplication1
{{
    public class DiscriminatedUnionSymbolAnalyzerTests
    {{   
        public void Switch(Result result)
        {{
            switch (result)
            {{
                case Result.Success success:
                    throw new System.NotImplementedException();
                case Result.Warning warning:
                    break;
                case Result.Error error:
                    break;
                case null:
                    throw new System.NotImplementedException();
                default:
                    throw new Sundew.DiscriminatedUnions.UnreachableCaseException(typeof(Result));
            }}
        }}
    }}
{TestData.ValidResultDiscriminatedUnion}
}}";
            var expected = new[]
            {
                VerifyCS.Diagnostic(SundewDiscriminatedUnionsAnalyzer.AllCasesNotHandledRule)
                    .WithArguments("'Success', 'null'", Resources.Cases, TestData.ConsoleApplication1Result, Resources.Are)
                    .WithSpan(16, 13, 24, 14),
            };

            await VerifyCS.VerifyCodeFixAsync(test, expected, fixtest);
        }

        [TestMethod]
        public async Task Given_SwitchStatement_When_NullableContextIsEnabledAndCasesInBetweenHasPattern_Then_RemainingCaseShouldBeHandledInBetweenAfterPattern()
        {
            var test = $@"{TestData.Usings}

namespace ConsoleApplication1
{{
    public class DiscriminatedUnionSymbolAnalyzerTests
    {{   
        public void Switch(Result result)
        {{
            switch(result)
            {{
                default:
                    throw new Sundew.DiscriminatedUnions.UnreachableCaseException(typeof(Result));
            }}
        }}
    }}
{TestData.ValidResultDiscriminatedUnion}
}}";

            var fixtest = $@"{TestData.Usings}

namespace ConsoleApplication1
{{
    public class DiscriminatedUnionSymbolAnalyzerTests
    {{   
        public void Switch(Result result)
        {{
            switch (result)
            {{
                case Result.Success success:
                    throw new System.NotImplementedException();
                case Result.Warning warning:
                    throw new System.NotImplementedException();
                case Result.Error error:
                    throw new System.NotImplementedException();
                case null:
                    throw new System.NotImplementedException();
                default:
                    throw new Sundew.DiscriminatedUnions.UnreachableCaseException(typeof(Result));
            }}
        }}
    }}
{TestData.ValidResultDiscriminatedUnion}
}}";
            var expected = new[]
            {
                VerifyCS.Diagnostic(SundewDiscriminatedUnionsAnalyzer.AllCasesNotHandledRule)
                    .WithArguments("'Success', 'Warning', 'Error', 'null'", Resources.Cases, TestData.ConsoleApplication1Result, Resources.Are)
                    .WithSpan(16, 13, 20, 14),
            };

            await VerifyCS.VerifyCodeFixAsync(test, expected, fixtest);
        }
    }
}
