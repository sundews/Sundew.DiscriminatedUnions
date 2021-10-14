// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SwitchStatementAllCasesNotHandledCodeFixTests.cs" company="Hukano">
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
    public class SwitchStatementAllCasesNotHandledCodeFixTests
    {
        [TestMethod]
        public async Task Given_SwitchStatement_When_NullableContextIsEnabledAndMultipleCasesAreNotHandled_Then_RemainingCasesWithoutNullShouldBeHandled()
        {
            var test = $@"#nullable enable
{TestData.Usings}

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
            }}
        }}
    }}
{TestData.ValidResultDiscriminatedUnion}
}}";

            var fixtest = $@"#nullable enable
{TestData.Usings}

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
            }}
        }}
    }}
{TestData.ValidResultDiscriminatedUnion}
}}";
            var expected = new[]
            {
                VerifyCS.Diagnostic(SundewDiscriminatedUnionsAnalyzer.AllCasesNotHandledRule)
                    .WithArguments("'Warning', 'Error'", Resources.Cases, TestData.ConsoleApplication1Result, Resources.Are)
                    .WithSpan(17, 13, 21, 14),
            };

            await VerifyCS.VerifyCodeFixAsync(test, expected, fixtest);
        }

        [TestMethod]
        public async Task Given_SwitchStatement_When_NullableContextIsDisabledAndMultipleCasesAreNotHandled_Then_RemainingCasesWithNullShouldBeHandled()
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
            }}
        }}
    }}
{TestData.ValidResultDiscriminatedUnion}
}}";
            var expected = new[]
            {
                VerifyCS.Diagnostic(SundewDiscriminatedUnionsAnalyzer.AllCasesNotHandledRule)
                    .WithArguments("'Warning', 'Error', 'null'", Resources.Cases, TestData.ConsoleApplication1Result, Resources.Are)
                    .WithSpan(16, 13, 20, 14),
            };

            await VerifyCS.VerifyCodeFixAsync(test, expected, fixtest);
        }

        [TestMethod]
        public async Task Given_SwitchStatement_When_NullableContextIsEnabledAndCasesInBetweenIsNotHandled_Then_RemainingCaseShouldBeHandledInBetween()
        {
            var test = $@"#nullable enable
{TestData.Usings}

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
            }}
        }}
    }}
{TestData.ValidResultDiscriminatedUnion}
}}";

            var fixtest = $@"#nullable enable
{TestData.Usings}

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
            }}
        }}
    }}
{TestData.ValidResultDiscriminatedUnion}
}}";
            var expected = new[]
            {
                VerifyCS.Diagnostic(SundewDiscriminatedUnionsAnalyzer.AllCasesNotHandledRule)
                    .WithArguments("'Warning'", Resources.Case, TestData.ConsoleApplication1Result, Resources.Is)
                    .WithSpan(17, 13, 23, 14),
            };

            await VerifyCS.VerifyCodeFixAsync(test, expected, fixtest);
        }

        [TestMethod]
        public async Task Given_SwitchStatement_When_NullableContextIsDisabledAndCasesInBetweenIsNotHandled_Then_RemainingCaseShouldBeHandledInBetween()
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
                case Result.Error error:
                    break;
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
                    break;
                case null:
                    throw new System.NotImplementedException();
            }}
        }}
    }}
{TestData.ValidResultDiscriminatedUnion}
}}";
            var expected = new[]
            {
                VerifyCS.Diagnostic(SundewDiscriminatedUnionsAnalyzer.AllCasesNotHandledRule)
                    .WithArguments("'Warning', 'null'", Resources.Cases, TestData.ConsoleApplication1Result, Resources.Are)
                    .WithSpan(16, 13, 22, 14),
            };

            await VerifyCS.VerifyCodeFixAsync(test, expected, fixtest);
        }

        [TestMethod]
        public async Task Given_SwitchStatement_When_NullableContextIsDisabledAndNoCasesAreHandled_Then_AllCasesAndNullCaseShouldBeHandled()
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

        [TestMethod]
        public async Task Given_SwitchStatement_When_NullableContextIsDisabledAndCasesInBetweenHasPattern_Then_RemainingCaseShouldBeHandledInBetweenAfterPattern()
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
                case Result.Warning {{ Message: ""Tough Warning"" }}:
                    break;
                case Result.Error:
                    break;
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
                case Result.Warning {{ Message: ""Tough Warning"" }}:
                    break;
                case Result.Warning warning:
                    throw new System.NotImplementedException();
                case Result.Error:
                    break;
                case null:
                    throw new System.NotImplementedException();
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
    }
}
