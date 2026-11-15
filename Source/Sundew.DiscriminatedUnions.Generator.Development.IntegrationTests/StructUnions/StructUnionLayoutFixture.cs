// --------------------------------------------------------------------------------------------------------------------
// <copyright file="StructUnionLayoutFixture.cs" company="Sundews">
// Copyright (c) Sundews. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Sundew.DiscriminatedUnions.Generator.Development.IntegrationTests.StructUnions;

using System;
using System.Linq;
using AwesomeAssertions;
using Sundew.DiscriminatedUnions.Generator.Development.IntegrationTests.Unions;

public class StructUnionLayoutFixture
{
    private const string SharedUnmanagedStorage = "__UnmanagedCases";
    private const string SharedReferenceStorage = "private readonly object? __reference;";
    private const string Cases = """
        public readonly record struct Empty;
        public readonly record struct Count(int Value);
        public readonly record struct Range(long From, long To);
        public readonly record struct Named(string Name);
        public sealed record Failure(string Reason);
        public sealed record Problem(string Title);

        """;

    [Test]
    public void Run_When_UnionHasSeveralUnmanagedCases_Then_TheyShouldShareStorage()
    {
        var source = Generate("public readonly partial struct Union : ICase<Count>, ICase<Range>, ICase<Named>, ICase<Empty>;");

        source.Should().Contain("private readonly __UnmanagedCases __unmanaged;")
            .And.Contain("public global::Count Count;")
            .And.Contain("public global::Range Range;")
            .And.Contain("private readonly global::Named __caseNamed;")
            .And.Contain("private static readonly object __boxedEmpty = default(global::Empty);")
            .And.NotContain("__caseCount")
            .And.NotContain("__caseEmpty");
    }

    [Test]
    [Arguments("public readonly partial struct Union : ICase<Count>, ICase<Named>;")]
    [Arguments("public readonly record struct Tagged<TItem>(int Value); public readonly partial struct Union<TItem> : ICase<Count>, ICase<Tagged<TItem>>;")]
    [Arguments("public partial class Outer<TItem> { public readonly record struct Inner(int Value); public readonly partial struct Union : ICase<Count>, ICase<Inner>; }")]
    public void Run_When_UnmanagedCasesCannotOverlap_Then_EachShouldHaveItsOwnField(string union)
    {
        var source = Generate(union);

        source.Should().Contain("private readonly global::Count __caseCount;").And.NotContain(SharedUnmanagedStorage);
    }

    [Test]
    [Arguments("public readonly partial struct Union<TItem> : ICase<Count>, ICase<Range>, ICase<List<TItem>>;", "__Union_1_UnmanagedCases")]
    [Arguments("public partial class Outer<TItem> { public readonly partial struct Union : ICase<Count>, ICase<Range>; }", "__Outer_1_Union_UnmanagedCases")]
    [Arguments("public partial class Outer<TFirst, TSecond> { public partial class Middle { public readonly partial struct Union<TItem> : ICase<Count>, ICase<Range>; } }", "__Outer_2_Middle_Union_1_UnmanagedCases")]
    public void Run_When_UnionIsGeneric_Then_CasesThatDoNotDependOnATypeParameterShouldShareStorageThatIsDeclaredNextToTheUnion(string union, string expectedTypeName)
    {
        // The runtime does not allow a generic type, or a type nested in one, to lay out its fields explicitly.
        var source = Generate(union);

        source.Should().Contain($"private readonly global::{expectedTypeName} __unmanaged;")
            .And.Contain($"this.__unmanaged = new global::{expectedTypeName} {{ Count = value }};")
            .And.Contain("public global::Count Count;")
            .And.Contain("public global::Range Range;")
            .And.NotContain("__caseCount")
            .And.NotContain("private struct");
        var lines = source.ReplaceLineEndings().Split(Environment.NewLine);
        lines.Should().Contain($"internal struct {expectedTypeName}", "it is declared in the namespace, where nothing it is nested in makes it generic");
        lines[Array.IndexOf(lines, $"internal struct {expectedTypeName}") - 2].Should().Be("[global::System.Runtime.InteropServices.StructLayoutAttribute(global::System.Runtime.InteropServices.LayoutKind.Explicit)]");
    }

    [Test]
    public void Run_When_GenericUnionIsInANamespace_Then_SharedStorageShouldBeDeclaredInThatNamespace()
    {
        var result = GeneratorRunner.Run("namespace Space { public readonly record struct Count(int Value); public readonly record struct Range(long From, long To); public readonly partial struct Union<TItem> : ICase<Count>, ICase<Range>, ICase<List<TItem>>; }");

        result.CompilationDiagnostics.Should().BeEmpty();
        var source = result.GeneratedSources.Single().Source;
        source.Should().Contain("private readonly global::Space.__Union_1_UnmanagedCases __unmanaged;");
        source.ReplaceLineEndings().Split(Environment.NewLine).Should().Contain("    internal struct __Union_1_UnmanagedCases");
    }

