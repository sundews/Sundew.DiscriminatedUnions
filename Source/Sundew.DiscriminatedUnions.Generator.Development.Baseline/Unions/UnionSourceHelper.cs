// --------------------------------------------------------------------------------------------------------------------
// <copyright file="UnionSourceHelper.cs" company="Sundews">
// Copyright (c) Sundews. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Sundew.DiscriminatedUnions.Generator.Unions;

using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Sundew.DiscriminatedUnions.Generator.Model;
using Sundew.DiscriminatedUnions.Generator.OutputStage;

/// <summary>
/// Writes the source that the generated part of a struct union and of a union declaration have in common.
/// </summary>
internal static class UnionSourceHelper
{
    /// <summary>
    /// The separator between the items of a list.
    /// </summary>
    public const string ListSeparator = ", ";

    /// <summary>
    /// The attribute that keeps the debugger from stepping into generated members.
    /// </summary>
    public const string DebuggerNonUserCodeAttribute = "[global::System.Diagnostics.DebuggerNonUserCodeAttribute]";

    /// <summary>
    /// The interface that gives access to the list of cases of a union.
    /// </summary>
    public const string DiscriminatedUnionInterface = "global::Sundew.DiscriminatedUnions.IDiscriminatedUnion";

    private const string GeneratorName = "Sundew.DiscriminateUnions.Generator";

    /// <summary>
    /// Gets the attribute that marks a member as generated.
    /// </summary>
    public static string GeneratedCodeAttribute { get; } = $"[global::System.CodeDom.Compiler.GeneratedCodeAttribute(\"{GeneratorName}\", \"{Assembly.GetExecutingAssembly().GetName().Version}\")]";

    /// <summary>
    /// Gets the type parameter list for the specified type parameters.
    /// </summary>
    /// <param name="typeParameters">The type parameters.</param>
    /// <returns>The type parameter list, which is empty when there are no type parameters.</returns>
    public static string GetTypeParameterList(IReadOnlyCollection<string> typeParameters)
    {
        return typeParameters.Count > 0 ? $"<{string.Join(ListSeparator, typeParameters)}>" : string.Empty;
    }

    /// <summary>
    /// Gets the interface that makes the specified union equatable to itself.
    /// </summary>
    /// <param name="unionName">The name of the union.</param>
    /// <returns>The interface.</returns>
    public static string GetEquatableInterface(string unionName)
    {
        return $"global::System.IEquatable<{unionName}>";
    }

    /// <summary>
    /// Escapes the specified text, so that it can be written in a documentation comment.
    /// </summary>
    /// <param name="text">The text.</param>
    /// <returns>The escaped text.</returns>
    public static string EscapeDocumentation(string text)
    {
        return text.Replace("&", "&amp;").Replace("<", "&lt;").Replace(">", "&gt;");
    }

    /// <summary>
    /// Writes what precedes the declaration of the union: the nullable context, the namespace and the types the union is nested in.
    /// </summary>
    /// <param name="sourceWriter">The source writer.</param>
    /// <param name="unionType">The union type.</param>
    public static void WriteStart(SourceWriter sourceWriter, UnionType unionType)
    {
        sourceWriter.Line("#nullable enable").Line();
        if (unionType.Namespace.Length > 0)
        {
            sourceWriter.Line($"namespace {unionType.Namespace}").OpenBlock();
        }

        foreach (var containingType in unionType.ContainingTypes)
        {
            sourceWriter.Line($"partial {GetKeyword(containingType.UnderlyingType)} {containingType.Name}{GetTypeParameterList(containingType.TypeParameters)}").OpenBlock();
        }
    }

    /// <summary>
    /// Writes what follows the declaration of the union, which closes what <see cref="WriteStart"/> opened.
    /// </summary>
    /// <param name="sourceWriter">The source writer.</param>
    /// <param name="unionType">The union type.</param>
    public static void WriteEnd(SourceWriter sourceWriter, UnionType unionType)
    {
        WriteContainingTypesEnd(sourceWriter, unionType);
        WriteNamespaceEnd(sourceWriter, unionType);
    }

