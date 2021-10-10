// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SwitchStatementCodeFixTests.cs" company="Hukano">
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
    public class SwitchStatementCodeFixTests
    {
        [TestMethod]
        public async Task Given_SwitchStatement_WhenNullableContextIsDisableAndMultipleCasesAreMissing_Then_RemainingCasesWithNullShouldBeHandled()
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
    }
}
