// --------------------------------------------------------------------------------------------------------------------
// <copyright file="StructUnionTypeSafetyFixture.cs" company="Sundews">
// Copyright (c) Sundews. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Sundew.DiscriminatedUnions.Generator.Development.IntegrationTests.StructUnions;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using AwesomeAssertions;
using Sundew.DiscriminatedUnions.Generator.Development.IntegrationTests.Unions;

/// <summary>
/// Tests that a generated struct union only hands out the value it was created from, as every case type that value is.
/// A struct union keeps its cases in storage that they share, so only its tag says what a field contains,
/// and the compiler trusts what <c>TryGetValue</c> says without looking at the value.
/// The tests therefore generate a union for every combination of the ways cases are stored, compile and load them,
/// and check every member of every union for every value the union can be created from.
/// </summary>
public class StructUnionTypeSafetyFixture
{
    private const string PlainGroup = "Plain";
    private const string GenericGroup = "Generic";
    private const string NestedGroup = "Nested";
    private const string RelatedGroup = "Related";
    private const string NullableGroup = "Nullable";
    private const string FrameworkGroup = "Framework";
    private const string TypeParameter = "TItem";
    private const string TypeParameterList = "<TItem>";
    private const string ListSeparator = ", ";
    private const string MatcherName = "Matcher";
    private const string MatchPrefix = "Match";
    private const string ArmsPrefix = "Arms";
    private const string ValuesPrefix = "Values";
    private const int MaximumReportedFailures = 25;

    // The case types by how a union stores them: nothing for a struct without fields, storage that overlaps for structs without references,
    // a field of its own for a struct with references and one reference field for all classes. The generic ones depend on the type parameter of the union.
    private const string CaseTypes = """
        public readonly record struct Empty0;
        public readonly record struct Empty1;
        public readonly record struct EmptyOf<TItem>;
        public readonly record struct Blittable0(byte Value);
        public readonly record struct Blittable1(int Value);
        public readonly record struct Blittable2(long First, long Second);
        public readonly record struct BlittableOf<TItem>(int Value);
        public readonly record struct Mixed0(string Value);
        public readonly record struct Mixed1(object Item, int Count);
        public readonly record struct MixedOf<TItem>(TItem Value);
        public sealed record Reference0(string Value);
        public sealed record Reference1(int Value);
        public sealed record ReferenceOf<TItem>(TItem Value);
        public interface IShape;
        public readonly record struct Dot(int X) : IShape;
        public readonly record struct Blank : IShape;
        public sealed record Line(int Length) : IShape;
        public sealed record Curve(int Radius) : IShape;
        public interface IFirst;
        public interface ISecond;
        public sealed record First(int Value) : IFirst;
        public sealed record Second(int Value) : ISecond;
        public sealed record Both(int Value) : IFirst, ISecond;
        public sealed class DisposableException(string message) : Exception(message), IDisposable { public void Dispose() { } }

        """;

    private static readonly CasePools Pools = new(["Empty0", "Empty1"], ["Blittable0", "Blittable1", "Blittable2"], ["Mixed0", "Mixed1"], ["Reference0", "Reference1"]);

    // In a generic union the first case of each kind depends on the type parameter, which decides whether it can share storage.
    private static readonly CasePools GenericPools = new(["EmptyOf<TItem>", "Empty0"], ["BlittableOf<TItem>", "Blittable0", "Blittable2"], ["MixedOf<TItem>", "Mixed0"], ["ReferenceOf<TItem>", "Reference0"]);

    private static readonly Type[][] NoTypeArguments = [[]];
    private static readonly Type[][] TypeArguments = [[typeof(int)], [typeof(string)]];

