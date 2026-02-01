// --------------------------------------------------------------------------------------------------------------------
// <copyright file="FactoryMethodsMarkerAnalyzerTests.cs" company="Sundews">
// Copyright (c) Sundews. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Sundew.DiscriminatedUnions.Development.Tests;

using System.Threading.Tasks;
using VerifyCS = Sundew.DiscriminatedUnions.Development.Tests.Verifiers.CSharpCodeFixVerifier<
    Sundew.DiscriminatedUnions.Analyzer.PopulateFactoryMethodsMarkerAnalyzer,
    Sundew.DiscriminatedUnions.CodeFixes.DiscriminatedUnionsCodeFixProvider,
    Sundew.DiscriminatedUnions.Analyzer.DiscriminatedUnionSwitchWarningSuppressor>;

public class FactoryMethodsMarkerAnalyzerTests
{
    [Test]
    public async Task
        Given_DiscriminatedUnionWithUnnestedCases_When_UnionIsGenericAndIsPartial_Then_NotDiagnosticsAreReported()
    {
        var test = $@"{TestData.Usings}

namespace Unions;

[DiscriminatedUnion]
public abstract partial record Input<TValue>()
{{
}}

public sealed record IntInput(int Value) : Input<int>;

public sealed record DoubleInput(double Value) : Input<double>;
";

        await VerifyCS.VerifyAnalyzerAsync(test);
    }
}