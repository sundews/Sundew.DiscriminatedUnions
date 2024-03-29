﻿// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MultiUnionAnalyzerTests.cs" company="Sundews">
// Copyright (c) Sundews. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Sundew.DiscriminatedUnions.Tests;

using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Sundew.DiscriminatedUnions.Tests.Verifiers;
using VerifyCS = Sundew.DiscriminatedUnions.Tests.Verifiers.CSharpCodeFixVerifier<
    Sundew.DiscriminatedUnions.Analyzer.DiscriminatedUnionsAnalyzer,
    Sundew.DiscriminatedUnions.CodeFixes.DiscriminatedUnionsCodeFixProvider,
    Sundew.DiscriminatedUnions.Analyzer.DiscriminatedUnionSwitchWarningSuppressor>;

[TestClass]
public class MultiUnionAnalyzerTests
{
    [TestMethod]
    public async Task Given_MultiUnion_When_ItHasSubUnions_Then_NoDiagnosticsAreReported()
    {
        var test = $@"#nullable enable
{TestData.Usings}
namespace Unions;

{TestData.ValidMultiUnion}
";
        await VerifyCS.VerifyAnalyzerAsync(test);
    }
}