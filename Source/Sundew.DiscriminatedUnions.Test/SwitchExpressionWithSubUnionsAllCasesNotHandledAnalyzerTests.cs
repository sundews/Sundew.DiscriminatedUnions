// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SwitchExpressionWithSubUnionsAllCasesNotHandledAnalyzerTests.cs" company="Hukano">
// Copyright (c) Hukano. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Sundew.DiscriminatedUnions.Test
{
    using System.Threading.Tasks;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using VerifyCS = Sundew.DiscriminatedUnions.Test.CSharpCodeFixVerifier<
        Sundew.DiscriminatedUnions.Analyzer.SundewDiscriminatedUnionsAnalyzer,
        Sundew.DiscriminatedUnions.CodeFixes.SundewDiscriminatedUnionsCodeFixProvider,
        Sundew.DiscriminatedUnions.Analyzer.SundewDiscriminatedUnionSwitchWarningSuppressor>;

    [TestClass]
    public class SwitchExpressionWithSubUnionsAllCasesNotHandledAnalyzerTests
    {
        [TestMethod]
        public async Task T()
        {
            var test = $@"#nullable enable
{TestData.Usings}
namespace ConsoleApplication1
{{
    {TestData.ValidDiscriminatedUnionWithSubUnions}
}}";
            await VerifyCS.VerifyAnalyzerAsync(test);
        }

        [TestMethod]
        public async Task T2()
        {
            var test = $@"#nullable enable
{TestData.Usings}
namespace ConsoleApplication1
{{
    public class DiscriminatedUnionSymbolAnalyzerTests
    {{   
        public int Evaluate(Expression expression)
        {{
            return expression switch
                {{
                    AddExpression addExpression => Evaluate(addExpression.Lhs) + Evaluate(addExpression.Rhs),
                    SubtractExpression subtractExpression => Evaluate(subtractExpression.Lhs) - Evaluate(subtractExpression.Rhs),
                    ValueExpression valueExpression => valueExpression.Value,
                }};
        }}
    }}

    {TestData.ValidDiscriminatedUnionWithSubUnions}
}}";
            await VerifyCS.VerifyAnalyzerAsync(test);
        }
    }
}