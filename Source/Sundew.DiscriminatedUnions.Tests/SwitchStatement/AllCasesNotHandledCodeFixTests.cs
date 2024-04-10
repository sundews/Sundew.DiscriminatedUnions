// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AllCasesNotHandledCodeFixTests.cs" company="Sundews">
// Copyright (c) Sundews. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Sundew.DiscriminatedUnions.Tests.SwitchStatement;

using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Sundew.DiscriminatedUnions.Analyzer;
using VerifyCS = Sundew.DiscriminatedUnions.Tests.Verifiers.CSharpCodeFixVerifier<
    Sundew.DiscriminatedUnions.Analyzer.DiscriminatedUnionsAnalyzer,
    Sundew.DiscriminatedUnions.CodeFixes.DiscriminatedUnionsCodeFixProvider,
    Sundew.DiscriminatedUnions.Analyzer.DiscriminatedUnionSwitchWarningSuppressor>;

[TestClass]
public class AllCasesNotHandledCodeFixTests
{
    [TestMethod]
    public async Task Given_SwitchStatement_When_NullableContextIsEnabledAndMultipleCasesAreNotHandled_Then_RemainingCasesWithoutNullShouldBeHandled()
    {
        var test = $@"#nullable enable
{TestData.Usings}

namespace Unions;

public class DiscriminatedUnionSymbolAnalyzerTests
{{   
    public void Switch(Result result)
    {{
        switch(result)
        {{
            case Result.Success success:
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
                throw new System.NotImplementedException();
        }}
    }}
}}
{TestData.ValidResultUnion}
";
        var expected = new[]
        {
            VerifyCS.Diagnostic(DiscriminatedUnionsAnalyzer.SwitchAllCasesNotHandledRule)
                .WithArguments("'Warning', 'Error'", Resources.Cases, TestData.UnionsResult, Resources.Are)
                .WithSpan(18, 9, 22, 10),
        };
        var expectedAfter = new[]
        {
            VerifyCS.Diagnostic(DiscriminatedUnionsAnalyzer.CaseShouldBeImplementedRule)
                .WithArguments("Warning")
                .WithSpan(23, 17, 23, 60),
            VerifyCS.Diagnostic(DiscriminatedUnionsAnalyzer.CaseShouldBeImplementedRule)
                .WithArguments("Error")
                .WithSpan(25, 17, 25, 60),
        };
        await VerifyCS.VerifyCodeFixAsync(test, expected, fixtest, expectedAfter);
    }

