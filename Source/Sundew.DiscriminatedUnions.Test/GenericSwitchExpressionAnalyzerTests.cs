﻿// --------------------------------------------------------------------------------------------------------------------
// <copyright file="GenericSwitchExpressionAnalyzerTests.cs" company="Hukano">
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
    public class GenericSwitchExpressionAnalyzerTests
    {
        [TestMethod]
        public async Task Given_SwitchExpressionInEnabledNullableContext_When_AllCasesAndNullAreHandled_Then_NullCaseShouldNotBeHandledIsReported()
        {
            var test = $@"#nullable enable
{TestData.Usings}

namespace ConsoleApplication1
{{
    public class DiscriminatedUnionSymbolAnalyzerTests
    {{   
        public bool Switch<T>(Option<T> option)
            where T : notnull
        {{
            return option switch
            {{
                Option<T>.Some some => true,
                Option<T>.None => false,
                null => false,
            }};
        }}
    }}
{TestData.ValidGenericOptionDiscriminatedUnion}
}}";

            await VerifyCS.VerifyAnalyzerAsync(
                test,
                VerifyCS.Diagnostic(SundewDiscriminatedUnionsAnalyzer.HasUnreachableNullCaseRule)
                    .WithSpan(18, 20, 23, 14));
        }

        [TestMethod]
        public async Task Given_SwitchExpressionInEnabledNullableContext_When_ValueIsNotNullAndAllCasesAndNullCaseAreHandled_Then_HasUnreachableNullCaseIsReported()
        {
            var test = $@"#nullable enable
{TestData.Usings}

    namespace ConsoleApplication1
    {{
        public class DiscriminatedUnionSymbolAnalyzerTests
        {{   
            public int Switch(Option<int> option)
            {{
                return option switch
                    {{
                        Option<int>.Some some => some.Value,
                        Option<int>.None => 0,
                        null => -1,
                    }};
            }}
        }}
{TestData.ValidGenericOptionDiscriminatedUnion}
    }}";

            await VerifyCS.VerifyAnalyzerAsync(
                test,
                VerifyCS.Diagnostic(SundewDiscriminatedUnionsAnalyzer.HasUnreachableNullCaseRule)
                    .WithSpan(17, 24, 22, 22));
        }

        [TestMethod]
        [Ignore]
        public async Task Given_SwitchExpressionInEnabledNullableContext_When_ValueComeFromAMethodAndIsNotNullAndAllCasesAndNullCaseAreHandled_Then_HasUnreachableNullCaseIsReported()
        {
            var test = $@"#nullable enable
{TestData.Usings}

namespace ConsoleApplication1
{{
    public class DiscriminatedUnionSymbolAnalyzerTests
    {{   
        public int Switch()
        {{
            var option = Compute(""test"");
            return option switch
                {{
                    Option<int>.Some some => some.Value,
                    Option<int>.None => 0,
                    null => -1,
                }};
        }}

        private static Option<int> Compute(string test)
        {{
            if (test == ""test"")
            {{
                return new Option<int>.Some(45);
            }}

            return new Option<int>.None();
        }}
    }}
{TestData.ValidGenericOptionDiscriminatedUnion}
}}";

            await VerifyCS.VerifyAnalyzerAsync(
                test,
                VerifyCS.Diagnostic(SundewDiscriminatedUnionsAnalyzer.HasUnreachableNullCaseRule)
                    .WithSpan(17, 24, 22, 22));
        }

        [TestMethod]
        public async Task Given_SwitchExpressionInDisableNullableContext_When_AllCasesExceptNullAreHandled_Then_AllCasesNotHandledIsReported()
        {
            var test = $@"{TestData.Usings}

namespace ConsoleApplication1
{{
    public class DiscriminatedUnionSymbolAnalyzerTests
    {{   
        public bool Switch<T>(Option<T> option)
            where T : notnull
        {{
            return option switch
            {{
                Option<T>.Some some => true,
                Option<T>.None => false,
            }};
        }}
    }}
{TestData.ValidGenericOptionDiscriminatedUnion}
}}";

            await VerifyCS.VerifyAnalyzerAsync(
                test,
                VerifyCS.Diagnostic(SundewDiscriminatedUnionsAnalyzer.AllCasesNotHandledRule)
                    .WithArguments("'null'", Resources.Case, "ConsoleApplication1.Option<T>", Resources.Is)
                    .WithSpan(17, 20, 21, 14));
        }

        [TestMethod]
        public async Task Given_SwitchExpressionInDisableNullableContext_When_ValueMayBeNullAndAllCasesAndNullCaseAreHandled_Then_NoDiagnosticsAreReported()
        {
            var test = $@"{TestData.Usings}

namespace ConsoleApplication1
{{
    public class DiscriminatedUnionSymbolAnalyzerTests
    {{   
        public int Switch(Option<int> option)
        {{
            return option switch
                {{
                    Option<int>.Some some => some.Value,
                    Option<int>.None => 0,
                    null => -1,
                }};
        }}
    }}
{TestData.ValidGenericOptionDiscriminatedUnion}
}}";

            await VerifyCS.VerifyAnalyzerAsync(test);
        }

        [TestMethod]
        public async Task Given_SwitchExpressionInEnabledNullableContext_When_ValueMayBeNullAndNullIsNotHandled_Then_AllCasesNotHandledIsReported()
        {
            var test = $@"#nullable enable
{TestData.Usings}

    namespace ConsoleApplication1
    {{
        public class DiscriminatedUnionSymbolAnalyzerTests
        {{   
            public bool Switch(Option<int>? option)
            {{
                return option switch
                    {{
                        Option<int>.Some => true,
                        Option<int>.None => false,
                    }};
            }}
        }}
{TestData.ValidGenericOptionDiscriminatedUnion}
    }}";

            await VerifyCS.VerifyAnalyzerAsync(
                test,
                VerifyCS.Diagnostic(SundewDiscriminatedUnionsAnalyzer.AllCasesNotHandledRule)
                    .WithArguments("'null'", Resources.Case, TestData.ConsoleApplication1OptionInt, Resources.Is)
                    .WithSpan(17, 24, 21, 22));
        }
    }
}
