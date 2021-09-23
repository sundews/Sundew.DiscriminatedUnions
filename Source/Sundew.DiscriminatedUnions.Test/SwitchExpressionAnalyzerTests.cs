using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Threading.Tasks;
using Sundew.DiscriminatedUnions.Analyzer;
using VerifyCS = Sundew.DiscriminatedUnions.Test.CSharpCodeFixVerifier<
    Sundew.DiscriminatedUnions.Analyzer.SundewDiscriminatedUnionsAnalyzer,
    Sundew.DiscriminatedUnions.SundewDiscriminatedUnionsCodeFixProvider>;

namespace Sundew.DiscriminatedUnions.Test
{
    [TestClass]
    public class SwitchExpressionAnalyzerTests
    {
        //No diagnostics expected to show up
        [TestMethod]
        public async Task Given_NoDiscriminatedUnionSwitch_Then_NoDiagnosticsAreReported()
        {
            var test = @"";

            await VerifyCS.VerifyAnalyzerAsync(test);
        }
        
        [TestMethod]
        public async Task Given_SwitchExpression_When_ExactlyAllCasesAreHandled_Then_NoDiagnosticsAreReported()
        {
            var test = $@"{TestData.Usings}

    namespace ConsoleApplication1
    {{
        public class DiscriminatedUnionTests
        {{   
            public bool Switch(Result result)
            {{
                return result switch
                    {{
                        Success success => true,
                        Warning {{ Message: ""Tough warning"" }} warning => false,
                        Warning warning => true,
                        Error error => false,
                        _ => throw new DiscriminatedUnionException(result),
                    }};
            }}
        }}
{TestData.ResultDiscriminatedUnion}
    }}";

            await VerifyCS.VerifyAnalyzerAsync(test);
        }

        [TestMethod]
        public async Task Given_SwitchExpression_When_AllCasesExceptDefaultCaseAreHandled_Then_SwitchShouldThrowInDefaultCaseIsReported()
        {
            var test = $@"{TestData.Usings}

    namespace ConsoleApplication1
    {{
        public class DiscriminatedUnionTests
        {{   
            public bool Switch(Result result)
            {{
                return result switch
                    {{
                        Success => true,
                        Warning {{ Message: ""Tough warning"" }} warning => false,
                        Warning warning => true,
                        Error error => false,
                    }};
            }}
        }}
{TestData.ResultDiscriminatedUnion}
    }}";

            await VerifyCS.VerifyAnalyzerAsync(
                test,
                VerifyCS.Diagnostic(SundewDiscriminatedUnionsAnalyzer.SwitchShouldThrowInDefaultCaseDiagnosticId)
                    .WithArguments("ConsoleApplication1.Result")
                    .WithSpan(16, 24, 22, 22));
        }
        
        /* Real Discriminated union tests
        [TestMethod]
        public async Task Given_SwitchExpression_When_ExactlyAllCasesAreHandled_Then_NoDiagnosticsAreReported()
        {
            var test = $@"{TestData.Usings}

    namespace ConsoleApplication1
    {{
        public class DiscriminatedUnionTests
        {{   
            public bool Switch(Result result)
            {{
                return result switch
                    {{
                        Success => true,
                        Warning {{ Message: ""Tough warning"" }} warning => false,
                        Warning warning => true,
                        Error error => false,
                    }};
            }}
        }}
{TestData.ResultDiscriminatedUnion}
    }}";

            await VerifyCS.VerifyAnalyzerAsync(test);
        }
 
        
        [TestMethod]
        public async Task Given_SwitchExpression_When_DefaultCaseIsHandled_Then_SwitchShouldNotHaveDefaultCaseIsReported()
        {
            var test = $@"{TestData.Usings}

    namespace ConsoleApplication1
    {{
        public class DiscriminatedUnionTests
        {{   
            public bool Switch(Result result)
            {{
                return result switch
                    {{
                        Success => true,
                        Warning warning => true,
                        Error error => false,
                        _ => false,
                    }};
            }}
        }}
{TestData.ResultDiscriminatedUnion}
    }}";

            await VerifyCS.VerifyAnalyzerAsync(
                test,
                VerifyCS.Diagnostic(SundewDiscriminatedUnionsAnalyzer.SwitchShouldNotHaveDefaultCaseDiagnosticId)
                    .WithArguments("ConsoleApplication1.Result")
                    .WithSpan(19, 25, 19, 26));
        }*/

        [TestMethod]
        public async Task Given_SwitchExpression_When_DefaultCaseIsHandled_Then_SwitchShouldThrowInDefaultCaseIsReported()
        {
            var test = $@"{TestData.Usings}

    namespace ConsoleApplication1
    {{
        public class DiscriminatedUnionTests
        {{   
            public bool Switch(Result result)
            {{
                return result switch
                    {{
                        Success => true,
                        Warning {{ Message: ""Tough warning"" }} warning => false,
                        Warning warning => true,
                        Error error => false,
                        _ => false,
                    }};
            }}
        }}
{TestData.ResultDiscriminatedUnion}
    }}";

            await VerifyCS.VerifyAnalyzerAsync(
                test,
                VerifyCS.Diagnostic(SundewDiscriminatedUnionsAnalyzer.SwitchShouldThrowInDefaultCaseDiagnosticId)
                    .WithArguments("ConsoleApplication1.Result")
                    .WithSpan(22, 25, 22, 26));
        }

        [TestMethod]
        public async Task Given_SwitchExpression_When_DefaultCaseIsHandledAndNotAllCasesAreHandled_Then_AllCasesNotHandledAndSwitchShouldNotHaveDefaultCaseAreReported()
        {
            var test = $@"{TestData.Usings}

    namespace ConsoleApplication1
    {{
        public class DiscriminatedUnionTests
        {{   
            public bool Switch(Result result)
            {{
                return result switch
                    {{
                        Success => true,
                        _ => false,
                    }};
            }}
        }}
{TestData.ResultDiscriminatedUnion}
    }}";

            await VerifyCS.VerifyAnalyzerAsync(test, 
                VerifyCS.Diagnostic(SundewDiscriminatedUnionsAnalyzer.AllCasesNotHandledDiagnosticId)
                    .WithArguments("Warning, Error")
                    .WithSpan(16, 24, 20, 22),
                VerifyCS.Diagnostic(SundewDiscriminatedUnionsAnalyzer.SwitchShouldThrowInDefaultCaseDiagnosticId)
                    .WithArguments("ConsoleApplication1.Result")
                    .WithSpan(19, 25, 19, 26));
        }

        [TestMethod]
        public async Task Given_SwitchExpression_When_MultipleCasesAreMissing_Then_AllCasesNotHandledIsReported()
        {
            var test = $@"{TestData.Usings}

    namespace ConsoleApplication1
    {{
        public class DiscriminatedUnionTests
        {{   
            public bool Switch(Result result)
            {{
                return result switch
                    {{
                        Success => true,
                    }};
            }}
        }}
{TestData.ResultDiscriminatedUnion}
    }}";

            await VerifyCS.VerifyAnalyzerAsync(
                test, 
                VerifyCS.Diagnostic(SundewDiscriminatedUnionsAnalyzer.AllCasesNotHandledDiagnosticId)
                    .WithArguments("Warning, Error")
                    .WithSpan(16, 24, 19, 22),
                VerifyCS.Diagnostic(SundewDiscriminatedUnionsAnalyzer.SwitchShouldThrowInDefaultCaseDiagnosticId)
                    .WithArguments("ConsoleApplication1.Result")
                    .WithSpan(16, 24, 19, 22));
        }
    }
}
