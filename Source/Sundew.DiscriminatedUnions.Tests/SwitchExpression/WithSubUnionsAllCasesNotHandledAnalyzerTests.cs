﻿// --------------------------------------------------------------------------------------------------------------------
// <copyright file="WithSubUnionsAllCasesNotHandledAnalyzerTests.cs" company="Sundews">
// Copyright (c) Sundews. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Sundew.DiscriminatedUnions.Tests.SwitchExpression;

using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Sundew.DiscriminatedUnions.Analyzer;
using Sundew.DiscriminatedUnions.Tests.Verifiers;
using VerifyCS = Sundew.DiscriminatedUnions.Tests.Verifiers.CSharpCodeFixVerifier<
    Sundew.DiscriminatedUnions.Analyzer.DiscriminatedUnionsAnalyzer,
    Sundew.DiscriminatedUnions.CodeFixes.DiscriminatedUnionsCodeFixProvider,
    Sundew.DiscriminatedUnions.Analyzer.DiscriminatedUnionSwitchWarningSuppressor>;

[TestClass]
public class WithSubUnionsAllCasesNotHandledAnalyzerTests
{
    [TestMethod]
    public async Task Given_SwitchExpression_When_DiscriminatedUnionHasSubUnionAndAllCasesAreNotHandled_Then_AllCasesNotHandledIsReported()
    {
        var test = $@"#nullable enable
{TestData.Usings}
namespace Unions;

public class DiscriminatedUnionSymbolAnalyzerTests
{{   
    public int Evaluate(Expression expression)
    {{
        return expression switch
            {{
                AdditionExpression additionExpression => Evaluate(additionExpression.Lhs) + Evaluate(additionExpression.Rhs),
                SubtractionExpression subtractionExpression => Evaluate(subtractionExpression.Lhs) - Evaluate(subtractionExpression.Rhs),
            }};
    }}
}}

{TestData.ValidMultiUnion}
";
        await VerifyCS.VerifyAnalyzerAsync(
            test,
            VerifyCS.Diagnostic(DiscriminatedUnionsAnalyzer.SwitchAllCasesNotHandledRule)
                .WithArguments("'ValueExpression'", Resources.Case, "Unions.Expression", Resources.Is)
                .WithSpan(17, 16, 21, 14));
    }

    [TestMethod]
    public async Task Given_SwitchExpression_When_DiscriminatedUnionIsSubUnionAndAllCasesAreNotHandled_Then_AllCasesNotHandledIsReported()
    {
        var test = $@"#nullable enable
{TestData.Usings}
namespace Unions;

public class DiscriminatedUnionSymbolAnalyzerTests
{{   
    public string GetOperator(ArithmeticExpression expression)
    {{
        return expression switch
            {{
                AdditionExpression additionExpression => ""+"",
            }};
    }}
}}

{TestData.ValidMultiUnion}
";
        await VerifyCS.VerifyAnalyzerAsync(
            test,
            VerifyCS.Diagnostic(DiscriminatedUnionsAnalyzer.SwitchAllCasesNotHandledRule)
                .WithArguments("'SubtractionExpression'", Resources.Case, "Unions.ArithmeticExpression", Resources.Is)
                .WithSpan(17, 16, 20, 14));
    }
}