    private static readonly Dictionary<Type, object[]> KnownSamples = new()
    {
        [typeof(bool)] = [true, false],
        [typeof(byte)] = [(byte)1, (byte)2],
        [typeof(char)] = [(char)1, (char)2],
        [typeof(short)] = [(short)1, (short)2],
        [typeof(int)] = [1, 2],
        [typeof(long)] = [1L, 2L],
        [typeof(float)] = [1F, 2F],
        [typeof(double)] = [1D, 2D],
        [typeof(decimal)] = [1M, 2M],
        [typeof(string)] = ["one", "two"],
        [typeof(object)] = ["one", "two"],
        [typeof(Guid)] = [new Guid(1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0), new Guid(2, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0)],
        [typeof(DateTime)] = [new DateTime(1), new DateTime(2)],
        [typeof(DateTimeOffset)] = [new DateTimeOffset(1, TimeSpan.Zero), new DateTimeOffset(2, TimeSpan.Zero)],
        [typeof(TimeSpan)] = [new TimeSpan(1), new TimeSpan(2)],
    };

    private static readonly Lazy<LoadedUnions> Unions = new(LoadUnions);

    [Test]
    public void Load_Then_EveryUnionShouldBeGeneratedAndCompileWithoutWarnings()
    {
        var unions = Unions.Value;

        unions.Result.Diagnostics.Should().BeEmpty();
        unions.Result.GeneratedSources.Should().HaveCount(unions.Sources.Count);
        unions.Sources.Where(x => x.Group is PlainGroup or GenericGroup or NestedGroup).Should().HaveCount(531, "that is every combination of up to 3 blittable, 2 mixed, 2 reference and 2 empty cases, with and without a default case, in three kinds of unions");
        unions.Result.GeneratedSources.Should().Contain(x => x.Source.Contains("private struct __UnmanagedCases"), "cases should overlap within a union")
            .And.Contain(x => x.Source.Contains("internal struct __"), "cases of a generic union should overlap next to the union")
            .And.Contain(x => x.Source.Contains("private readonly object? __reference;"), "reference cases should share a field");
    }

    [Test]
    [Arguments(PlainGroup)]
    [Arguments(GenericGroup)]
    [Arguments(NestedGroup)]
    [Arguments(RelatedGroup)]
    [Arguments(NullableGroup)]
    [Arguments(FrameworkGroup)]
    public void Union_Then_ItShouldOnlyHandOutTheValueItWasCreatedFromAsEveryCaseTheValueIs(string group)
    {
        var unions = Unions.Value;
        var assembly = unions.Result.Assembly;
        assembly.Should().NotBeNull("the unions should compile: {0}", string.Join(Environment.NewLine, unions.Result.Diagnostics.Take(MaximumReportedFailures)));
        var matcher = assembly!.GetType(MatcherName, true)!;
        var failures = new List<string>();
        var verifiedUnions = 0;

        foreach (var union in unions.Sources.Where(x => x.Group == group))
        {
            foreach (var typeArguments in union.IsGeneric ? TypeArguments : NoTypeArguments)
            {
                var unionType = assembly.GetType(union.MetadataName, true)!;
                var verifier = new UnionVerifier(
                    union.IsGeneric ? unionType.MakeGenericType(typeArguments) : unionType,
                    GetMethod(matcher, MatchPrefix + union.Name, typeArguments),
                    (Type[])GetMethod(matcher, ArmsPrefix + union.Name, typeArguments).Invoke(null, null)!,
                    x => failures.Add($"{union.Declaration} {string.Join(ListSeparator, typeArguments.Select(type => type.Name))}: {x}"));
                verifier.Verify(union.Values == null ? null : (object[])GetMethod(matcher, ValuesPrefix + union.Name, typeArguments).Invoke(null, null)!);
                verifiedUnions++;
            }
        }

        verifiedUnions.Should().BeGreaterThan(0);
        string.Join(Environment.NewLine, failures.Take(MaximumReportedFailures)).Should().BeEmpty("{0} checks failed in total", failures.Count);
    }

