using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis.Testing;
using Sundew.DiscriminatedUnions.Analyzer;
using VerifyCS = Sundew.DiscriminatedUnions.Test.CSharpCodeFixVerifier<
    Sundew.DiscriminatedUnions.Analyzer.SundewDiscriminatedUnionsAnalyzer,
    Sundew.DiscriminatedUnions.SundewDiscriminatedUnionsCodeFixProvider>;

namespace Sundew.DiscriminatedUnions.Test
{
    [TestClass]
    public class SwitchStatementAnalyzerTests
    {
        //No diagnostics expected to show up
        [TestMethod]
        public async Task Given_EmptyCode_Then_NoDiagnosticsAreReported()
        {
            var test = @"";

            await VerifyCS.VerifyAnalyzerAsync(test);
        }

        [TestMethod]
        public async Task Given_NoDiscriminatedUnionSwitch_Then_NoDiagnosticsAreReported()
        {
            var test = $@"{TestData.Usings}

    namespace ConsoleApplication1
    {{
        public class DiscriminatedUnionTests
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
        public class DiscriminatedUnionTests
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
{TestData.ResultDiscriminatedUnion}
    }}";

            await VerifyCS.VerifyAnalyzerAsync(
                test,
                VerifyCS.Diagnostic(SundewDiscriminatedUnionsAnalyzer.SwitchShouldThrowInDefaultCaseDiagnosticId)
                    .WithArguments("ConsoleApplication1.Result")
                    .WithSpan(27, 21, 27, 29));
        }  
        
        [TestMethod]
        public async Task Given_SwitchStatement_When_AllCasesAreHandled_Then_NoDiagnosticsAreReported()
        {
            var test = $@"#nullable enable
{TestData.Usings}

    namespace ConsoleApplication1
    {{
        public class DiscriminatedUnionTests
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
                        throw new Sundew.DiscriminatedUnions.DiscriminatedUnionException(typeof(Result));
                }}
            }}
        }}
{TestData.ResultDiscriminatedUnion}
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
        public class DiscriminatedUnionTests
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
{TestData.ResultDiscriminatedUnion}
    }}";

            await VerifyCS.VerifyAnalyzerAsync(test);
        }

        [TestMethod]
        public async Task Given_SwitchStatement_When_ValueMayBeNullAndExactlyAllCasesAreHandled_Then_NoDiagnosticsAreReported()
        {
            var test = $@"{TestData.Usings}

    namespace ConsoleApplication1
    {{
        public class DiscriminatedUnionTests
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
{TestData.ResultDiscriminatedUnion}
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
        public class DiscriminatedUnionTests
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
{TestData.ResultDiscriminatedUnion}
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
        public class DiscriminatedUnionTests
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
{TestData.ResultDiscriminatedUnion}
    }}";

            await VerifyCS.VerifyAnalyzerAsync(
                test,
                VerifyCS.Diagnostic(SundewDiscriminatedUnionsAnalyzer.SwitchShouldThrowInDefaultCaseDiagnosticId)
                    .WithArguments("ConsoleApplication1.Result")
                    .WithSpan(25, 21, 25, 29));
        }

        [TestMethod]
        public async Task Given_SwitchStatement_When_ValueMayBeNullAndDefaultCaseIsHandled_Then_SwitchShouldThrowInDefaultCaseIsReported()
        {
            var test = $@"{TestData.Usings}

    namespace ConsoleApplication1
    {{
        public class DiscriminatedUnionTests
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
{TestData.ResultDiscriminatedUnion}
    }}";

            await VerifyCS.VerifyAnalyzerAsync(
                test,
                VerifyCS.Diagnostic(SundewDiscriminatedUnionsAnalyzer.SwitchShouldThrowInDefaultCaseDiagnosticId)
                    .WithArguments("ConsoleApplication1.Result")
                    .WithSpan(24, 21, 24, 29),
                VerifyCS.Diagnostic(SundewDiscriminatedUnionsAnalyzer.AllCasesNotHandledDiagnosticId)
                    .WithArguments("null")
                    .WithSpan(16, 17, 26, 18));
        }

        [TestMethod]
        public async Task Given_SwitchStatement_When_MultipleCasesAreMissing_Then_AllCasesNotHandledIsReported()
        {
            var test = $@"{TestData.Usings}

    namespace ConsoleApplication1
    {{
        public class DiscriminatedUnionTests
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
{TestData.ResultDiscriminatedUnion}
    }}";

            await VerifyCS.VerifyAnalyzerAsync(
                test,
                VerifyCS.Diagnostic(SundewDiscriminatedUnionsAnalyzer.AllCasesNotHandledDiagnosticId)
                    .WithArguments("Warning, Error, null")
                    .WithSpan(16, 17, 20, 18));
        }

        [TestMethod]
        public async Task
            Given_SwitchStatement_When_DefaultCaseIsHandledAndNotAllCasesAreHandled_Then_AllCasesNotHandledAndSwitchShouldNotHaveDefaultCaseAreReported()
        {
            var test = $@"{TestData.Usings}

    namespace ConsoleApplication1
    {{
        public class DiscriminatedUnionTests
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
{TestData.ResultDiscriminatedUnion}
    }}";

            await VerifyCS.VerifyAnalyzerAsync(test,
                VerifyCS.Diagnostic(SundewDiscriminatedUnionsAnalyzer.AllCasesNotHandledDiagnosticId)
                    .WithArguments("Warning, Error, null")
                    .WithSpan(16, 17, 22, 18),
                VerifyCS.Diagnostic(SundewDiscriminatedUnionsAnalyzer.SwitchShouldThrowInDefaultCaseDiagnosticId)
                    .WithArguments("ConsoleApplication1.Result")
                    .WithSpan(20, 21, 20, 29));
        }

        [TestMethod]
        public async Task Given_SwitchStatement_When_ValueMayBeNullAndAllCasesExceptDefaultCaseAreHandled_Then_SwitchShouldThrowInDefaultCaseIsReported()
        {
            var test = $@"#nullable enable
{TestData.Usings}

    namespace ConsoleApplication1
    {{
        public class DiscriminatedUnionTests
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
{TestData.ResultDiscriminatedUnion}
    }}";

            await VerifyCS.VerifyAnalyzerAsync(
                test,
                VerifyCS.Diagnostic(SundewDiscriminatedUnionsAnalyzer.SwitchShouldThrowInDefaultCaseDiagnosticId)
            .WithArguments("ConsoleApplication1.Result")
                .WithSpan(29, 21, 29, 29));
        }
    }
}