    /// <summary>
    /// Writes the end of the types the union is nested in, which leaves the namespace open for what is declared next to them.
    /// </summary>
    /// <param name="sourceWriter">The source writer.</param>
    /// <param name="unionType">The union type.</param>
    public static void WriteContainingTypesEnd(SourceWriter sourceWriter, UnionType unionType)
    {
        for (var index = 0; index < unionType.ContainingTypes.Count; index++)
        {
            sourceWriter.CloseBlock();
        }
    }

    /// <summary>
    /// Writes the end of the namespace of the union.
    /// </summary>
    /// <param name="sourceWriter">The source writer.</param>
    /// <param name="unionType">The union type.</param>
    public static void WriteNamespaceEnd(SourceWriter sourceWriter, UnionType unionType)
    {
        if (unionType.Namespace.Length > 0)
        {
            sourceWriter.CloseBlock();
        }
    }

    /// <summary>
    /// Writes the property that lists the cases of the union.
    /// </summary>
    /// <param name="sourceWriter">The source writer.</param>
    /// <param name="caseTypeNames">The fully qualified names of the case types.</param>
    public static void WriteCases(SourceWriter sourceWriter, IEnumerable<string> caseTypeNames)
    {
        sourceWriter
            .Line("/// <summary>")
            .Line("/// Gets all cases in the union.")
            .Line("/// </summary>")
            .Line(GeneratedCodeAttribute)
            .Line("public static global::System.Collections.Generic.IReadOnlyList<global::System.Type> Cases { get; }")
            .Indent()
            .Line($"= new global::System.Type[] {{ {string.Join(ListSeparator, caseTypeNames.Select(x => $"typeof({x})"))} }};")
            .Unindent()
            .Line();
    }

    /// <summary>
    /// Writes the factories of the union: the members that create the union from a new instance of a case.
    /// </summary>
    /// <param name="sourceWriter">The source writer.</param>
    /// <param name="unionName">The name of the union.</param>
    /// <param name="factories">The factories.</param>
    /// <param name="wrapsCases">A value indicating whether the union is created from the case. Otherwise the case is the union, as it is when it derives from a closed type.</param>
    public static void WriteFactories(SourceWriter sourceWriter, string unionName, IEnumerable<UnionFactory> factories, bool wrapsCases)
    {
        var unionDisplayName = EscapeDocumentation(unionName);
        foreach (var factory in factories)
        {
            var caseDisplayName = EscapeDocumentation(factory.CaseDisplayName);
            var accessibility = factory.IsPublic ? "public" : "internal";
            var newCase = $"new {factory.CaseTypeName}({string.Join(ListSeparator, factory.Parameters.Select(x => x.Name))})";
            var creation = wrapsCases ? $"new {unionName}({newCase})" : newCase;
            if (factory.Parameters.Count == 0)
            {
                sourceWriter
                    .Line("/// <summary>")
                    .Line(wrapsCases
                        ? $"/// Gets the <c>{unionDisplayName}</c> that contains the <c>{caseDisplayName}</c> case."
                        : $"/// Gets the <c>{caseDisplayName}</c> case of <c>{unionDisplayName}</c>.")
                    .Line("/// </summary>")
                    .Line(GeneratedCodeAttribute);
                if (factory.IsCreatedOnce)
                {
                    // Without parameters there is only one such union. Where creating it allocates, it is created once and kept.
                    sourceWriter
                        .Line($"{accessibility} static {unionName} {factory.Name} {{ get; }}")
                        .Indent()
                        .Line($"= {creation};")
                        .Unindent();
                }
                else
                {
                    sourceWriter.Line($"{accessibility} static {unionName} {factory.Name} => {creation};");
                }

                sourceWriter.Line();
                continue;
            }

            sourceWriter
                .Line("/// <summary>")
                .Line(wrapsCases
                    ? $"/// Creates a new <c>{unionDisplayName}</c> that contains a new <c>{caseDisplayName}</c> case."
                    : $"/// Creates a new <c>{caseDisplayName}</c> case of <c>{unionDisplayName}</c>.")
                .Line("/// </summary>");
            foreach (var parameter in factory.Parameters)
            {
                var parameterName = parameter.Name.TrimStart('@');
                sourceWriter.Line($"/// <param name=\"{parameterName}\">The {parameterName}.</param>");
            }

            sourceWriter
                .Line(wrapsCases ? $"/// <returns>The <c>{unionDisplayName}</c>.</returns>" : $"/// <returns>The case as <c>{unionDisplayName}</c>.</returns>")
                .Line(GeneratedCodeAttribute)
                .Line(DebuggerNonUserCodeAttribute)
                .Line($"{accessibility} static {unionName} {factory.Name}({GetParameterList(factory)})")
                .OpenBlock()
                .Line($"return {creation};")
                .CloseBlock()
                .Line();
        }
    }

