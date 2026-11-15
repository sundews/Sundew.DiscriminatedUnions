// --------------------------------------------------------------------------------------------------------------------
// <copyright file="UnionSegregationSourceBuilder.cs" company="Sundews">
// Copyright (c) Sundews. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Sundew.DiscriminatedUnions.Generator.Unions;

using System.Collections.Generic;
using System.Linq;
using Sundew.DiscriminatedUnions.Generator.Extensions;
using Sundew.DiscriminatedUnions.Generator.OutputStage;
using Sundew.DiscriminatedUnions.Text;
using static Sundew.DiscriminatedUnions.Generator.Unions.UnionSourceHelper;

/// <summary>
/// Builds the sources that segregate a union: the type with a list per case and the extension method that distributes the unions in an enumerable to those lists.
/// Both are named and shaped as they are for class unions, so that code that segregates a class union keeps working when the union becomes another kind of union.
/// </summary>
internal static class UnionSegregationSourceBuilder
{
    /// <summary>
    /// The suffix that follows the name of the union in the name of the type with a list per case.
    /// </summary>
    public const string SegregationSuffix = GeneratorConstants.Segregation;

    /// <summary>
    /// The suffix that follows the name of the union in the name of the type that declares the extension method.
    /// </summary>
    public const string ExtensionsSuffix = GeneratorConstants.Extensions;

    private const string ValueName = "value";
    private const string UniqueSuffix = "_";
    private const string GlobalAlias = "global::";
    private const string ReadOnlyList = "global::System.Collections.Generic.IReadOnlyList";
    private const string List = "global::System.Collections.Generic.List";
    private const string Enumerable = "global::System.Collections.Generic.IEnumerable";

    /// <summary>
    /// Builds the source of the type that has a list for each case of the specified union.
    /// </summary>
    /// <param name="unionType">The union type.</param>
    /// <param name="segregation">The segregation.</param>
    /// <returns>The source.</returns>
    public static string BuildSegregation(UnionType unionType, UnionSegregation segregation)
    {
        var segregationName = unionType.Name + SegregationSuffix;
        var cases = GetCaseNames(unionType, segregation);
        var sourceWriter = new SourceWriter();
        WriteNamespaceStart(sourceWriter, unionType.Namespace);
        sourceWriter
            .Line("/// <summary>")
            .Line($"/// Contains a list for each case of <c>{EscapeDocumentation(unionType.NameWithTypeParameters)}</c>, with the items that are that case.")
            .Line("/// </summary>")
            .Line(DebuggerNonUserCodeAttribute)
            .Line(GeneratedCodeAttribute)
            .Line($"{GetAccessibility(segregation)} sealed partial class {segregationName}{GetTypeParameterList(segregation)}");
        WriteConstraints(sourceWriter, segregation);
        sourceWriter
            .OpenBlock()
            .Line($"internal {segregationName}({string.Join(ListSeparator, cases.Select(x => $"{ReadOnlyList}<{x.Case.TypeName}> {x.ParameterName}"))})")
            .OpenBlock();
        foreach (var @case in cases)
        {
            sourceWriter.Line($"this.{@case.PropertyName} = {@case.ParameterName};");
        }

        sourceWriter.CloseBlock().Line();
        foreach (var @case in cases)
        {
            sourceWriter
                .Line("/// <summary>")
                .Line($"/// Gets the items that are the <c>{EscapeDocumentation(@case.Case.DisplayName)}</c> case.")
                .Line("/// </summary>")
                .Line($"public {ReadOnlyList}<{@case.Case.TypeName}> {@case.PropertyName} {{ get; }}")
                .Line();
        }

        sourceWriter.CloseBlock();
        WriteNamespaceEnd(sourceWriter, unionType.Namespace);
        return sourceWriter.ToString();
    }

    /// <summary>
    /// Builds the source of the type that declares the extension method, which segregates the specified union.
    /// </summary>
    /// <param name="unionType">The union type.</param>
    /// <param name="segregation">The segregation.</param>
    /// <returns>The source.</returns>
    public static string BuildExtensions(UnionType unionType, UnionSegregation segregation)
    {
        var typeParameterList = GetTypeParameterList(segregation);
        var segregationName = GlobalAlias + (unionType.Namespace.Length > 0 ? unionType.Namespace + '.' : string.Empty) + unionType.Name + SegregationSuffix + typeParameterList;
        var unionsName = GetUnionsName(unionType).AvoidKeywordCollision();
        var cases = GetCaseNames(unionType, segregation);
        var sourceWriter = new SourceWriter();
        WriteNamespaceStart(sourceWriter, unionType.Namespace);

        // The type is partial and only states its accessibility when that is public. That way it combines with the one that is generated for a union
        // of the same name with other type parameters, and with a partial type of that name that the project declares itself.
        sourceWriter
            .Line("/// <summary>")
            .Line($"/// Contains the extension method that segregates <c>{EscapeDocumentation(unionType.NameWithTypeParameters)}</c>.")
            .Line("/// </summary>")
            .Line(DebuggerNonUserCodeAttribute)
            .Line(GeneratedCodeAttribute)
            .Line($"{(segregation.IsPublic ? GeneratorConstants.Public + ' ' : string.Empty)}static partial class {unionType.Name + ExtensionsSuffix}")
            .OpenBlock()
            .Line("/// <summary>")
            .Line("/// Segregates the items in the specified enumerable by case.")
            .Line("/// </summary>")
            .Line($"/// <param name=\"{unionsName.TrimStart('@')}\">The {unionsName.TrimStart('@')}.</param>")
            .Line($"/// <returns>A new <c>{EscapeDocumentation(unionType.Name + SegregationSuffix + typeParameterList)}</c>, which has a list for each case.</returns>")
            .Line($"{GetAccessibility(segregation)} static {segregationName} {GeneratorConstants.Segregate}{typeParameterList}(this {Enumerable}<{segregation.UnionTypeName}> {unionsName})");
        WriteConstraints(sourceWriter, segregation);
        sourceWriter.OpenBlock();
        foreach (var @case in cases)
        {
            sourceWriter.Line($"var {@case.ListName} = new {List}<{@case.Case.TypeName}>();");
        }

        sourceWriter
            .Line()
            .Line($"foreach (var {ValueName} in {unionsName})")
            .OpenBlock();
        var matchedCases = segregation.MatchOrder.Select(x => cases[x]).ToArray();
        if (segregation.CaseMatching == UnionCaseMatching.TryGetValue)
        {
            WriteTryGetValues(sourceWriter, matchedCases);
        }
        else
        {
            WriteSwitch(sourceWriter, matchedCases, segregation.CaseMatching == UnionCaseMatching.Value ? $"{ValueName}.Value" : ValueName);
        }

        sourceWriter
            .CloseBlock()
            .Line()
            .Line($"return new {segregationName}({string.Join(ListSeparator, cases.Select(x => x.ListName))});")
            .CloseBlock()
            .CloseBlock();
        WriteNamespaceEnd(sourceWriter, unionType.Namespace);
        return sourceWriter.ToString();
    }

