// --------------------------------------------------------------------------------------------------------------------
// <copyright file="InterfaceBasedDiscriminatedUnionAnalyzerTests.cs" company="Hukano">
// Copyright (c) Hukano. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Sundew.DiscriminatedUnions.Test;

using System.Threading.Tasks;
using Sundew.DiscriminatedUnions.Analyzer;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using VerifyCS = Sundew.DiscriminatedUnions.Test.CSharpCodeFixVerifier<
    Sundew.DiscriminatedUnions.Analyzer.SundewDiscriminatedUnionsAnalyzer,
    Sundew.DiscriminatedUnions.CodeFixes.SundewDiscriminatedUnionsCodeFixProvider,
    Sundew.DiscriminatedUnions.Analyzer.SundewDiscriminatedUnionSwitchWarningSuppressor>;

[TestClass]
public class InterfaceBasedDiscriminatedUnionAnalyzerTests
{
    [TestMethod]
    public async Task Given_DiscriminatedUnion_When_CaseIsNotSealed_Then_CasesShouldBeSealedIsReported()
    {
        var test = $@"#nullable enable
{TestData.Usings}
namespace Sundew.DiscriminatedUnions.TestData
{{
    public sealed record DoubleValueExpression(double Value) : IExpression;
}}";

        await VerifyCS.VerifyAnalyzerAsync(
            test,
            VerifyCS.Diagnostic(SundewDiscriminatedUnionsAnalyzer.DiscriminatedUnionsMustHavePrivateProtectedConstructorDiagnosticId)
                .WithArguments("Sundew.DiscriminatedUnions.TestData.DoubleValueExpression", "Cases must be in the same assembly")
                .WithSpan(12, 5, 12, 76));
    }
}