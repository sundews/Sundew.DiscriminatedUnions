// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ContradictingTests.cs" company="Hukano">
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
    public class ContradictingTests
    {
        [TestMethod]
        public async Task
            Given_SwitchExpressionInEnabledNullableContext_When_ValueIsNotNullAndAllCasesAndNullCaseAreHandled_Then_HasUnreachableNullCaseIsReported()
        {
            var test = $@"#nullable enable
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using Sundew.DiscriminatedUnions;

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

    [Sundew.DiscriminatedUnions.DiscriminatedUnion]
    public abstract record Option<T>
        where T : notnull
    {{
        private Option()
        {{ }}

        public sealed record Some(T Value) : Option<T>;

        public sealed record None : Option<T>;
    }}
}}";

            await VerifyCS.VerifyAnalyzerAsync(
                test,
                VerifyCS.Diagnostic(SundewDiscriminatedUnionsAnalyzer.HasUnreachableNullCaseRule)
                    .WithSpan(16, 24, 21, 22));
        }

        [TestMethod]
        public async Task
            Given_SwitchExpressionInEnabledNullableContext_When_ValueComeFromAMethodAndIsNotNullAndAllCasesAndNullCaseAreHandled_Then_HasUnreachableNullCaseIsReported()
        {
            var test = $@"#nullable enable
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using Sundew.DiscriminatedUnions;

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

    [Sundew.DiscriminatedUnions.DiscriminatedUnion]
    public abstract record Option<T>
        where T : notnull
    {{
        private Option()
        {{ }}

        public sealed record Some(T Value) : Option<T>;

        public sealed record None : Option<T>;
    }}
}}";

            await VerifyCS.VerifyAnalyzerAsync(
                test,
                VerifyCS.Diagnostic(SundewDiscriminatedUnionsAnalyzer.HasUnreachableNullCaseRule)
                    .WithSpan(17, 20, 22, 18));
        }
    }
}