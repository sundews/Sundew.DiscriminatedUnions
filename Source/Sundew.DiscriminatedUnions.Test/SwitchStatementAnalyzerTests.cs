﻿// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SwitchStatementAnalyzerTests.cs" company="Hukano">
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
    Sundew.DiscriminatedUnions.SundewDiscriminatedUnionsCodeFixProvider>;

    [TestClass]
    public class SwitchStatementAnalyzerTests
    {
        [TestMethod]
        public async Task Given_EmptyCode_Then_NoDiagnosticsAreReported()
        {
            var test = string.Empty;

            await VerifyCS.VerifyAnalyzerAsync(test);
        }

        [TestMethod]
        public async Task Given_NoDiscriminatedUnionSwitch_Then_NoDiagnosticsAreReported()
        {
            var test = $@"{TestData.Usings}

    namespace ConsoleApplication1
    {{
        public class DiscriminatedUnionSymbolAnalyzerTests
        {{   
            public bool Switch(int value)
            {{
                switch (value)
                {{
                    case 0:
                        return true;
                    case 1:
                        return false;
                    case 2:
                        return true;
                    case 3:
                        return false;
                    default:
                        return false;
                }}
            }}
        }}
    }}";

            await VerifyCS.VerifyAnalyzerAsync(test);
        }

        [TestMethod]
        public async Task Given_SwitchStatement_When_AllCasesHandledButDefaultCaseIsIncorrect_Then_SwitchShouldThrowInDefaultCaseIsReported()
        {
            var test = $@"#nullable enable
{TestData.Usings}

    namespace ConsoleApplication1
    {{
        public class DiscriminatedUnionSymbolAnalyzerTests
        {{   
            public bool Switch(Result result)
            {{
                switch (result)
                {{
                    case Success:
                        return true;
                    case Warning {{ Message: ""Tough warning"" }} warning:
                        return false;
                    case Warning warning:
                        return true;
                    case Error error:
                        return false;
                    default:
                        return false;
                }}
            }}
        }}
{TestData.ValidResultDiscriminatedUnion}
    }}";

            await VerifyCS.VerifyAnalyzerAsync(
                test,
                VerifyCS.Diagnostic(SundewDiscriminatedUnionsAnalyzer.SwitchShouldThrowInDefaultCaseDiagnosticId)
                    .WithArguments(TestData.ConsoleApplication1Result)
                    .WithSpan(27, 21, 28, 38));
        }

        [TestMethod]
        public async Task Given_SwitchStatement_When_AllCasesAreHandled_Then_NoDiagnosticsAreReported()
        {
            var test = $@"#nullable enable
{TestData.Usings}

    namespace ConsoleApplication1
    {{
        public class DiscriminatedUnionSymbolAnalyzerTests
        {{   
            public bool Switch(Result result)
            {{
                switch (result)
                {{
                    case Success:
                        return true;
                    case Warning {{ Message: ""Tough warning"" }} warning:
                        return false;
                    case Warning warning:
                        return true;
                    case Error error:
                        return false;
                    default:
                        throw new Sundew.DiscriminatedUnions.UnreachableCaseException(typeof(Result));
                }}
            }}
        }}
{TestData.ValidResultDiscriminatedUnion}
    }}";

            await VerifyCS.VerifyAnalyzerAsync(test);
        }

        [TestMethod]
        public async Task Given_SwitchStatement_When_ExactlyAllCasesAreHandled_Then_NoDiagnosticsAreReported()
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
                    case Success:
                        break;
                    case Warning warning:
                        break;
                    case Error error:
                        break;
                }}
            }}
        }}
{TestData.ValidResultDiscriminatedUnion}
    }}";

            await VerifyCS.VerifyAnalyzerAsync(test);
        }

        [TestMethod]
        public async Task Given_SwitchStatement_When_ValueMayBeNullAndExactlyAllCasesAreHandled_Then_NoDiagnosticsAreReported()
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
                    case Success:
                        break;
                    case Warning warning:
                        break;
                    case Error error:
                        break;
                    case null:
                        break;
                }}
            }}
        }}
{TestData.ValidResultDiscriminatedUnion}
    }}";

            await VerifyCS.VerifyAnalyzerAsync(test);
        }

        /*
        [TestMethod]
        public async Task Given_SwitchStatement_When_DefaultCaseIsHandled_Then_SwitchShouldNotHaveDefaultCaseIsReported()
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
                    case Success:
                        break;
                    case Warning warning:
                        break;
                    case Error error:
                        break;
                    default:
                        break;
                }}
            }}
        }}
{TestData.ValidResultDiscriminatedUnion}
    }}";

            await VerifyCS.VerifyAnalyzerAsync(
                test,
                VerifyCS.Diagnostic(SundewDiscriminatedUnionsAnalyzer.SwitchShouldNotHaveDefaultCaseDiagnosticId)
                    .WithArguments("ConsoleApplication1.Result")
                    .WithSpan(24, 21, 24, 29));
        }*/

        [TestMethod]
        public async Task Given_SwitchStatement_When_DefaultCaseIsHandled_Then_SwitchShouldThrowInDefaultCaseIsReported()
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
                    case Success:
                        break;
                    case Warning warning:
                        break;
                    case Error error:
                        break;
                    default:
                        break;
                }}
            }}
        }}
{TestData.ValidResultDiscriminatedUnion}
    }}";

            await VerifyCS.VerifyAnalyzerAsync(
                test,
                VerifyCS.Diagnostic(SundewDiscriminatedUnionsAnalyzer.SwitchShouldThrowInDefaultCaseDiagnosticId)
                    .WithArguments(TestData.ConsoleApplication1Result)
                    .WithSpan(25, 21, 26, 31));
        }

        [TestMethod]
        public async Task Given_SwitchStatement_When_ValueMayBeNullAndDefaultCaseIsHandled_Then_SwitchShouldThrowInDefaultCaseIsReported()
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
                    case Success:
                        break;
                    case Warning warning:
                        break;
                    case Error error:
                        break;
                    default:
                        break;
                }}
            }}
        }}
{TestData.ValidResultDiscriminatedUnion}
    }}";

            await VerifyCS.VerifyAnalyzerAsync(
                test,
                VerifyCS.Diagnostic(SundewDiscriminatedUnionsAnalyzer.SwitchShouldThrowInDefaultCaseDiagnosticId)
                    .WithArguments(TestData.ConsoleApplication1Result)
                    .WithSpan(24, 21, 25, 31),
                VerifyCS.Diagnostic(SundewDiscriminatedUnionsAnalyzer.AllCasesNotHandledDiagnosticId)
                    .WithArguments("null", string.Empty, TestData.ConsoleApplication1Result, "is")
                    .WithSpan(16, 17, 26, 18));
        }

        [TestMethod]
        public async Task Given_SwitchStatement_When_MultipleCasesAreMissing_Then_AllCasesNotHandledIsReported()
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
                    case Success:
                        break;
                }}
            }}
        }}
{TestData.ValidResultDiscriminatedUnion}
    }}";

            await VerifyCS.VerifyAnalyzerAsync(
                test,
                VerifyCS.Diagnostic(SundewDiscriminatedUnionsAnalyzer.AllCasesNotHandledDiagnosticId)
                    .WithArguments("Error, Warning, null", 's', TestData.ConsoleApplication1Result, "are")
                    .WithSpan(16, 17, 20, 18));
        }

        [TestMethod]
        public async Task
            Given_SwitchStatement_When_DefaultCaseIsHandledAndNotAllCasesAreHandled_Then_AllCasesNotHandledAndSwitchShouldNotHaveDefaultCaseAreReported()
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
                    case Success:
                        break;
                    default:
                        break;
                }}
            }}
        }}
{TestData.ValidResultDiscriminatedUnion}
    }}";

            await VerifyCS.VerifyAnalyzerAsync(
                test,
                VerifyCS.Diagnostic(SundewDiscriminatedUnionsAnalyzer.AllCasesNotHandledDiagnosticId)
                    .WithArguments("Error, Warning, null", 's', TestData.ConsoleApplication1Result, "are")
                    .WithSpan(16, 17, 22, 18),
                VerifyCS.Diagnostic(SundewDiscriminatedUnionsAnalyzer.SwitchShouldThrowInDefaultCaseDiagnosticId)
                    .WithArguments(TestData.ConsoleApplication1Result)
                    .WithSpan(20, 21, 21, 31));
        }

        [TestMethod]
        public async Task Given_SwitchStatement_When_ValueMayBeNullAndAllCasesExceptDefaultCaseAreHandled_Then_SwitchShouldThrowInDefaultCaseIsReported()
        {
            var test = $@"#nullable enable
{TestData.Usings}

    namespace ConsoleApplication1
    {{
        public class DiscriminatedUnionSymbolAnalyzerTests
        {{   
            public bool Switch(Result? result)
            {{
                switch (result)
                {{
                    case Success:
                        return true;
                    case Warning {{ Message: ""Tough warning"" }} warning:
                        return false;
                    case Warning warning:
                        return true;
                    case Error error:
                        return false;
                    case null:
                        return false;
                    default:
                        return false;
                }}
            }}
        }}
{TestData.ValidResultDiscriminatedUnion}
    }}";

            await VerifyCS.VerifyAnalyzerAsync(
                test,
                VerifyCS.Diagnostic(SundewDiscriminatedUnionsAnalyzer.SwitchShouldThrowInDefaultCaseDiagnosticId)
            .WithArguments(TestData.ConsoleApplication1Result)
                .WithSpan(29, 21, 30, 38));
        }
    }
}