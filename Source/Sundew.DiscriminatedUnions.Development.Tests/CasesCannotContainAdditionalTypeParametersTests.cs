// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CasesCannotContainAdditionalTypeParametersTests.cs" company="Sundews">
// Copyright (c) Sundews. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Sundew.DiscriminatedUnions.Development.Tests;

using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Sundew.DiscriminatedUnions.Analyzer;
using VerifyCS = Sundew.DiscriminatedUnions.Development.Tests.Verifiers.CSharpCodeFixVerifier<
    Sundew.DiscriminatedUnions.Analyzer.DiscriminatedUnionsAnalyzer,
    Sundew.DiscriminatedUnions.CodeFixes.DiscriminatedUnionsCodeFixProvider,
    Sundew.DiscriminatedUnions.Analyzer.DiscriminatedUnionSwitchWarningSuppressor>;

[TestClass]
public class CasesCannotContainAdditionalTypeParametersTests
{
    [TestMethod]
    public async Task Given_Union_When_CasesAreNotNestedAndNoFactoryMethodIsDeclared_Then_UnnestedCasesShouldHaveFactoryMethodAreReported2()
    {
        var test = $@"#nullable enable
namespace Unions;

[Sundew.DiscriminatedUnions.DiscriminatedUnion]
public interface IGenericUnion<T>
    where T : notnull
{{
    public T Value {{ get; }}

    [Sundew.DiscriminatedUnions.CaseType(typeof(AdditionTypeParameter<,>))]
    public static IGenericUnion<T> AdditionTypeParameter<TAdditional>(TAdditional additional, T value)
        where TAdditional : notnull
        => new AdditionTypeParameter<TAdditional, T>(additional, value);
}}

public sealed record AdditionTypeParameter<TAdditional, T>(TAdditional Additional, T Value) : IGenericUnion<T>
    where T : notnull
    where TAdditional : notnull;
";

        await VerifyCS.VerifyAnalyzerAsync(
            test,
            VerifyCS.Diagnostic(DiscriminatedUnionsAnalyzer.CasesCannotContainTypeParametersWhichAreNotInTheUnionRule)
                .WithArguments("Unions.AdditionTypeParameter<TAdditional, T>", "TAdditional", "IGenericUnion`1")
                .WithSpan(16, 1, 18, 33));
    }
}