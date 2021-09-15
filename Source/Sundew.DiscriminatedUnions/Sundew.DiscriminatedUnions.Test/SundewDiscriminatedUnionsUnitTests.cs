using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis.Testing;
using VerifyCS = Sundew.DiscriminatedUnions.Test.CSharpCodeFixVerifier<
    Sundew.DiscriminatedUnions.Analyzer.SundewDiscriminatedUnionsAnalyzer,
    Sundew.DiscriminatedUnions.SundewDiscriminatedUnionsCodeFixProvider>;

namespace Sundew.DiscriminatedUnions.Test
{
    [TestClass]
    public class SundewDiscriminatedUnionsUnitTest
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
            var test = $@"{Usings}

    namespace ConsoleApplication1
    {{
        public class DiscriminatedUnionTests
        {{   
            public bool Switch(Result result)
            {{
                return result switch
                    {{
                        Success success => true,
                        Error error => false,
                    }};
            }}
        }}
{ResultDiscriminatedUnion}
    }}";

            await VerifyCS.VerifyAnalyzerAsync(test);
        }

        [TestMethod]
        public async Task Given_SwitchExpression_When_OneCaseIsMissing_Then_DiagnosticIsReportedForTheMissingCase()
        {
            var test = $@"{Usings}

    namespace ConsoleApplication1
    {{
        public class DiscriminatedUnionTests
        {{   
            public bool Switch(Result result)
            {{
                return result switch
                    {{
                        Success success => true,
                    }};
            }}
        }}
{ResultDiscriminatedUnion}
    }}";

            await VerifyCS.VerifyAnalyzerAsync(test, VerifyCS.Diagnostic().WithArguments("Error not handled in case.").WithSpan(15, 24, 18, 22));
        }

        [TestMethod]
        public async Task Given_SwitchStatement_When_ExactlyAllCasesAreHandled_Then_NoDiagnosticsAreReported()
        {
            var test = $@"{Usings}

    namespace ConsoleApplication1
    {{
        public class DiscriminatedUnionTests
        {{   
            public void Switch(Result result)
            {{
                switch(result)
                {{
                    case Success success:
                        break;
                    case Error error:
                        break;
                }}
            }}
        }}
{ResultDiscriminatedUnion}
    }}";

            await VerifyCS.VerifyAnalyzerAsync(test);
        }

        //Diagnostic and CodeFix both triggered and checked for
        [TestMethod]
        public async Task TestMethod2()
        {
            var test = $@"{Usings}

    namespace ConsoleApplication1
    {{
        class {{|#0:TypeName|}}
        {{   
        }}
    }}";

            var fixtest = $@"{Usings}

    namespace ConsoleApplication1
    {{
        class TYPENAME
        {{   
        }}
    }}";

            var expected = VerifyCS.Diagnostic("SundewDiscriminatedUnions").WithLocation(0).WithArguments("TypeName");
            await VerifyCS.VerifyCodeFixAsync(test, expected, fixtest);
        }

        private const string Usings = @"
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using System.Diagnostics;";

        private const string ResultDiscriminatedUnion = @"
        [Sundew.DiscriminatedUnions.DiscriminatedUnion]
        public abstract class Result
        { }

        public class Success : Result
        {
            public Success(string message)
            {
                this.Message = message;
            }

            public string Message { get; private set; }
        }

        public class Error : Result
        {
            public Error(int code)
            {
                this.Code = code;
            }

            public int Code { get; private set; }
        }";
    }
}
