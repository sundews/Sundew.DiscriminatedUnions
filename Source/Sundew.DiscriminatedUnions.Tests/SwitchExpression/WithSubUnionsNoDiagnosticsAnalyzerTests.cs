﻿// --------------------------------------------------------------------------------------------------------------------
// <copyright file="WithSubUnionsNoDiagnosticsAnalyzerTests.cs" company="Sundews">
// Copyright (c) Sundews. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Sundew.DiscriminatedUnions.Tests.SwitchExpression;

using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Sundew.DiscriminatedUnions.Tests.Verifiers;
using VerifyCS = Sundew.DiscriminatedUnions.Tests.Verifiers.CSharpCodeFixVerifier<
    Sundew.DiscriminatedUnions.Analyzer.DiscriminatedUnionsAnalyzer,
    Sundew.DiscriminatedUnions.CodeFixes.DiscriminatedUnionsCodeFixProvider,
    Sundew.DiscriminatedUnions.Analyzer.DiscriminatedUnionSwitchWarningSuppressor>;

[TestClass]
public class WithSubUnionsNoDiagnosticsAnalyzerTests
{
    [TestMethod]
    public async Task Given_SwitchExpression_When_AllCasesAreHandled_Then_NoDiagnosticsAreReported()
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
                ValueExpression valueExpression => valueExpression.Value,
            }};
    }}
}}

{TestData.ValidMultiUnion}
";
        await VerifyCS.VerifyAnalyzerAsync(test);
    }

    [TestMethod]
    public async Task Given_SwitchExpressionOnSubUnion_When_AllCasesAreHandled_Then_NoDiagnosticsAreReported()
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
                SubtractionExpression subtractionExpression => ""-"",
            }};
    }}
}}

{TestData.ValidMultiUnion}
";
        await VerifyCS.VerifyAnalyzerAsync(test);
    }

    [TestMethod]
    public async Task Given_SwitchExpression_When_AllCasesAreHandledThroughSubUnion_Then_NoDiagnosticsAreReported()
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
                ArithmeticExpression arithmeticExpression => arithmeticExpression.ToString().Length,
                ValueExpression valueExpression => valueExpression.Value,
            }};
    }}
}}

{TestData.ValidMultiUnion}
";
        await VerifyCS.VerifyAnalyzerAsync(test);
    }
}