    /// <summary>
    /// Writes the equality operators, which compare by calling the Equals method of the union.
    /// </summary>
    /// <param name="sourceWriter">The source writer.</param>
    /// <param name="unionName">The name of the union.</param>
    public static void WriteOperators(SourceWriter sourceWriter, string unionName)
    {
        WriteOperator(sourceWriter, unionName, "==", "left.Equals(right)", "equal");
        WriteOperator(sourceWriter, unionName, "!=", "!left.Equals(right)", "not equal");
    }

    /// <summary>
    /// Writes the override of the Equals method that takes an object.
    /// </summary>
    /// <param name="sourceWriter">The source writer.</param>
    /// <param name="unionName">The name of the union.</param>
    public static void WriteEqualsObject(SourceWriter sourceWriter, string unionName)
    {
        sourceWriter
            .Line("/// <inheritdoc/>")
            .Line(GeneratedCodeAttribute)
            .Line(DebuggerNonUserCodeAttribute)
            .Line("public override bool Equals(object? obj)")
            .OpenBlock()
            .Line($"return obj is {unionName} other && this.Equals(other);")
            .CloseBlock()
            .Line();
    }

    private static void WriteOperator(SourceWriter sourceWriter, string unionName, string operatorToken, string expression, string description)
    {
        sourceWriter
            .Line("/// <summary>")
            .Line($"/// Indicates whether the left and the right are {description}.")
            .Line("/// </summary>")
            .Line("/// <param name=\"left\">The left.</param>")
            .Line("/// <param name=\"right\">The right.</param>")
            .Line($"/// <returns><c>true</c>, if the left and the right are {description}, otherwise <c>false</c>.</returns>")
            .Line(GeneratedCodeAttribute)
            .Line(DebuggerNonUserCodeAttribute)
            .Line($"public static bool operator {operatorToken}({unionName} left, {unionName} right)")
            .OpenBlock()
            .Line($"return {expression};")
            .CloseBlock()
            .Line();
    }

    private static string GetParameterList(UnionFactory factory)
    {
        return string.Join(
            ListSeparator,
            factory.Parameters.Select((parameter, index) =>
            {
                var modifier = factory.HasParameterArray && index == factory.Parameters.Count - 1 ? "params " : string.Empty;
                var defaultValue = parameter.DefaultValue != null ? " = " + parameter.DefaultValue : string.Empty;
                return $"{modifier}{parameter.TypeName} {parameter.Name}{defaultValue}";
            }));
    }

    private static string GetKeyword(UnderlyingType underlyingType)
    {
        return underlyingType switch
        {
            UnderlyingType.RecordClass => GeneratorConstants.Record,
            UnderlyingType.Interface => GeneratorConstants.Interface,
            UnderlyingType.Struct => GeneratorConstants.Struct,
            UnderlyingType.RecordStruct => GeneratorConstants.RecordStruct,
            _ => GeneratorConstants.Class,
        };
    }
}
