// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SwitchShouldNotHaveDefaultCaseCodeFixTests.cs" company="Hukano">
// Copyright (c) Hukano. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Sundew.DiscriminatedUnions.Test.SwitchExpression;

using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Sundew.DiscriminatedUnions.Analyzer;
using VerifyCS = Sundew.DiscriminatedUnions.Test.CSharpCodeFixVerifier<
    Sundew.DiscriminatedUnions.Analyzer.DimensionalUnionsAnalyzer,
    Sundew.DiscriminatedUnions.CodeFixes.DimensionalUnionsCodeFixProvider,
    Sundew.DiscriminatedUnions.Analyzer.DimensionalUnionSwitchWarningSuppressor>;

[TestClass]
public class SwitchShouldNotHaveDefaultCaseCodeFixTests
{
    [TestMethod]
    public async Task Given_SwitchExpression_When_DefaultCaseIsHandled_Then_DefaultCaseShouldBeRemoved()
    {
        var test = $@"#nullable enable
{TestData.Usings}

namespace Unions;

public class DiscriminatedUnionSymbolAnalyzerTests
{{   
    public bool Switch(Result result)
    {{
        return result switch
        {{
            Result.Success success => true,
            Result.Warning warning => true,
            Result.Error error => false,
            _ => false,
        }};
    }}
}}
{TestData.ValidResultDiscriminatedUnion}
";

        var fixtest = $@"#nullable enable
{TestData.Usings}

namespace Unions;

public class DiscriminatedUnionSymbolAnalyzerTests
{{   
    public bool Switch(Result result)
    {{
        return result switch
        {{
            Result.Success success => true,
            Result.Warning warning => true,
            Result.Error error => false,
        }};
    }}
}}
{TestData.ValidResultDiscriminatedUnion}
";

        var expected = new[]
        {
            VerifyCS.Diagnostic(DimensionalUnionsAnalyzer.SwitchShouldNotHaveDefaultCaseRule)
                .WithArguments(TestData.UnionsResult)
                .WithSpan(23, 13, 23, 23),
        };
        await VerifyCS.VerifyCodeFixAsync(test, expected, fixtest);
    }
}