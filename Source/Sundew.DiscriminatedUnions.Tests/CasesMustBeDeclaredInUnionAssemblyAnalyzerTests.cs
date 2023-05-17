// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CasesMustBeDeclaredInUnionAssemblyAnalyzerTests.cs" company="Sundews">
// Copyright (c) Sundews. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Sundew.DiscriminatedUnions.Test;

using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Sundew.DiscriminatedUnions.Analyzer;
using VerifyCS = Sundew.DiscriminatedUnions.Test.CSharpCodeFixVerifier<
    Sundew.DiscriminatedUnions.Analyzer.DiscriminatedUnionsAnalyzer,
    Sundew.DiscriminatedUnions.CodeFixes.DiscriminatedUnionsCodeFixProvider,
    Sundew.DiscriminatedUnions.Analyzer.DiscriminatedUnionSwitchWarningSuppressor>;

[TestClass]
public class CasesMustBeDeclaredInUnionAssemblyAnalyzerTests
{
    [TestMethod]
    public async Task Given_Union_When_CaseIsDeclaredInDifferentAssembly_Then_CasesMustBeDeclaredInUnionAssemblyIsReported()
    {
        var test = $@"#nullable enable
{TestData.Usings}
namespace Unions;

public sealed record DoubleValueExpression(double Value) : IExpression;
";

        await VerifyCS.VerifyAnalyzerAsync(
            test,
            VerifyCS.Diagnostic(DiscriminatedUnionsAnalyzer.CasesMustBeDeclaredInUnionAssemblyRule)
                .WithArguments("Unions.DoubleValueExpression", "Sundew.DiscriminatedUnions.TestData")
                .WithSpan(13, 1, 13, 72),
            VerifyCS.Diagnostic(DiscriminatedUnionsAnalyzer.UnnestedCasesShouldHaveFactoryMethodRule)
                .WithArguments("Unions.DoubleValueExpression", "Sundew.DiscriminatedUnions.TestData.IExpression")
                .WithSpan(13, 1, 13, 72));
    }
}