    private static MethodInfo GetMethod(Type matcher, string name, Type[] typeArguments)
    {
        var method = matcher.GetMethod(name) ?? throw new InvalidOperationException($"The method {name} was not found.");
        return typeArguments.Length > 0 ? method.MakeGenericMethod(typeArguments) : method;
    }

    private static LoadedUnions LoadUnions()
    {
        var unions = GetStorageCombinations().Concat(GetRelatedUnions()).Concat(GetNullableUnions()).Concat(GetFrameworkUnions()).ToArray();
        var source = new StringBuilder(CaseTypes);
        foreach (var union in unions)
        {
            source.AppendLine(union.Declaration);
        }

        // The matcher lets the compiler match each union, which is what the generated members are for.
        source.AppendLine($"public static class {MatcherName}").AppendLine("{");
        foreach (var union in unions)
        {
            var typeParameters = union.IsGeneric ? TypeParameterList : string.Empty;
            var arms = string.Join(ListSeparator, union.Arms.Select((x, index) => $"{x} => {index}"));
            var armTypes = string.Join(ListSeparator, union.Arms.Select(x => $"typeof({x})"));
            source.AppendLine($"public static int {MatchPrefix}{union.Name}{typeParameters}({union.TypeName} value) => value switch {{ {arms}, null => -1 }};");
            source.AppendLine($"public static Type[] {ArmsPrefix}{union.Name}{typeParameters}() => [{armTypes}];");
            if (union.Values != null)
            {
                source.AppendLine($"public static object[] {ValuesPrefix}{union.Name}{typeParameters}() => [{union.Values}];");
            }
        }

        source.AppendLine("}");
        return new LoadedUnions(unions, GeneratorRunner.Load(source.ToString()));
    }

    private static IEnumerable<UnionSource> GetStorageCombinations()
    {
        foreach (var group in new[] { PlainGroup, GenericGroup, NestedGroup })
        {
            var pools = group == GenericGroup ? GenericPools : Pools;
            var number = 0;
            for (var blittable = 0; blittable <= pools.Blittable.Length; blittable++)
            {
                for (var mixed = 0; mixed <= pools.Mixed.Length; mixed++)
                {
                    for (var reference = 0; reference <= pools.Reference.Length; reference++)
                    {
                        for (var empty = 0; empty <= pools.Empty.Length; empty++)
                        {
                            var cases = pools.Empty.Take(empty).Concat(pools.Blittable.Take(blittable)).Concat(pools.Mixed.Take(mixed)).Concat(pools.Reference.Take(reference)).ToArray();

                            // A union needs a case, and when all cases are reference types the generator leaves it to a union declaration.
                            if (cases.Length == reference)
                            {
                                continue;
                            }

                            // The cases are declared in a different order each time, so that tags and fields are not always handed out in the same order.
                            var rotation = number % cases.Length;
                            cases = cases.Skip(rotation).Concat(cases.Take(rotation)).ToArray();
                            yield return CreateUnion(group, group + number++, cases, null);
                            if (empty > 0)
                            {
                                yield return CreateUnion(group, group + number++, cases, pools.Empty[0]);
                            }
                        }
                    }
                }
            }
        }
    }

    private static UnionSource CreateUnion(string group, string name, IReadOnlyList<string> cases, string? defaultCase, IReadOnlyList<string>? arms = null, string? values = null)
    {
        var interfaces = string.Join(ListSeparator, cases.Select(x => $"{(x == defaultCase ? "IDefaultCase" : "ICase")}<{x}>"));
        arms ??= cases;
        if (group == NestedGroup)
        {
            // The union has no type parameter of its own, but it is generic because the class it is nested in is.
            return new UnionSource(group, name, name + "`1+Union", $"public partial class {name}{TypeParameterList} {{ public readonly partial struct Union : {interfaces}; }}", $"{name}{TypeParameterList}.Union", true, arms, values);
        }

        return interfaces.Contains(TypeParameter)
            ? new UnionSource(group, name, name + "`1", $"public readonly partial struct {name}{TypeParameterList} : {interfaces};", name + TypeParameterList, true, arms, values)
            : new UnionSource(group, name, name, $"public readonly partial struct {name} : {interfaces};", name, false, arms, values);
    }