    [Test]
    public void Run_When_CaseOfGenericUnionMentionsATypeParameterOrIsHiddenWithinAType_Then_OnlyTheOtherCasesShouldShareStorage()
    {
        // The shared storage has no type parameters and is not nested, so it cannot hold a case that needs either, even when that case contains no references.
        var source = Generate("""
            public readonly record struct Tagged<TItem>(int Value);
            public partial class Holder
            {
                private readonly record struct Hidden(int Value);

                private readonly partial struct Union<TItem> : ICase<Count>, ICase<Range>, ICase<Tagged<TItem>>, ICase<Hidden>;
            }
            """);

        source.Should().Contain("private readonly global::__Holder_Union_1_UnmanagedCases __unmanaged;")
            .And.Contain("public global::Count Count;")
            .And.Contain("public global::Range Range;")
            .And.Contain("private readonly global::Tagged<TItem> __caseTagged;")
            .And.Contain("private readonly global::Holder.Hidden __caseHidden;");
    }

    [Test]
    public void Run_When_GenericUnionsShareANameAndDifferInTypeParameters_Then_EachShouldGetSharedStorageOfItsOwn()
    {
        var result = GeneratorRunner.Run(Cases + "public readonly partial struct Union<TItem> : ICase<Count>, ICase<Range>; public readonly partial struct Union<TFirst, TSecond> : ICase<Count>, ICase<Range>;");

        result.GeneratorDiagnostics.Should().BeEmpty();
        result.CompilationDiagnostics.Should().BeEmpty();
        result.GeneratedSources[0].Source.Should().Contain("internal struct __Union_1_UnmanagedCases");
        result.GeneratedSources[1].Source.Should().Contain("internal struct __Union_2_UnmanagedCases");
    }

    [Test]
    public void Run_When_UnionHasSeveralReferenceCases_Then_TheyShouldShareAReferenceField()
    {
        var source = Generate("public readonly partial struct Union : ICase<Failure>, ICase<Problem>, ICase<Count>;");

        source.Should().Contain(SharedReferenceStorage).And.NotContain("__caseFailure").And.NotContain("__caseProblem");
    }

    [Test]
    public void Run_When_UnionHasOneReferenceCase_Then_ItShouldHaveItsOwnField()
    {
        var source = Generate("public readonly partial struct Union : ICase<Failure>, ICase<Count>;");

        source.Should().Contain("private readonly global::Failure? __caseFailure;").And.NotContain(SharedReferenceStorage);
    }

    [Test]
    public void Run_When_CasesArePrimitives_Then_TheyShouldBeStored()
    {
        var source = Generate("public readonly partial struct Union : ICase<int>, ICase<double>, ICase<bool>;");

        source.Should().Contain("public int Int32;").And.Contain("public double Double;").And.Contain("public bool Boolean;").And.NotContain("__boxed");
    }

    [Test]
    public void Run_When_CasesHaveTheSameName_Then_TheirStorageShouldBeNumbered()
    {
        var source = Generate("public readonly partial struct Union : ICase<List<int>>, ICase<List<string>>, ICase<KeyValuePair<int, int>>, ICase<KeyValuePair<long, long>>;");

        source.Should().Contain("public global::System.Collections.Generic.KeyValuePair<int, int> KeyValuePair3;")
            .And.Contain("public global::System.Collections.Generic.KeyValuePair<long, long> KeyValuePair4;");
    }

    [Test]
    [Arguments("net11.0", true)]
    [Arguments("net8.0", true)]
    [Arguments("net6.0", false)]
    [Arguments("netstandard2.0", false)]
    public void Run_Then_CaseListInterfaceShouldOnlyBeImplementedWhereItExists(string targetFramework, bool expectedResult)
    {
        var source = GeneratorRunner.Run(Cases + "public readonly partial struct Union : ICase<Count>, ICase<Range>;", targetFramework).GeneratedSources.Single().Source;

        source.Contains("global::Sundew.DiscriminatedUnions.IDiscriminatedUnion").Should().Be(expectedResult);
        source.Should().Contain("public static global::System.Collections.Generic.IReadOnlyList<global::System.Type> Cases { get; }");
    }

    [Test]
    public void Run_When_ReferenceAssemblyLeavesOutTheFieldsOfStructs_Then_ThoseStructsShouldNeitherShareStorageNorLoseIt()
    {
        // System.Guid and System.Threading.CancellationToken have no fields in the reference assembly of .NET Standard 2.0,
        // although the first has data and the second a reference at runtime. Token wraps one of them in a struct declared in source.
        const string source = """
            namespace Sundew.DiscriminatedUnions { public interface ICase<TCase> { } }
            public readonly struct Token { public readonly System.Threading.CancellationToken Value; }
            public readonly partial struct Union : ICase<Guid>, ICase<System.Threading.CancellationToken>, ICase<Token>, ICase<int>, ICase<long>;
            """;

        var generatedSource = GeneratorRunner.Run(source, "netstandard2.0", GeneratorRunner.NetStandard20References).GeneratedSources.Single().Source;

        generatedSource.Should().Contain("private readonly global::System.Guid __caseGuid;")
            .And.Contain("private readonly global::System.Threading.CancellationToken __caseCancellationToken;")
            .And.Contain("private readonly global::Token __caseToken;")
            .And.Contain("public int Int32;")
            .And.Contain("public long Int64;")
            .And.NotContain("__boxed");
    }

    private static string Generate(string union)
    {
        var result = GeneratorRunner.Run(Cases + union);
        result.GeneratorDiagnostics.Should().BeEmpty();
        result.CompilationDiagnostics.Should().BeEmpty();
        return result.GeneratedSources.Single().Source;
    }
}
