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
    public class SwitchExpressionCodeFixTests
    {
        //Diagnostic and CodeFix both triggered and checked for
        [TestMethod, Ignore]
        public async Task TestMethod2()
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
                        _ => false,
                    }}
            }}
        }}
{TestData.ResultDiscriminatedUnion}
    }}";

            var fixtest = $@"{TestData.Usings}

    namespace ConsoleApplication1
    {{
        public class DiscriminatedUnionTests
        {{   
            public bool Switch(Result result)
            {{
                return result switch
                    {{
                        Success success => true,
                        Warning warning => true,
                        Error error => false,
                        _ => throw new DiscriminatedUnionException(result),
                    }}
            }}
        }}
{TestData.ResultDiscriminatedUnion}
    }}";

            var expected = new[]
            {
                VerifyCS.Diagnostic(SundewDiscriminatedUnionsAnalyzer.AllCasesNotHandledDiagnosticId)
                    .WithArguments("Error")
                    .WithSpan(15, 17, 21, 18),
                VerifyCS.Diagnostic(SundewDiscriminatedUnionsAnalyzer.SwitchShouldThrowInDefaultCaseDiagnosticId)
                    .WithArguments("ConsoleApplication1.Result")
                    .WithSpan(19, 21, 19, 29),
            };
            await VerifyCS.VerifyCodeFixAsync(test, expected, fixtest);
        }
    }
}