    private static IEnumerable<UnionSource> GetRelatedUnions()
    {
        // The compiler matches a union by the type of its value. So when case types are related, a value is several cases at once,
        // and it is the same union whichever of the constructors that accept it created it.
        yield return CreateUnion(
            RelatedGroup,
            "DerivedClasses",
            ["Exception", "InvalidOperationException", "Blittable0"],
            null,
            ["InvalidOperationException", "Exception", "Blittable0"],
            "new InvalidOperationException(\"a\"), new ObjectDisposedException(\"b\"), new ArgumentException(\"c\"), new Blittable0(1)");
        yield return CreateUnion(
            RelatedGroup,
            "DerivedClassesWithDefault",
            ["Empty0", "ArgumentException", "Exception", "InvalidOperationException"],
            "Empty0",
            ["InvalidOperationException", "ArgumentException", "Exception", "Empty0"],
            "new InvalidOperationException(\"a\"), new ArgumentNullException(\"b\"), new ArgumentException(\"c\"), new FormatException(\"d\"), default(Empty0)");
        yield return CreateUnion(
            RelatedGroup,
            "ImplementedInterface",
            ["IShape", "Dot", "Line", "Blank", "Blittable1"],
            null,
            ["Dot", "Line", "Blank", "IShape", "Blittable1"],
            "new Dot(1), new Dot(2), new Line(1), default(Blank), new Curve(1), new Blittable1(1)");
        yield return CreateUnion(
            RelatedGroup,
            "ImplementedInterfaceWithDefault",
            ["Dot", "IShape", "Blank"],
            "Blank",
            ["Dot", "Blank", "IShape"],
            "new Dot(1), default(Blank), new Line(1), new Curve(2)");
        yield return CreateUnion(
            RelatedGroup,
            "ObjectCase",
            ["Blittable0", "object", "Reference0", "Mixed0"],
            null,
            ["Blittable0", "Reference0", "Mixed0", "object"],
            "\"text\", 5, new Blittable0(1), new Reference0(\"a\"), new Mixed0(\"b\"), new Reference1(1)");
        yield return CreateUnion(
            RelatedGroup,
            "UnrelatedInterfaces",
            ["IFirst", "ISecond", "Blittable0"],
            null,
            null,
            "new First(1), new Second(1), new Both(1), new Blittable0(1)");
        yield return CreateUnion(
            RelatedGroup,
            "ClassAndInterface",
            ["Exception", "IDisposable", "Blittable0"],
            null,
            null,
            "new InvalidOperationException(\"a\"), new DisposableException(\"b\"), new System.IO.MemoryStream(), new Blittable0(1)");
        yield return CreateUnion(
            RelatedGroup,
            "RelatedCollections",
            ["IEnumerable<TItem>", "List<TItem>", "TItem[]", "Blittable0"],
            null,
            ["List<TItem>", "TItem[]", "IEnumerable<TItem>", "Blittable0"],
            "new List<TItem>(), new TItem[1], new HashSet<TItem>(), new Blittable0(1)");
    }

