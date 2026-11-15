// --------------------------------------------------------------------------------------------------------------------
// <copyright file="StructUnionDefaultValueAnalyzerTests.cs" company="Sundews">
// Copyright (c) Sundews. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Sundew.DiscriminatedUnions.Development.Tests.StructUnions;

using System.Threading.Tasks;
using Sundew.DiscriminatedUnions.Analyzer;
using VerifyCS = Sundew.DiscriminatedUnions.Development.Tests.Verifiers.CSharpAnalyzerVerifier<Sundew.DiscriminatedUnions.Analyzer.StructUnionDefaultValueAnalyzer>;

public class StructUnionDefaultValueAnalyzerTests
{
    // The analyzer recognizes struct unions by the interfaces they list, so the unions do not need their generated members here.
    private const string Unions = @"
namespace Sundew.DiscriminatedUnions
{
    public interface ICase<TCase> { }

    public interface IDefaultCase<TCase> : ICase<TCase> { }
}

namespace Unions
{
    using Sundew.DiscriminatedUnions;

    public readonly struct Empty { }

    public readonly struct Circle { }

    public readonly struct Square { }

    public readonly partial struct Shape : ICase<Circle>, ICase<Square>
    {
        public static Shape None => default;

        public static bool operator ==(Shape left, Shape right) => true;

        public static bool operator !=(Shape left, Shape right) => false;
    }

    public readonly partial struct ShapeOrEmpty : IDefaultCase<Empty>, ICase<Circle>, ICase<Square> { }
}
";

    [Test]
    public async Task Given_StructUnionWithoutDefaultCase_When_DefaultValueIsCreated_Then_ItShouldBeReported()
    {
        var test = Unions + @"
namespace Unions
{
    public class Tests
    {
        public Shape Literal() => default;

        public Shape Expression() => default(Shape);

        public Shape Construction() => new Shape();
    }
}
";

        await VerifyCS.VerifyAnalyzerAsync(
            test,
            VerifyCS.Diagnostic(StructUnionDefaultValueAnalyzer.StructUnionDefaultValueHasNoCaseRule).WithArguments("Shape").WithSpan(35, 35, 35, 42),
            VerifyCS.Diagnostic(StructUnionDefaultValueAnalyzer.StructUnionDefaultValueHasNoCaseRule).WithArguments("Shape").WithSpan(37, 38, 37, 52),
            VerifyCS.Diagnostic(StructUnionDefaultValueAnalyzer.StructUnionDefaultValueHasNoCaseRule).WithArguments("Shape").WithSpan(39, 40, 39, 51));
    }

    [Test]
    public async Task Given_StructUnionWithDefaultCase_When_DefaultValueIsCreated_Then_NothingShouldBeReported()
    {
        var test = Unions + @"
namespace Unions
{
    public class Tests
    {
        public ShapeOrEmpty Literal() => default;

        public ShapeOrEmpty Expression() => default(ShapeOrEmpty);

        public ShapeOrEmpty Construction() => new ShapeOrEmpty();
    }
}
";

        await VerifyCS.VerifyAnalyzerAsync(test);
    }

    [Test]
    public async Task Given_StructUnionWithoutDefaultCase_When_DefaultValueIsOnlyComparedTo_Then_NothingShouldBeReported()
    {
        var test = Unions + @"
namespace Unions
{
    public class Tests
    {
        public bool IsNone(Shape shape) => shape == default || default(Shape) != shape || (shape == (new Shape()));

        public int NotAUnion() => default(int) + new int();
    }
}
";

        await VerifyCS.VerifyAnalyzerAsync(test);
    }
}
