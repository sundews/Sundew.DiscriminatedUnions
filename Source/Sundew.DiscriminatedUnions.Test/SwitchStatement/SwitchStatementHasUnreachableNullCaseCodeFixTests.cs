// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SwitchStatementHasUnreachableNullCaseCodeFixTests.cs" company="Hukano">
// Copyright (c) Hukano. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Sundew.DiscriminatedUnions.Test.SwitchStatement;

using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Sundew.DiscriminatedUnions.Analyzer;
using VerifyCS = Sundew.DiscriminatedUnions.Test.CSharpCodeFixVerifier<
    Sundew.DiscriminatedUnions.Analyzer.DimensionalUnionsAnalyzer,
    Sundew.DiscriminatedUnions.CodeFixes.DimensionalUnionsCodeFixProvider,
    Sundew.DiscriminatedUnions.Analyzer.DimensionalUnionSwitchWarningSuppressor>;

[TestClass]
public class SwitchStatementHasUnreachableNullCaseCodeFixTests
{
    [TestMethod]
    public async Task Given_SwitchStatement_When_ValueCannotBeNullAndNullCaseIsHandled_Then_NullCaseShouldBeRemoved()
    {
        var test = $@"#nullable enable
{TestData.Usings}

namespace Unions;

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
{TestData.ValidResultUnion}
";

        var fixtest = $@"#nullable enable
{TestData.Usings}

namespace Unions;

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
{TestData.ValidResultUnion}
";

        var expected = new[]
        {
            VerifyCS.Diagnostic(DimensionalUnionsAnalyzer.SwitchHasUnreachableNullCaseRule)
                .WithArguments(TestData.UnionsResult)
                .WithSpan(26, 13, 27, 23),
        };
        await VerifyCS.VerifyCodeFixAsync(test, expected, fixtest);
    }
}