    private static IEnumerable<UnionSource> GetNullableUnions()
    {
        // A case that is declared as a nullable value type is stored as the type it wraps, and null creates the union without a value.
        yield return CreateUnion(NullableGroup, "NullableCase", ["int?", "Reference0"], null, ["int", "Reference0"]);
        yield return CreateUnion(NullableGroup, "NullableAndNotNullable", ["int", "int?", "string"], null, ["int", "string"]);
        yield return CreateUnion(NullableGroup, "NullableWithDefault", ["Empty0", "Blittable1?", "Reference0"], "Empty0", ["Empty0", "Blittable1", "Reference0"]);
        yield return CreateUnion(NullableGroup, "NullableEmpty", ["Empty0?", "int"], null, ["Empty0", "int"]);
        yield return CreateUnion(NullableGroup, "NullableDefault", ["Empty0?", "int", "long?"], "Empty0?", ["Empty0", "int", "long"]);
        yield return CreateUnion(NullableGroup, "NullableOverlapping", ["int?", "long?", "Blittable2?", "string"], null, ["int", "long", "Blittable2", "string"]);
        yield return CreateUnion(NullableGroup, "NullableImplementation", ["Dot?", "IShape", "Blittable1"], null, ["Dot", "IShape", "Blittable1"], "new Dot(1), new Line(1), new Blittable1(1)");
        yield return CreateUnion(NullableGroup, "NullableOfGeneric", ["MixedOf<TItem>?", "int?", "long"], null, ["MixedOf<TItem>", "int", "long"]);
    }

    private static IEnumerable<UnionSource> GetFrameworkUnions()
    {
        // Types the generator knows nothing about beyond their fields: the values of the numeric ones share their leading bytes.
        yield return CreateUnion(FrameworkGroup, "Primitives", ["bool", "byte", "char", "short", "int", "long", "float", "double", "decimal"], null);
        yield return CreateUnion(FrameworkGroup, "FrameworkStructs", ["Guid", "DateTime", "TimeSpan", "DayOfWeek", "DateTimeOffset", "ValueTuple<int, long>"], null);
        yield return CreateUnion(FrameworkGroup, "FrameworkReferences", ["string", "int[]", "Version", "ValueTuple<int, string>", "KeyValuePair<string, int>", "int"], null);
    }

    private static object[] CreateSamples(Type type)
    {
        if (KnownSamples.TryGetValue(type, out var samples))
        {
            return samples;
        }

        if (type.IsEnum)
        {
            return [Enum.ToObject(type, 1), Enum.ToObject(type, 2)];
        }

        if (type.IsArray)
        {
            var elementType = type.GetElementType()!;
            return CreateSamples(elementType).Select(x =>
            {
                var array = Array.CreateInstance(elementType, 1);
                array.SetValue(x, 0);
                return (object)array;
            }).ToArray();
        }

        // A struct without fields has a single value. The other types are created with different arguments, so that they have two values that are not equal.
        var constructor = type.GetConstructors().OrderByDescending(x => x.GetParameters().Length).FirstOrDefault();
        if (constructor == null || constructor.GetParameters().Length == 0)
        {
            return [Activator.CreateInstance(type)!];
        }

        var arguments = constructor.GetParameters().Select(x => CreateSamples(x.ParameterType)).ToArray();
        return Enumerable.Range(0, 2).Select(index => constructor.Invoke(arguments.Select(x => x[Math.Min(index, x.Length - 1)]).ToArray())).ToArray();
    }

    /// <summary>
    /// The case types of the generated unions by how a union stores them.
    /// </summary>
    private sealed record CasePools(string[] Empty, string[] Blittable, string[] Mixed, string[] Reference);

    /// <summary>
    /// A union that the tests declare.
    /// </summary>
    /// <param name="Group">The group of unions it is tested with.</param>
    /// <param name="Name">The name of the union.</param>
    /// <param name="MetadataName">The name of the union type in the assembly.</param>
    /// <param name="Declaration">The source that declares the union.</param>
    /// <param name="TypeName">The union type as it is written in source.</param>
    /// <param name="IsGeneric">A value indicating whether the union has a type parameter or is nested in a type that has.</param>
    /// <param name="Arms">The case types in the order a switch matches them, which is the most specific type first.</param>
    /// <param name="Values">The expressions for the values to create the union from, or <c>null</c> to create it from samples of each case type.</param>
    private sealed record UnionSource(string Group, string Name, string MetadataName, string Declaration, string TypeName, bool IsGeneric, IReadOnlyList<string> Arms, string? Values);

