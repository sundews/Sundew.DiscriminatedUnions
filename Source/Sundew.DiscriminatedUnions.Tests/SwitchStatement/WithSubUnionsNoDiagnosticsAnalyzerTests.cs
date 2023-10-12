// --------------------------------------------------------------------------------------------------------------------
// <copyright file="WithSubUnionsNoDiagnosticsAnalyzerTests.cs" company="Sundews">
// Copyright (c) Sundews. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Sundew.DiscriminatedUnions.Tests.SwitchStatement;

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
    public async Task Given_SwitchStatement_When_AllCasesAreHandled_Then_NoDiagnosticsAreReported()
    {
        var test = $@"#nullable enable
{TestData.Usings}
namespace Unions;

public class DiscriminatedUnionSymbolAnalyzerTests
{{   
    public void Evaluate(Expression expression)
    {{
        switch (expression)
        {{
            case AdditionExpression additionExpression:
                break;
            case SubtractionExpression subtractionExpression:
                break;
            case ValueExpression valueExpression:
                break;
        }};
    }}
}}

{TestData.ValidMultiUnion}
";
        await VerifyCS.VerifyAnalyzerAsync(test);
    }

    [TestMethod]
    public async Task Given_SwitchStatementOnSubUnion_When_AllCasesAreHandled_Then_NoDiagnosticsAreReported()
    {
        var test = $@"#nullable enable
{TestData.Usings}
namespace Unions;

public class DiscriminatedUnionSymbolAnalyzerTests
{{ 
    public string GetOperator(ArithmeticExpression expression)
    {{
        switch (expression)
        {{
            case AdditionExpression additionExpression:
                return ""+"";
            case SubtractionExpression subtractionExpression:
                return ""-"";
            default:
                throw new UnreachableCaseException(typeof(ArithmeticExpression));
        }};
    }}
}}

{TestData.ValidMultiUnion}
";
        await VerifyCS.VerifyAnalyzerAsync(test);
    }

    [TestMethod]
    public async Task Given_SwitchStatement_When_AllCasesAreHandledThroughSubUnion_Then_NoDiagnosticsAreReported()
    {
        var test = $@"#nullable enable
{TestData.Usings}
namespace Unions;

public class DiscriminatedUnionSymbolAnalyzerTests
{{   
    public void Evaluate(Expression expression)
    {{
        switch (expression)
        {{
            case ArithmeticExpression arithmeticExpression:
                break;
            case ValueExpression valueExpression:
                break;
        }};
    }}
}}

{TestData.ValidMultiUnion}
";
        await VerifyCS.VerifyAnalyzerAsync(test);
    }
}