    private static void WriteSwitch(SourceWriter sourceWriter, IReadOnlyList<CaseNames> cases, string expression)
    {
        sourceWriter
            .Line($"switch ({expression})")
            .OpenBlock();
        foreach (var @case in cases)
        {
            sourceWriter
                .Line($"case {@case.Case.PatternTypeName} {@case.VariableName}:")
                .Indent()
                .Line($"{@case.ListName}.Add({@case.VariableName});")
                .Line("break;")
                .Unindent();
        }

        sourceWriter.CloseBlock();
    }

    private static void WriteTryGetValues(SourceWriter sourceWriter, IReadOnlyList<CaseNames> cases)
    {
        for (var index = 0; index < cases.Count; index++)
        {
            var @case = cases[index];
            sourceWriter
                .Line($"{(index > 0 ? "else " : string.Empty)}if ({ValueName}.TryGetValue(out {@case.Case.PatternTypeName} {@case.VariableName}))")
                .OpenBlock()
                .Line($"{@case.ListName}.Add({@case.VariableName});")
                .CloseBlock();
        }
    }

    private static void WriteNamespaceStart(SourceWriter sourceWriter, string @namespace)
    {
        sourceWriter.Line("#nullable enable").Line();
        if (@namespace.Length > 0)
        {
            sourceWriter.Line($"namespace {@namespace}").OpenBlock();
        }
    }

    private static void WriteNamespaceEnd(SourceWriter sourceWriter, string @namespace)
    {
        if (@namespace.Length > 0)
        {
            sourceWriter.CloseBlock();
        }
    }

    private static void WriteConstraints(SourceWriter sourceWriter, UnionSegregation segregation)
    {
        sourceWriter.Indent();
        foreach (var typeParameter in segregation.TypeParameters.Where(x => x.Constraints.Count > 0))
        {
            sourceWriter.Line($"where {typeParameter.Name} : {string.Join(ListSeparator, typeParameter.Constraints)}");
        }

        sourceWriter.Unindent();
    }

    private static string GetAccessibility(UnionSegregation segregation)
    {
        return segregation.IsPublic ? GeneratorConstants.Public : GeneratorConstants.Internal;
    }

    private static string GetTypeParameterList(UnionSegregation segregation)
    {
        return UnionSourceHelper.GetTypeParameterList(segregation.TypeParameters.Select(x => x.Name).ToArray());
    }

    private static string GetUnionsName(UnionType unionType)
    {
        return unionType.Name.Uncapitalize().Pluralize();
    }

    private static CaseNames[] GetCaseNames(UnionType unionType, UnionSegregation segregation)
    {
        // The names follow from the names of the cases as they do for class unions. What is added here keeps them from colliding with each other and with what surrounds them.
        var propertyNames = new HashSet<string>(UnionSymbolHelper.ObjectMemberNames) { unionType.Name + SegregationSuffix };
        var parameterNames = new HashSet<string>();
        var localNames = new HashSet<string>(segregation.TypeParameters.Select(x => x.Name)) { ValueName, GetUnionsName(unionType) };
        var cases = segregation.Cases;
        var properties = cases.Select(x => GetUniqueName(x.Name.Pluralize(), propertyNames)).ToArray();
        var parameters = properties.Select(x => GetUniqueName(x.Uncapitalize(), parameterNames)).ToArray();
        var lists = cases.Select(x => GetUniqueName(x.Name.Uncapitalize().Pluralize(true), localNames)).ToArray();
        var variables = cases.Select(x => GetUniqueName(x.Name.Uncapitalize(), localNames)).ToArray();
        return cases.Select((x, index) => new CaseNames(
            x,
            properties[index].AvoidKeywordCollision(),
            parameters[index].AvoidKeywordCollision(),
            lists[index].AvoidKeywordCollision(),
            variables[index].AvoidKeywordCollision())).ToArray();
    }

    private static string GetUniqueName(string name, HashSet<string> takenNames)
    {
        while (!takenNames.Add(name))
        {
            name += UniqueSuffix;
        }

        return name;
    }

    private readonly record struct CaseNames(UnionSegregationCase Case, string PropertyName, string ParameterName, string ListName, string VariableName);
}