    private sealed record LoadedUnions(IReadOnlyList<UnionSource> Sources, GeneratorRunner.LoadResult Result);

    /// <summary>
    /// A case of a union under test.
    /// </summary>
    /// <param name="Type">The type the union hands out for the case.</param>
    /// <param name="IsDefault">A value indicating whether the case is what the default value of the union represents.</param>
    /// <param name="AcceptsNull">A value indicating whether the union can be created from <c>null</c> for the case.</param>
    /// <param name="Constructor">The constructor that creates the union from the case.</param>
    /// <param name="TryGetValue">The method that gets the case from the union.</param>
    private sealed record UnionCase(Type Type, bool IsDefault, bool AcceptsNull, ConstructorInfo Constructor, MethodInfo TryGetValue);

    /// <summary>
    /// Checks what the members of a union say against the values the union was created from.
    /// What is expected follows from the values alone: a union contains a case when its value is of that type.
    /// </summary>
    private sealed class UnionVerifier
    {
        private const string CaseInterfaceName = "Sundew.DiscriminatedUnions.ICase`1";
        private const string DefaultCaseInterfaceName = "Sundew.DiscriminatedUnions.IDefaultCase`1";

        private readonly Type unionType;
        private readonly MethodInfo match;
        private readonly Type[] arms;
        private readonly Action<string> fail;
        private readonly UnionCase[] cases;
        private readonly PropertyInfo value;
        private readonly PropertyInfo hasValue;
        private readonly MethodInfo equals;
        private readonly MethodInfo equality;
        private readonly MethodInfo inequality;

        public UnionVerifier(Type unionType, MethodInfo match, Type[] arms, Action<string> fail)
        {
            this.unionType = unionType;
            this.match = match;
            this.arms = arms;
            this.fail = fail;
            this.value = unionType.GetProperty("Value")!;
            this.hasValue = unionType.GetProperty("HasValue")!;
            this.equals = unionType.GetMethod(nameof(object.Equals), [unionType])!;
            this.equality = unionType.GetMethod("op_Equality", [unionType, unionType])!;
            this.inequality = unionType.GetMethod("op_Inequality", [unionType, unionType])!;
            this.cases = unionType.GetInterfaces()
                .Where(x => x.IsGenericType && x.GetGenericTypeDefinition().FullName is CaseInterfaceName or DefaultCaseInterfaceName)
                .GroupBy(x => Nullable.GetUnderlyingType(x.GetGenericArguments()[0]) ?? x.GetGenericArguments()[0])
                .Select(x =>
                {
                    // A case that is declared as a nullable value type is created from the nullable type and handed out as the type it wraps.
                    var isNullable = x.Any(type => Nullable.GetUnderlyingType(type.GetGenericArguments()[0]) != null);
                    return new UnionCase(
                        x.Key,
                        x.Any(type => type.GetGenericTypeDefinition().FullName == DefaultCaseInterfaceName),
                        isNullable || !x.Key.IsValueType,
                        unionType.GetConstructor([isNullable ? typeof(Nullable<>).MakeGenericType(x.Key) : x.Key])!,
                        unionType.GetMethod("TryGetValue", [x.Key.MakeByRefType()])!);
                })
                .ToArray();
        }

