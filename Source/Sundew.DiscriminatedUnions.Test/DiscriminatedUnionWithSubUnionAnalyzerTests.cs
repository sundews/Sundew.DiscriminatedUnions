// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DiscriminatedUnionWithSubUnionAnalyzerTests.cs" company="Hukano">
// Copyright (c) Hukano. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Sundew.DiscriminatedUnions.Test;

using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using VerifyCS = Sundew.DiscriminatedUnions.Test.CSharpCodeFixVerifier<
    Sundew.DiscriminatedUnions.Analyzer.SundewDiscriminatedUnionsAnalyzer,
    Sundew.DiscriminatedUnions.CodeFixes.SundewDiscriminatedUnionsCodeFixProvider,
    Sundew.DiscriminatedUnions.Analyzer.SundewDiscriminatedUnionSwitchWarningSuppressor>;

[TestClass]
public class DiscriminatedUnionWithSubUnionAnalyzerTests
{
    [TestMethod]
    public async Task Given_DiscriminatedUnion_When_ItHasSubUnions_Then_NoDiagnosticsAreReported()
    {
        var test = $@"#nullable enable
{TestData.Usings}
namespace ConsoleApplication1
{{
    {TestData.ValidDiscriminatedUnionWithSubUnions}
}}";
        await VerifyCS.VerifyAnalyzerAsync(test);
    }
}