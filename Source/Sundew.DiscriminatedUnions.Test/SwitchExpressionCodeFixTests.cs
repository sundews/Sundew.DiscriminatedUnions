// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SwitchExpressionCodeFixTests.cs" company="Hukano">
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
    public class SwitchExpressionCodeFixTests
    {
        [TestMethod]
        public async Task Given_SwitchExpression_When_NullableContextIsEnabledAndMultipleCasesAreNotHandled_Then_RemainingCasesWithoutNullShouldBeHandled()
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
            }};
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
        public bool Switch(Result result)
        {{
            return result switch
            {{
                Result.Success success => true,
                Result.Warning warning => throw new System.NotImplementedException(),
                Result.Error error => throw new System.NotImplementedException(),
            }};
        }}
    }}
{TestData.ValidResultDiscriminatedUnion}
}}";

            var expected = new[]
            {
                VerifyCS.Diagnostic(SundewDiscriminatedUnionsAnalyzer.AllCasesNotHandledRule)
                    .WithArguments("'Warning', 'Error'", Resources.Cases, TestData.ConsoleApplication1Result, Resources.Are)
                    .WithSpan(17, 20, 20, 14),
            };
            await VerifyCS.VerifyCodeFixAsync(test, expected, fixtest);
        }

        [TestMethod]
        public async Task Given_SwitchExpression_When_NullableContextIsDisableAndMultipleCasesAreNotHandled_Then_RemainingCasesWithNullShouldBeHandled()
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
            }};
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
                Result.Success success => true,
                Result.Warning warning => throw new System.NotImplementedException(),
                Result.Error error => throw new System.NotImplementedException(),
                null => throw new System.NotImplementedException(),
            }};
        }}
    }}
{TestData.ValidResultDiscriminatedUnion}
}}";

            var expected = new[]
            {
                VerifyCS.Diagnostic(SundewDiscriminatedUnionsAnalyzer.AllCasesNotHandledRule)
                    .WithArguments("'Warning', 'Error', 'null'", Resources.Cases, TestData.ConsoleApplication1Result, Resources.Are)
                    .WithSpan(16, 20, 19, 14),
            };
            await VerifyCS.VerifyCodeFixAsync(test, expected, fixtest);
        }
    }
}
