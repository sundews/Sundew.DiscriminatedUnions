// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CasesMustBeDeclaredInUnionAssemblyAnalyzerTests.cs" company="Hukano">
// Copyright (c) Hukano. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Sundew.DiscriminatedUnions.Test;

using System.Threading.Tasks;
using Sundew.DiscriminatedUnions.Analyzer;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using VerifyCS = Sundew.DiscriminatedUnions.Test.CSharpCodeFixVerifier<
    Sundew.DiscriminatedUnions.Analyzer.DimensionalUnionsAnalyzer,
    Sundew.DiscriminatedUnions.CodeFixes.DimensionalUnionsCodeFixProvider,
    Sundew.DiscriminatedUnions.Analyzer.DimensionalUnionSwitchWarningSuppressor>;

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
            VerifyCS.Diagnostic(DimensionalUnionsAnalyzer.CasesMustBeDeclaredInUnionAssemblyRule)
                .WithArguments("Unions.DoubleValueExpression", "Sundew.DiscriminatedUnions.TestData")
                .WithSpan(13, 1, 13, 72));
    }
}