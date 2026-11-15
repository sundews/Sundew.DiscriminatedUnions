// --------------------------------------------------------------------------------------------------------------------
// <copyright file="GeneratorCachingFixture.cs" company="Sundews">
// Copyright (c) Sundews. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Sundew.DiscriminatedUnions.Generator.Development.IntegrationTests.Unions;

using AwesomeAssertions;
using Microsoft.CodeAnalysis;

/// <summary>
/// Tests that the generator only writes a source again when the union it is for has changed.
/// The generator runs every time the code changes. It then compares what it finds out about each union to what it found out the time before,
/// and only an output whose union differs is written again. So nothing that changes with an unrelated edit, such as a position in the file, may be part of that.
/// </summary>
public class GeneratorCachingFixture
{
    private const string Polygon = "public sealed record Polygon(int Sides);";
    private const string Unions = $$"""
        namespace Unions;

        [DiscriminatedUnion(GeneratorFeatures.Segregate)]
        public abstract partial record ClassUnion
        {
            public sealed partial record First : ClassUnion;

            public sealed partial record Second(int Value) : ClassUnion;
        }

        public readonly record struct Circle(double Radius);
        public readonly record struct Square(double Side);
        {{Polygon}}

        [DiscriminatedUnion(GeneratorFeatures.Segregate)]
        public readonly partial struct StructUnion : ICase<Circle>, ICase<Square>, ICase<Polygon>;

        [DiscriminatedUnion(GeneratorFeatures.Segregate)]
        public partial union UnionDeclaration(Circle, Square);

        [DiscriminatedUnion(GeneratorFeatures.Segregate)]
        public closed partial record ClosedType;

        public sealed partial record Leaf(int Value) : ClosedType;

        public sealed record Other : ClosedType;
        """;

    private const string OtherSource = "namespace Unions; public static class Unrelated { public static int Value => 1; }";

    [Test]
    [Arguments("// A line above the unions, which moves every one of them.\n", "")]
    [Arguments("", "\n// A line below the unions.")]
    public void RunAgain_When_ACommentIsAddedToTheSourceThatDeclaresTheUnions_Then_NoSourceShouldBeGeneratedAgain(string prefix, string suffix)
    {
        var result = GeneratorRunner.RunAgain([Unions, OtherSource], [prefix + Unions + suffix, OtherSource]);

        result.Should().NotBeEmpty().And.OnlyContain(x => x == IncrementalStepRunReason.Cached);
    }

    [Test]
    public void RunAgain_When_AnotherSourceChanges_Then_NoSourceShouldBeGeneratedAgain()
    {
        var result = GeneratorRunner.RunAgain([Unions, OtherSource], [Unions, OtherSource.Replace("=> 1", "=> 2")]);

        result.Should().NotBeEmpty().And.OnlyContain(x => x == IncrementalStepRunReason.Cached);
    }

    [Test]
    public void RunAgain_When_ACaseOfOneUnionChanges_Then_OnlyTheSourceOfThatUnionShouldBeGeneratedAgain()
    {
        var result = GeneratorRunner.RunAgain([Unions, OtherSource], [Unions.Replace(Polygon, "public sealed record Polygon(int Sides, string Name);"), OtherSource]);

        result.Should().ContainSingle(x => x == IncrementalStepRunReason.Modified, "the factory of the struct union changes with the constructor of its case")
            .And.OnlyContain(x => x == IncrementalStepRunReason.Modified || x == IncrementalStepRunReason.Cached);
    }
}
