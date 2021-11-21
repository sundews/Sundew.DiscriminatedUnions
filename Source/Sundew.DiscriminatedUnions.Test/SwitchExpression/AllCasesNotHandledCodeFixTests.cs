﻿// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AllCasesNotHandledCodeFixTests.cs" company="Hukano">
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
public class AllCasesNotHandledCodeFixTests
{
    [TestMethod]
    public async Task Given_SwitchExpression_When_NullableContextIsEnabledAndMultipleCasesAreNotHandled_Then_RemainingCasesWithoutNullShouldBeHandled()
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
            Result.Warning warning => throw new System.NotImplementedException(),
            Result.Error error => throw new System.NotImplementedException(),
        }};
    }}
}}
{TestData.ValidResultDiscriminatedUnion}
";

        var expected = new[]
        {
            VerifyCS.Diagnostic(DimensionalUnionsAnalyzer.SwitchAllCasesNotHandledRule)
                .WithArguments("'Warning', 'Error'", Resources.Cases, TestData.UnionsResult, Resources.Are)
                .WithSpan(18, 16, 21, 10),
        };
        await VerifyCS.VerifyCodeFixAsync(test, expected, fixtest);
    }

    [TestMethod]
    public async Task Given_SwitchExpression_When_NullableContextIsEnabledAndCasesInBetweenIsNotHandled_Then_RemainingCaseShouldBeHandledInBetween()
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
            Result.Error error => false,
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
            Result.Warning warning => throw new System.NotImplementedException(),
            Result.Error error => false,
        }};
    }}
}}
{TestData.ValidResultDiscriminatedUnion}
";

        var expected = new[]
        {
            VerifyCS.Diagnostic(DimensionalUnionsAnalyzer.SwitchAllCasesNotHandledRule)
                .WithArguments("'Warning'", Resources.Case, TestData.UnionsResult, Resources.Is)
                .WithSpan(18, 16, 22, 10),
        };
        await VerifyCS.VerifyCodeFixAsync(test, expected, fixtest);
    }

    [TestMethod]
    public async Task Given_SwitchExpression_When_NullableContextIsEnabledAndCasesInBetweenHasPattern_Then_RemainingCaseShouldBeHandledInBetweenAfterPattern()
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
            Result.Warning {{ Message: ""Some warning"" }} => false,
            Result.Error error => false,
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
            Result.Success success => throw new System.NotImplementedException(),
            Result.Warning {{ Message: ""Some warning"" }} => false,
            Result.Warning warning => throw new System.NotImplementedException(),
            Result.Error error => false,
        }};
    }}
}}
{TestData.ValidResultDiscriminatedUnion}
";

        var expected = new[]
        {
            VerifyCS.Diagnostic(DimensionalUnionsAnalyzer.SwitchAllCasesNotHandledRule)
                .WithArguments("'Success', 'Warning'", Resources.Cases, TestData.UnionsResult, Resources.Are)
                .WithSpan(18, 16, 22, 10),
        };
        await VerifyCS.VerifyCodeFixAsync(test, expected, fixtest);
    }

    [TestMethod]
    public async Task Given_SwitchExpression_When_NullableContextIsEnabledAndNoCasesAreHandled_Then_AllCaseShouldBeHandled()
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
            Result.Success success => throw new System.NotImplementedException(),
            Result.Warning warning => throw new System.NotImplementedException(),
            Result.Error error => throw new System.NotImplementedException(),
        }};
    }}
}}
{TestData.ValidResultDiscriminatedUnion}
";

        var expected = new[]
        {
            VerifyCS.Diagnostic(DimensionalUnionsAnalyzer.SwitchAllCasesNotHandledRule)
                .WithArguments("'Success', 'Warning', 'Error'", Resources.Cases, TestData.UnionsResult, Resources.Are)
                .WithSpan(18, 16, 20, 10),
        };
        await VerifyCS.VerifyCodeFixAsync(test, expected, fixtest);
    }

    [TestMethod]
    public async Task Given_SwitchExpression_When_NullableContextIsDisableAndMultipleCasesAreNotHandled_Then_RemainingCasesWithNullShouldBeHandled()
    {
        var test = $@"{TestData.Usings}

namespace Unions;

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
";

        var fixtest = $@"{TestData.Usings}

namespace Unions;

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
";

        var expected = new[]
        {
            VerifyCS.Diagnostic(DimensionalUnionsAnalyzer.SwitchAllCasesNotHandledRule)
                .WithArguments("'Warning', 'Error', 'null'", Resources.Cases, TestData.UnionsResult, Resources.Are)
                .WithSpan(17, 16, 20, 10),
        };
        await VerifyCS.VerifyCodeFixAsync(test, expected, fixtest);
    }
}