        public void Verify(object[]? values)
        {
            values ??= this.cases.SelectMany(x => CreateSamples(x.Type)).ToArray();
            var defaultCase = this.cases.SingleOrDefault(x => x.IsDefault);
            var defaultUnion = Activator.CreateInstance(this.unionType)!;
            var defaultValue = defaultCase == null ? null : Activator.CreateInstance(defaultCase.Type);
            this.Verify(defaultUnion, defaultValue, "the default value");

            // A value is created through every constructor that accepts it, because all of them have to produce the same union.
            var unions = new List<(object Union, object? Value, string Description)> { (defaultUnion, defaultValue, "the default value") };
            foreach (var sample in values)
            {
                foreach (var @case in this.cases.Where(x => x.Type.IsInstanceOfType(sample)))
                {
                    var description = $"created from {Describe(sample)} as {@case.Type.Name}";
                    var union = @case.Constructor.Invoke([sample]);
                    this.Verify(union, sample, description);
                    unions.Add((union, sample, description));
                }
            }

            foreach (var @case in this.cases.Where(x => x.AcceptsNull))
            {
                var description = $"created from null as {@case.Type.Name}";
                if (defaultCase == null)
                {
                    var union = @case.Constructor.Invoke([null]);
                    this.Verify(union, null, description);
                    unions.Add((union, null, description));
                    continue;
                }

                // With a default case there is no union without a value, so null is rejected.
                try
                {
                    @case.Constructor.Invoke([null]);
                    this.fail($"{description}: it did not throw");
                }
                catch (TargetInvocationException exception) when (exception.InnerException is ArgumentNullException)
                {
                }
            }

            this.VerifyEquality(unions);
        }

        private static string Describe(object? value)
        {
            return value == null ? "null" : $"{value} ({value.GetType().Name})";
        }

        private void Verify(object union, object? expectedValue, string description)
        {
            this.Check((bool)this.hasValue.GetValue(union)! == (expectedValue != null), description, "HasValue is wrong");
            var actualValue = this.value.GetValue(union);
            this.Check(Equals(actualValue, expectedValue) && actualValue?.GetType() == expectedValue?.GetType(), description, $"Value is {Describe(actualValue)}");
            this.Check(union.ToString() == (expectedValue?.ToString() ?? string.Empty), description, $"ToString returned {union}");
            foreach (var @case in this.cases)
            {
                var arguments = new object?[] { null };
                var hasCase = (bool)@case.TryGetValue.Invoke(union, arguments)!;
                var expectedCase = @case.Type.IsInstanceOfType(expectedValue);
                this.Check(hasCase == expectedCase, description, $"TryGetValue(out {@case.Type.Name}) returned {hasCase}");
                this.Check(
                    Equals(arguments[0], expectedCase ? expectedValue : @case.Type.IsValueType ? Activator.CreateInstance(@case.Type) : null),
                    description,
                    $"TryGetValue(out {@case.Type.Name}) handed out {Describe(arguments[0])}");
            }

            // The switch is compiled against the union, so this is what the compiler makes of the generated members.
            var expectedArm = Array.FindIndex(this.arms, x => x.IsInstanceOfType(expectedValue));
            var arm = (int)this.match.Invoke(null, [union])!;
            this.Check(arm == expectedArm, description, $"a switch matched arm {arm} and not {expectedArm}");
        }

        private void VerifyEquality(IReadOnlyList<(object Union, object? Value, string Description)> unions)
        {
            foreach (var (union, unionValue, description) in unions)
            {
                foreach (var (other, otherValue, otherDescription) in unions)
                {
                    // Unions are equal when their values are, whichever case they were created as.
                    var expected = Equals(unionValue, otherValue);
                    var comparison = $"compared to the one {otherDescription}";
                    this.Check((bool)this.equals.Invoke(union, [other])! == expected, description, $"Equals is not {expected} {comparison}");
                    this.Check(union.Equals(other) == expected, description, $"Equals(object) is not {expected} {comparison}");
                    this.Check((bool)this.equality.Invoke(null, [union, other])! == expected, description, $"== is not {expected} {comparison}");
                    this.Check((bool)this.inequality.Invoke(null, [union, other])! != expected, description, $"!= is {expected} {comparison}");
                    this.Check(!expected || union.GetHashCode() == other.GetHashCode(), description, $"the hash code differs {comparison}");
                }
            }
        }

        private void Check(bool condition, string description, string failure)
        {
            if (!condition)
            {
                this.fail($"{description}: {failure}");
            }
        }
    }
}