    [TestMethod]
    public async Task Given_SwitchStatement_When_NullableContextIsDisabledAndMultipleCasesAreNotHandled_Then_RemainingCasesWithNullShouldBeHandled()
    {
        var test = $@"{TestData.Usings}

namespace Unions;

public class DiscriminatedUnionSymbolAnalyzerTests
{{   
    public void Switch(Result result)
    {{
        switch(result)
        {{
            case Result.Success success:
                break;
        }}
    }}
}}
{TestData.ValidResultUnion}
";

        var fixtest = $@"{TestData.Usings}

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
                throw new System.NotImplementedException();
            case null:
                throw new System.NotImplementedException();
        }}
    }}
}}
{TestData.ValidResultUnion}
";
        var expected = new[]
        {
            VerifyCS.Diagnostic(DiscriminatedUnionsAnalyzer.SwitchAllCasesNotHandledRule)
                .WithArguments("'Warning', 'Error', 'null'", Resources.Cases, TestData.UnionsResult, Resources.Are)
                .WithSpan(17, 9, 21, 10),
        };
        var expectedAfter = new[]
        {
            VerifyCS.Diagnostic(DiscriminatedUnionsAnalyzer.CaseShouldBeImplementedRule)
                .WithArguments("Warning")
                .WithSpan(22, 17, 22, 60),
            VerifyCS.Diagnostic(DiscriminatedUnionsAnalyzer.CaseShouldBeImplementedRule)
                .WithArguments("Error")
                .WithSpan(24, 17, 24, 60),
            VerifyCS.Diagnostic(DiscriminatedUnionsAnalyzer.CaseShouldBeImplementedRule)
                .WithArguments("null")
                .WithSpan(26, 17, 26, 60),
        };

        await VerifyCS.VerifyCodeFixAsync(test, expected, fixtest, expectedAfter);
    }

    [TestMethod]
    public async Task Given_SwitchStatement_When_NullableContextIsEnabledAndCasesInBetweenIsNotHandled_Then_RemainingCaseShouldBeHandledInBetween()
    {
        var test = $@"#nullable enable
{TestData.Usings}

namespace Unions;

public class DiscriminatedUnionSymbolAnalyzerTests
{{   
    public void Switch(Result result)
    {{
        switch(result)
        {{
            case Result.Success success:
                break;
            case Result.Error:
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
            case Result.Error:
                break;
        }}
    }}
}}
{TestData.ValidResultUnion}
";
        var expected = new[]
        {
            VerifyCS.Diagnostic(DiscriminatedUnionsAnalyzer.SwitchAllCasesNotHandledRule)
                .WithArguments("'Warning'", Resources.Case, TestData.UnionsResult, Resources.Is)
                .WithSpan(18, 9, 24, 10),
        };
        var expectedAfter = new[]
        {
            VerifyCS.Diagnostic(DiscriminatedUnionsAnalyzer.CaseShouldBeImplementedRule)
                .WithArguments("Warning")
                .WithSpan(23, 17, 23, 60),
        };
        await VerifyCS.VerifyCodeFixAsync(test, expected, fixtest, expectedAfter);
    }

    [TestMethod]
    public async Task Given_SwitchStatement_When_NullableContextIsDisabledAndCasesInBetweenIsNotHandled_Then_RemainingCaseShouldBeHandledInBetween()
    {
        var test = $@"{TestData.Usings}

namespace Unions;

public class DiscriminatedUnionSymbolAnalyzerTests
{{   
    public void Switch(Result result)
    {{
        switch(result)
        {{
            case Result.Success success:
                break;
            case Result.Error error:
                break;
        }}
    }}
}}
{TestData.ValidResultUnion}
";

        var fixtest = $@"{TestData.Usings}

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
                throw new System.NotImplementedException();
        }}
    }}
}}
{TestData.ValidResultUnion}
";
        var expected = new[]
        {
            VerifyCS.Diagnostic(DiscriminatedUnionsAnalyzer.SwitchAllCasesNotHandledRule)
                .WithArguments("'Warning', 'null'", Resources.Cases, TestData.UnionsResult, Resources.Are)
                .WithSpan(17, 9, 23, 10),
        };
        var expectedAfter = new[]
        {
            VerifyCS.Diagnostic(DiscriminatedUnionsAnalyzer.CaseShouldBeImplementedRule)
                .WithArguments("Warning")
                .WithSpan(22, 17, 22, 60),
            VerifyCS.Diagnostic(DiscriminatedUnionsAnalyzer.CaseShouldBeImplementedRule)
                .WithArguments("null")
                .WithSpan(26, 17, 26, 60),
        };

        await VerifyCS.VerifyCodeFixAsync(test, expected, fixtest, expectedAfter);
    }

    [TestMethod]
    public async Task Given_SwitchStatement_When_NullableContextIsDisabledAndNoCasesAreHandled_Then_AllCasesAndNullCaseShouldBeHandled()
    {
        var test = $@"{TestData.Usings}

namespace Unions;

public class DiscriminatedUnionSymbolAnalyzerTests
{{   
    public void Switch(Result result)
    {{
        switch(result)
        {{
            default:
                throw new Sundew.DiscriminatedUnions.UnreachableCaseException(typeof(Result));
        }}
    }}
}}
{TestData.ValidResultUnion}
";

        var fixtest = $@"{TestData.Usings}

namespace Unions;

public class DiscriminatedUnionSymbolAnalyzerTests
{{   
    public void Switch(Result result)
    {{
        switch (result)
        {{
            case Result.Success success:
                throw new System.NotImplementedException();
            case Result.Warning warning:
                throw new System.NotImplementedException();
            case Result.Error error:
                throw new System.NotImplementedException();
            case null:
                throw new System.NotImplementedException();
            default:
                throw new Sundew.DiscriminatedUnions.UnreachableCaseException(typeof(Result));
        }}
    }}
}}
{TestData.ValidResultUnion}
";
        var expected = new[]
        {
            VerifyCS.Diagnostic(DiscriminatedUnionsAnalyzer.SwitchAllCasesNotHandledRule)
                .WithArguments("'Success', 'Warning', 'Error', 'null'", Resources.Cases, TestData.UnionsResult, Resources.Are)
                .WithSpan(17, 9, 21, 10),
        };
        var expectedAfter = new[]
        {
            VerifyCS.Diagnostic(DiscriminatedUnionsAnalyzer.CaseShouldBeImplementedRule)
                .WithArguments("Success")
                .WithSpan(20, 17, 20, 60),
            VerifyCS.Diagnostic(DiscriminatedUnionsAnalyzer.CaseShouldBeImplementedRule)
                .WithArguments("Warning")
                .WithSpan(22, 17, 22, 60),
            VerifyCS.Diagnostic(DiscriminatedUnionsAnalyzer.CaseShouldBeImplementedRule)
                .WithArguments("Error")
                .WithSpan(24, 17, 24, 60),
            VerifyCS.Diagnostic(DiscriminatedUnionsAnalyzer.CaseShouldBeImplementedRule)
                .WithArguments("null")
                .WithSpan(26, 17, 26, 60),
        };
        await VerifyCS.VerifyCodeFixAsync(test, expected, fixtest, expectedAfter);
    }

    [TestMethod]
    public async Task Given_SwitchStatement_When_NullableContextIsDisabledAndCasesInBetweenHasPattern_Then_RemainingCaseShouldBeHandledInBetweenAfterPattern()
    {
        var test = $@"{TestData.Usings}

namespace Unions;

public class DiscriminatedUnionSymbolAnalyzerTests
{{   
    public void Switch(Result result)
    {{
        switch(result)
        {{
            case Result.Success success:
                break;
            case Result.Warning {{ Message: ""Tough Warning"" }}:
                break;
            case Result.Error:
                break;
        }}
    }}
}}
{TestData.ValidResultUnion}
";

        var fixtest = $@"{TestData.Usings}

namespace Unions;

public class DiscriminatedUnionSymbolAnalyzerTests
{{   
    public void Switch(Result result)
    {{
        switch (result)
        {{
            case Result.Success success:
                break;
            case Result.Warning {{ Message: ""Tough Warning"" }}:
                break;
            case Result.Warning warning:
                throw new System.NotImplementedException();
            case Result.Error:
                break;
            case null:
                throw new System.NotImplementedException();
        }}
    }}
}}
{TestData.ValidResultUnion}
";
        var expected = new[]
        {
            VerifyCS.Diagnostic(DiscriminatedUnionsAnalyzer.SwitchAllCasesNotHandledRule)
                .WithArguments("'Warning', 'null'", Resources.Cases, TestData.UnionsResult, Resources.Are)
                .WithSpan(17, 9, 25, 10),
        };
        var expectedAfter = new[]
        {
            VerifyCS.Diagnostic(DiscriminatedUnionsAnalyzer.CaseShouldBeImplementedRule)
                .WithArguments("Warning")
                .WithSpan(24, 17, 24, 60),
            VerifyCS.Diagnostic(DiscriminatedUnionsAnalyzer.CaseShouldBeImplementedRule)
                .WithArguments("null")
                .WithSpan(28, 17, 28, 60),
        };

        await VerifyCS.VerifyCodeFixAsync(test, expected, fixtest, expectedAfter);
    }

    [TestMethod]
    public async Task Given_SwitchStatement_When_UnionIsClosedGenericAndNullableContextIsEnableAndMultipleCasesAreNotHandled_Then_RemainingCasesShouldBeHandled()
    {
        var test = $@"#nullable enable
{TestData.Usings}

namespace Unions;

public class DiscriminatedUnionSymbolAnalyzerTests
{{   
    public void Switch(Option<string> textOption)
    {{
        switch (textOption)
        {{
        }}
    }}
}}
{TestData.ValidGenericOptionUnion}
";

        var fixtest = $@"#nullable enable
{TestData.Usings}

namespace Unions;

public class DiscriminatedUnionSymbolAnalyzerTests
{{   
    public void Switch(Option<string> textOption)
    {{
        switch (textOption)
        {{
            case Option<string>.Some some:
                throw new System.NotImplementedException();
            case Option<string>.None none:
                throw new System.NotImplementedException();
        }}
    }}
}}
{TestData.ValidGenericOptionUnion}
";

        var expected = new[]
        {
            VerifyCS.Diagnostic(DiscriminatedUnionsAnalyzer.SwitchAllCasesNotHandledRule)
                .WithArguments("'Some', 'None'", Resources.Cases, TestData.UnionsOptionString, Resources.Are)
                .WithSpan(18, 9, 20, 10),
        };
        var expectedAfter = new[]
        {
            VerifyCS.Diagnostic(DiscriminatedUnionsAnalyzer.CaseShouldBeImplementedRule)
                .WithArguments("Some")
                .WithSpan(21, 17, 21, 60),
            VerifyCS.Diagnostic(DiscriminatedUnionsAnalyzer.CaseShouldBeImplementedRule)
                .WithArguments("None")
                .WithSpan(23, 17, 23, 60),
        };
        await VerifyCS.VerifyCodeFixAsync(test, expected, fixtest, expectedAfter);
    }

    [TestMethod]
    public async Task Given_SwitchStatement_When_UnionIsOpenGenericAndNullableContextIsEnableAndMultipleCasesAreNotHandled_Then_RemainingCasesShouldBeHandled()
    {
        var test = $@"#nullable enable
{TestData.Usings}

namespace Unions;

public class DiscriminatedUnionSymbolAnalyzerTests
{{   
    public void Switch<T>(Option<T> textOption)
        where T : notnull
    {{        
        switch (textOption)
        {{
        }}
    }}
}}
{TestData.ValidGenericOptionUnion}
";

        var fixtest = $@"#nullable enable
{TestData.Usings}

namespace Unions;

public class DiscriminatedUnionSymbolAnalyzerTests
{{   
    public void Switch<T>(Option<T> textOption)
        where T : notnull
    {{
        switch (textOption)
        {{
            case Option<T>.Some some:
                throw new System.NotImplementedException();
            case Option<T>.None none:
                throw new System.NotImplementedException();
        }}
    }}
}}
{TestData.ValidGenericOptionUnion}
";

        var expected = new[]
        {
            VerifyCS.Diagnostic(DiscriminatedUnionsAnalyzer.SwitchAllCasesNotHandledRule)
                .WithArguments("'Some', 'None'", Resources.Cases, TestData.UnionsOptionT, Resources.Are)
                .WithSpan(19, 9, 21, 10),
        };
        var expectedAfter = new[]
        {
            VerifyCS.Diagnostic(DiscriminatedUnionsAnalyzer.CaseShouldBeImplementedRule)
                .WithArguments("Some")
                .WithSpan(22, 17, 22, 60),
            VerifyCS.Diagnostic(DiscriminatedUnionsAnalyzer.CaseShouldBeImplementedRule)
                .WithArguments("None")
                .WithSpan(24, 17, 24, 60),
        };
        await VerifyCS.VerifyCodeFixAsync(test, expected, fixtest, expectedAfter);
    }

    [TestMethod]
    public async Task Given_SwitchStatement_When_UnionIsUnnestedClosedGenericAndNullableContextIsEnableAndMultipleCasesAreNotHandled_Then_RemainingCasesShouldBeHandled()
    {
        var test = $@"#nullable enable
{TestData.Usings}

namespace Unions;

public class DiscriminatedUnionSymbolAnalyzerTests
{{   
    public void Switch(ListCardinality<string> listCardinality)
    {{
        switch (listCardinality)
        {{
        }};
    }}
}}
{TestData.ValidGenericListCardinalityUnion}
";

        var fixtest = $@"#nullable enable
{TestData.Usings}

namespace Unions;

public class DiscriminatedUnionSymbolAnalyzerTests
{{   
    public void Switch(ListCardinality<string> listCardinality)
    {{
        switch (listCardinality)
        {{
            case Empty<string> empty:
                throw new System.NotImplementedException();
            case Multiple<string> multiple:
                throw new System.NotImplementedException();
            case Single<string> single:
                throw new System.NotImplementedException();
        }};
    }}
}}
{TestData.ValidGenericListCardinalityUnion}
";

        var expected = new[]
        {
            VerifyCS.Diagnostic(DiscriminatedUnionsAnalyzer.SwitchAllCasesNotHandledRule)
                .WithArguments("'Empty', 'Multiple', 'Single'", Resources.Cases, TestData.ListCardinalityString, Resources.Are)
                .WithSpan(18, 9, 20, 10),
        };
        var expectedAfter = new[]
        {
            VerifyCS.Diagnostic(DiscriminatedUnionsAnalyzer.CaseShouldBeImplementedRule)
                .WithArguments("Empty")
                .WithSpan(21, 17, 21, 60),
            VerifyCS.Diagnostic(DiscriminatedUnionsAnalyzer.CaseShouldBeImplementedRule)
                .WithArguments("Multiple")
                .WithSpan(23, 17, 23, 60),
            VerifyCS.Diagnostic(DiscriminatedUnionsAnalyzer.CaseShouldBeImplementedRule)
                .WithArguments("Single")
                .WithSpan(25, 17, 25, 60),
        };
        await VerifyCS.VerifyCodeFixAsync(test, expected, fixtest, expectedAfter);
    }
}