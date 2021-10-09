// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SwitchExpressionCodeFixTests.cs" company="Hukano">
// Copyright (c) Hukano. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Sundew.DiscriminatedUnions.Test
{
    using System;
    using System.Threading.Tasks;
    using Microsoft.CodeAnalysis.Testing;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Sundew.DiscriminatedUnions.Analyzer;
    using VerifyCS = Sundew.DiscriminatedUnions.Test.CSharpCodeFixVerifier<
    Sundew.DiscriminatedUnions.Analyzer.SundewDiscriminatedUnionsAnalyzer,
    Sundew.DiscriminatedUnions.CodeFixes.SundewDiscriminatedUnionsCodeFixProvider,
    Sundew.DiscriminatedUnions.Analyzer.SundewDiscriminatedUnionSwitchWarningSuppressor>;

    [TestClass]
    public class SwitchExpressionCodeFixTests
    {
        [TestMethod]
        [Ignore]
        public async Task TestMethod2()
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
                        Success success => true,
                        _ => false,
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
            public bool Switch(Result result)
            {{
                return result switch
                    {{
                        Success success => true,
                        Warning warning => true,
                        Error error => false,
                    }}
            }}
        }}
{TestData.ValidResultDiscriminatedUnion}
    }}";

            var expected = new[]
            {
                VerifyCS.Diagnostic(SundewDiscriminatedUnionsAnalyzer.AllCasesNotHandledDiagnosticId)
                    .WithArguments("Error")
                    .WithSpan(15, 17, 21, 18),
            };
            await VerifyCS.VerifyCodeFixAsync(test, expected, fixtest);
        }
    }
}
