// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SwitchExpressionAnalyzerTests.cs" company="Hukano">
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
    Sundew.DiscriminatedUnions.Analyzer.SundewDiscriminatedUnionsCodeFixProvider,
    Sundew.DiscriminatedUnions.Analyzer.SundewDiscriminatedUnionSwitchWarningSuppressor>;

    [TestClass]
    public class SwitchExpressionAnalyzerTests
    {
        [TestMethod]
        public async Task Given_NoDiscriminatedUnionSwitch_Then_NoDiagnosticsAreReported()
        {
            var test = string.Empty;

            await VerifyCS.VerifyAnalyzerAsync(test);
        }

        [TestMethod]
        public async Task Given_SwitchExpression_When_ExactlyAllCasesAreHandled_Then_NoDiagnosticsAreReported()
        {
            var test = $@"#nullable enable
{TestData.Usings}

    namespace ConsoleApplication1
    {{
        public class DiscriminatedUnionSymbolAnalyzerTests
        {{   
            public bool Switch(Result result)
            {{
                return result switch
                    {{
                        Result.Success success => true,
                        Result.Warning {{ Message: ""Tough warning"" }} warning => false,
                        Result.Warning warning => true,
                        Result.Error error => false,
                    }};
            }}
        }}
{TestData.ValidResultDiscriminatedUnion}
    }}";

            await VerifyCS.VerifyAnalyzerAsync(test);
        }

        [TestMethod]
        public async Task Given_SwitchExpression_When_ValueMayBeNullAndExactlyAllCasesAreHandled_Then_NoDiagnosticsAreReported()
        {
            var test = $@"{TestData.Usings}

    namespace ConsoleApplication1
    {{
        public class DiscriminatedUnionSymbolAnalyzerTests
        {{   
            public bool Switch(Result result)
            {{
                return result switch
                    {{
                        Result.Success success => true,
                        Result.Warning {{ Message: ""Tough warning"" }} warning => false,
                        Result.Warning warning => true,
                        Result.Error error => false,
                        null => false,
                    }};
            }}
        }}
{TestData.ValidResultDiscriminatedUnion}
    }}";

            await VerifyCS.VerifyAnalyzerAsync(test);
        }

        [TestMethod]
        public async Task Given_SwitchExpression_When_DefaultCaseIsHandled_Then_SwitchShouldNotHaveDefaultCaseIsReported()
        {
            var test = $@"#nullable enable
{TestData.Usings}

    namespace ConsoleApplication1
    {{
        public class DiscriminatedUnionSymbolAnalyzerTests
        {{
            public bool Switch(Result result)
            {{
                return result switch
                    {{
                        Result.Success => true,
                        Result.Warning warning => true,
                        Result.Error error => false,
                        _ => false,
                    }};
            }}
        }}
{TestData.ValidResultDiscriminatedUnion}
    }}";

            await VerifyCS.VerifyAnalyzerAsync(
                test,
                VerifyCS.Diagnostic(SundewDiscriminatedUnionsAnalyzer.SwitchShouldNotHaveDefaultCaseRule)
                    .WithArguments(TestData.ConsoleApplication1Result)
                    .WithSpan(22, 25, 22, 35));
        }

        [TestMethod]
        public async Task
            Given_SwitchExpression_When_DefaultCaseIsHandledAndNotAllCasesAreHandled_Then_AllCasesNotHandledAndSwitchShouldNotHaveDefaultCaseAreReported()
        {
            var test = $@"{TestData.Usings}

    namespace ConsoleApplication1
    {{
        public class DiscriminatedUnionSymbolAnalyzerTests
        {{   
            public bool Switch(Result result)
            {{
                return result switch
                    {{
                        Result.Success => true,
                        _ => false,
                    }};
            }}
        }}
{TestData.ValidResultDiscriminatedUnion}
    }}";

            await VerifyCS.VerifyAnalyzerAsync(
                test,
                VerifyCS.Diagnostic(SundewDiscriminatedUnionsAnalyzer.AllCasesNotHandledRule)
                    .WithArguments("'Error', 'Warning', 'null'", 's', TestData.ConsoleApplication1Result, "are")
                    .WithSpan(16, 24, 20, 22),
                VerifyCS.Diagnostic(SundewDiscriminatedUnionsAnalyzer.SwitchShouldNotHaveDefaultCaseRule)
                    .WithArguments(TestData.ConsoleApplication1Result)
                    .WithSpan(19, 25, 19, 35));
        }

        [TestMethod]
        public async Task Given_SwitchExpression_When_MultipleCasesAreMissing_Then_AllCasesNotHandledIsReported()
        {
            var test = $@"#nullable enable
{TestData.Usings}

    namespace ConsoleApplication1
    {{
        public class DiscriminatedUnionSymbolAnalyzerTests
        {{   
            public bool Switch(Result result)
            {{
                return result switch
                    {{
                        Result.Success => true,
                    }};
            }}
        }}
{TestData.ValidResultDiscriminatedUnion}
    }}";

            await VerifyCS.VerifyAnalyzerAsync(
                test,
                VerifyCS.Diagnostic(SundewDiscriminatedUnionsAnalyzer.AllCasesNotHandledRule)
                    .WithArguments("'Error', 'Warning'", 's', TestData.ConsoleApplication1Result, "are")
                    .WithSpan(17, 24, 20, 22));
        }

        [TestMethod]
        public async Task Given_SwitchExpression_When_ValueMayBeNullAndNullIsNotHandled_Then_AllCasesNotHandledIsReported()
        {
            var test = $@"#nullable enable
{TestData.Usings}

    namespace ConsoleApplication1
    {{
        public class DiscriminatedUnionSymbolAnalyzerTests
        {{   
            public bool Switch(Result? result)
            {{
                return result switch
                    {{
                        Result.Success success => true,
                        Result.Warning {{ Message: ""Tough warning"" }} warning => false,
                        Result.Warning warning => true,
                        Result.Error error => false,
                    }};
            }}
        }}
{TestData.ValidResultDiscriminatedUnion}
    }}";

            await VerifyCS.VerifyAnalyzerAsync(
                test,
                VerifyCS.Diagnostic(SundewDiscriminatedUnionsAnalyzer.AllCasesNotHandledRule)
                    .WithArguments("'null'", string.Empty, TestData.ConsoleApplication1Result, "is")
                    .WithSpan(17, 24, 23, 22));
        }
    }
}
