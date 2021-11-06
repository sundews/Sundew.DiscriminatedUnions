// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SwitchStatementHasUnreachableNullCaseCodeFixTests.cs" company="Hukano">
// Copyright (c) Hukano. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Sundew.DiscriminatedUnions.Test.SwitchStatement
{
    using System.Threading.Tasks;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Sundew.DiscriminatedUnions.Analyzer;
    using VerifyCS = Sundew.DiscriminatedUnions.Test.CSharpCodeFixVerifier<
    Sundew.DiscriminatedUnions.Analyzer.SundewDiscriminatedUnionsAnalyzer,
    Sundew.DiscriminatedUnions.CodeFixes.SundewDiscriminatedUnionsCodeFixProvider,
    Sundew.DiscriminatedUnions.Analyzer.SundewDiscriminatedUnionSwitchWarningSuppressor>;

    [TestClass]
    public class SwitchStatementHasUnreachableNullCaseCodeFixTests
    {
        [TestMethod]
        public async Task Given_SwitchStatement_When_ValueCannotBeNullAndNullCaseIsHandled_Then_NullCaseShouldBeRemoved()
        {
            var test = $@"#nullable enable
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
                    break;
                case null:
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
                    break;
            }}
        }}
    }}
{TestData.ValidResultDiscriminatedUnion}
}}";

            var expected = new[]
            {
                VerifyCS.Diagnostic(SundewDiscriminatedUnionsAnalyzer.SwitchHasUnreachableNullCaseRule)
                    .WithArguments(TestData.ConsoleApplication1Result)
                    .WithSpan(25, 17, 26, 27),
            };
            await VerifyCS.VerifyCodeFixAsync(test, expected, fixtest);
        }
    }
}
