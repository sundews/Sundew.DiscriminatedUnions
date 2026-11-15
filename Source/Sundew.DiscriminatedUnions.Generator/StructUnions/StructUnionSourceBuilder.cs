// --------------------------------------------------------------------------------------------------------------------
// <copyright file="StructUnionSourceBuilder.cs" company="Sundews">
// Copyright (c) Sundews. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Sundew.DiscriminatedUnions.Generator.StructUnions;

using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Sundew.DiscriminatedUnions.Generator.DeclarationStage;
using Sundew.DiscriminatedUnions.Generator.Unions;
using static Sundew.DiscriminatedUnions.Generator.Unions.UnionSourceHelper;

/// <summary>
/// Builds the source of the generated part of a struct union.
/// The generated part has the shape the compiler recognizes as a union: the union attribute, the union interface,
/// a constructor and a <c>TryGetValue</c> method per case. Unlike a union declaration it keeps the cases in fields, so nothing is boxed.
/// </summary>
internal static class StructUnionSourceBuilder
{
    /// <summary>
    /// The name of the field that stores the cases that share their storage, because they are unmanaged.
    /// </summary>
    internal const string UnmanagedFieldName = "__unmanaged";

    /// <summary>
    /// The name of the field that stores the cases that share their storage, because they are reference types.
    /// </summary>
    internal const string ReferenceFieldName = "__reference";

    private const string TagFieldName = "__tag";
    private const string UnmanagedTypeName = "__UnmanagedCases";
    private const string SeparateTypePrefix = "__";
    private const string SeparateTypeNameSeparator = "_";
    private const string GlobalAlias = "global::";
    private const string This = "this";
    private const string Other = "other";
    private const string UnionAttribute = "[global::System.Runtime.CompilerServices.UnionAttribute]";
    private const string UnionInterface = "global::System.Runtime.CompilerServices.IUnion";
    private const string EqualityComparer = "global::System.Collections.Generic.EqualityComparer";
    private const string ValueOrThrow = "value ?? throw new global::System.ArgumentNullException(nameof(value))";
    private const string MatchedValueName = "matchedValue";
    private const string Or = " or ";
    private const string OrElse = " || ";

    /// <summary>
    /// Builds the source for the specified struct union.
    /// </summary>
    /// <param name="structUnion">The struct union.</param>
    /// <param name="featureSupport">The features supported by the target framework.</param>
    /// <returns>The source.</returns>
    public static string Build(StructUnionDeclaration structUnion, FeatureSupport featureSupport)
    {
        var cases = GetCaseLayouts(structUnion);
        var defaultCase = cases.Where(x => x.Case.IsDefault).Select(x => (CaseLayout?)x).FirstOrDefault();
        var unionName = structUnion.Type.NameWithTypeParameters;
        var generatedMembers = structUnion.GeneratedMembers;
        var interfaces = new List<string> { UnionInterface };
        if (generatedMembers.HasFlag(GeneratedMembers.Equality))
        {
            interfaces.Add(GetEquatableInterface(unionName));
        }

        if (generatedMembers.HasFlag(GeneratedMembers.Cases) && featureSupport.StaticsMembersInInterfaceSupport == StaticsMembersInInterfaceSupport.StaticAbstractInterfaceMember)
        {
            interfaces.Add(DiscriminatedUnionInterface);
        }

        var sourceWriter = new SourceWriter();
        WriteStart(sourceWriter, structUnion.Type);
        sourceWriter
            .Line(UnionAttribute)
            .Line($"{(structUnion.IsReadOnly ? "readonly " : string.Empty)}partial struct {unionName} : {string.Join(ListSeparator, interfaces)}")
            .OpenBlock();
        var unmanagedTypeName = GetUnmanagedTypeName(structUnion);
        WriteFields(sourceWriter, cases, unmanagedTypeName);
        WriteConstructors(sourceWriter, structUnion.Type.Name, EscapeDocumentation(unionName), cases, defaultCase.HasValue, unmanagedTypeName);
        WriteFactories(sourceWriter, unionName, structUnion.Factories, true);
        if (generatedMembers.HasFlag(GeneratedMembers.Cases))
        {
            WriteCases(sourceWriter, cases.Select(x => x.Case.DeclaredTypeName));
        }

        WriteValue(sourceWriter, cases, defaultCase);
        WriteHasValue(sourceWriter, defaultCase.HasValue);
        WriteTryGetValue(sourceWriter, cases);
        if (generatedMembers.HasFlag(GeneratedMembers.Equality))
        {
            WriteOperators(sourceWriter, unionName);
            var relatedCases = GetRelatedCases(cases);
            WriteEquals(sourceWriter, unionName, cases, relatedCases);
            WriteEqualsObject(sourceWriter, unionName);
            WriteGetHashCode(sourceWriter, cases, relatedCases);
        }

        if (generatedMembers.HasFlag(GeneratedMembers.ToString))
        {
            WriteToString(sourceWriter, cases, defaultCase);
        }

        if (structUnion.CanUseExplicitLayout)
        {
            WriteUnmanagedCases(sourceWriter, cases, EscapeDocumentation(unionName), null);
        }

        sourceWriter.CloseBlock();
        WriteContainingTypesEnd(sourceWriter, structUnion.Type);
        if (!structUnion.CanUseExplicitLayout)
        {
            // A generic type cannot have explicit layout, and neither can a type that is nested in one. So the storage is declared next to them.
            WriteUnmanagedCases(sourceWriter, cases, EscapeDocumentation(unionName), GetSeparateUnmanagedTypeName(structUnion.Type));
        }

        WriteNamespaceEnd(sourceWriter, structUnion.Type);
        return sourceWriter.ToString();
    }

    private static CaseLayout[] GetCaseLayouts(StructUnionDeclaration structUnion)
    {
        var cases = structUnion.Cases;
        var shareUnmanagedStorage = cases.Count(x => x.CanOverlap) > 1;
        var shareReferenceStorage = cases.Count(x => x.Kind == StructUnionCaseKind.Reference) > 1;
        var identifiers = new HashSet<string>();
        var caseLayouts = new CaseLayout[cases.Count];
        var nextTag = 1;
        for (var index = 0; index < cases.Count; index++)
        {
            var @case = cases[index];
            var tag = @case.IsDefault ? 0 : nextTag++;
            var storage = @case.Kind switch
            {
                StructUnionCaseKind.PayloadFree => CaseStorage.None,
                StructUnionCaseKind.Unmanaged when shareUnmanagedStorage && @case.CanOverlap => CaseStorage.UnmanagedField,
                StructUnionCaseKind.Reference when shareReferenceStorage => CaseStorage.ReferenceField,
                _ => CaseStorage.Field,
            };

            var identifier = cases.Count(x => x.Name == @case.Name) > 1 ? @case.Name + tag.ToString(CultureInfo.InvariantCulture) : @case.Name;
            while (!identifiers.Add(identifier))
            {
                identifier += '_';
            }

            caseLayouts[index] = new CaseLayout(@case, tag, storage, identifier);
        }

        return caseLayouts;
    }

    private static string GetUnmanagedTypeName(StructUnionDeclaration structUnion)
    {
        if (structUnion.CanUseExplicitLayout)
        {
            return UnmanagedTypeName;
        }

        var unionType = structUnion.Type;
        return GlobalAlias + (unionType.Namespace.Length > 0 ? unionType.Namespace + '.' : string.Empty) + GetSeparateUnmanagedTypeName(unionType);
    }

    private static string GetSeparateUnmanagedTypeName(UnionType unionType)
    {
        // The name says which union the storage belongs to, including how many type parameters each type has, because types can share a name and differ in those.
        var names = unionType.ContainingTypes.Select(x => GetNameWithArity(x.Name, x.TypeParameters.Count)).Concat([GetNameWithArity(unionType.Name, unionType.TypeParameters.Count)]);
        return SeparateTypePrefix + string.Join(SeparateTypeNameSeparator, names) + SeparateTypeNameSeparator + UnmanagedTypeName.TrimStart('_');
    }

    private static string GetNameWithArity(string name, int arity)
    {
        return arity > 0 ? name + SeparateTypeNameSeparator + arity.ToString(CultureInfo.InvariantCulture) : name;
    }

    private static void WriteFields(SourceWriter sourceWriter, IReadOnlyList<CaseLayout> cases, string unmanagedTypeName)
    {
        foreach (var caseLayout in cases.Where(x => x.Storage == CaseStorage.None))
        {
            sourceWriter
                .Line(GeneratedCodeAttribute)
                .Line($"private static readonly object {caseLayout.BoxedFieldName} = default({caseLayout.Case.TypeName});")
                .Line();
        }

        if (cases.Any(x => x.Storage == CaseStorage.UnmanagedField))
        {
            sourceWriter
                .Line(GeneratedCodeAttribute)
                .Line($"private readonly {unmanagedTypeName} {UnmanagedFieldName};")
                .Line();
        }

        foreach (var caseLayout in cases.Where(x => x.Storage == CaseStorage.Field))
        {
            sourceWriter
                .Line(GeneratedCodeAttribute)
                .Line($"private readonly {caseLayout.Case.TypeName}{(caseLayout.Case.Kind == StructUnionCaseKind.Reference ? "?" : string.Empty)} {caseLayout.FieldName};")
                .Line();
        }

        if (cases.Any(x => x.Storage == CaseStorage.ReferenceField))
        {
            sourceWriter
                .Line(GeneratedCodeAttribute)
                .Line($"private readonly object? {ReferenceFieldName};")
                .Line();
        }

        sourceWriter
            .Line(GeneratedCodeAttribute)
            .Line($"private readonly byte {TagFieldName};")
            .Line();
    }

    private static void WriteConstructors(SourceWriter sourceWriter, string name, string unionDisplayName, IReadOnlyList<CaseLayout> cases, bool hasDefaultCase, string unmanagedTypeName)
    {
        foreach (var caseLayout in cases)
        {
            // A case that is a reference type or a nullable value type can be null. Without a default case null is the union without a value, with a default case there is no such union.
            var acceptsNull = caseLayout.Case.AcceptsNull;
            var value = acceptsNull && hasDefaultCase ? ValueOrThrow : caseLayout.Case.IsNullable ? "value.GetValueOrDefault()" : "value";
            var tag = acceptsNull && !hasDefaultCase ? $"value is null ? (byte)0 : (byte){caseLayout.TagLiteral}" : caseLayout.TagLiteral;
            sourceWriter
                .Line("/// <summary>")
                .Line($"/// Initializes a new instance of the <c>{unionDisplayName}</c> struct that contains the <c>{EscapeDocumentation(caseLayout.Case.DisplayName)}</c> case.")
                .Line("/// </summary>")
                .Line("/// <param name=\"value\">The case.</param>")
                .Line(GeneratedCodeAttribute)
                .Line(DebuggerNonUserCodeAttribute)
                .Line($"public {name}({caseLayout.Case.DeclaredTypeName} value)")
                .OpenBlock();
            switch (caseLayout.Storage)
            {
                case CaseStorage.None when acceptsNull && hasDefaultCase:
                    // Nothing is stored for the case, but null is still not the case.
                    sourceWriter.Line($"_ = {value};");
                    break;
                case CaseStorage.Field:
                    sourceWriter.Line($"this.{caseLayout.FieldName} = {value};");
                    break;
                case CaseStorage.UnmanagedField:
                    sourceWriter.Line($"this.{UnmanagedFieldName} = new {unmanagedTypeName} {{ {caseLayout.Identifier} = {value} }};");
                    break;
                case CaseStorage.ReferenceField:
                    sourceWriter.Line($"this.{ReferenceFieldName} = {value};");
                    break;
            }

            sourceWriter
                .Line($"this.{TagFieldName} = {tag};")
                .CloseBlock()
                .Line();
        }
    }

    private static void WriteValue(SourceWriter sourceWriter, IReadOnlyList<CaseLayout> cases, CaseLayout? defaultCase)
    {
        sourceWriter
            .Line("/// <summary>")
            .Line("/// Gets the case this instance contains. Reading it boxes a case that is a struct, so prefer pattern matching.")
            .Line("/// </summary>")
            .Line(GeneratedCodeAttribute)
            .Line(DebuggerNonUserCodeAttribute)
            .Line("public object? Value")
            .OpenBlock()
            .Line("get")
            .OpenBlock()
            .Line($"switch (this.{TagFieldName})")
            .OpenBlock();
        foreach (var caseLayout in cases.Where(x => !x.Case.IsDefault))
        {
            sourceWriter
                .Line($"case {caseLayout.TagLiteral}:")
                .Indent()
                .Line($"return {GetBoxedExpression(caseLayout)};")
                .Unindent();
        }

        sourceWriter
            .Line("default:")
            .Indent()
            .Line($"return {(defaultCase.HasValue ? GetBoxedExpression(defaultCase.Value) : "null")};")
            .Unindent()
            .CloseBlock()
            .CloseBlock()
            .CloseBlock()
            .Line();
    }

    private static string GetBoxedExpression(CaseLayout caseLayout)
    {
        return caseLayout.Storage switch
        {
            CaseStorage.None => caseLayout.BoxedFieldName,
            CaseStorage.Field => $"this.{caseLayout.FieldName}",
            CaseStorage.UnmanagedField => $"this.{UnmanagedFieldName}.{caseLayout.Identifier}",
            _ => $"this.{ReferenceFieldName}",
        };
    }

    private static void WriteHasValue(SourceWriter sourceWriter, bool hasDefaultCase)
    {
        sourceWriter
            .Line("/// <summary>")
            .Line(hasDefaultCase
                ? "/// Gets a value indicating whether this instance contains a case. It always does, because the default value is a case."
                : "/// Gets a value indicating whether this instance contains a case. The default value does not.")
            .Line("/// </summary>")
            .Line(GeneratedCodeAttribute)
            .Line(DebuggerNonUserCodeAttribute)
            .Line($"public bool HasValue => {(hasDefaultCase ? "true" : $"this.{TagFieldName} != 0")};")
            .Line();
    }

    private static void WriteTryGetValue(SourceWriter sourceWriter, IReadOnlyList<CaseLayout> cases)
    {
        foreach (var caseLayout in cases)
        {
            sourceWriter
                .Line("/// <summary>")
                .Line($"/// Tries to get the <c>{EscapeDocumentation(caseLayout.Case.DisplayName)}</c> case.")
                .Line("/// </summary>")
                .Line("/// <param name=\"value\">The case, if this instance contains it.</param>")
                .Line("/// <returns><c>true</c>, if this instance contains the case, otherwise <c>false</c>.</returns>")
                .Line(GeneratedCodeAttribute)
                .Line(DebuggerNonUserCodeAttribute)
                .Line($"public bool TryGetValue(out {caseLayout.Case.TypeName} value)")
                .OpenBlock();
            if (caseLayout.Case.IsMatchedByOtherCases)
            {
                WriteTryGetValueOfRelatedCases(sourceWriter, cases, caseLayout);
            }
            else if (caseLayout.Storage is CaseStorage.UnmanagedField or CaseStorage.ReferenceField)
            {
                // The storage is shared with other cases, so it only holds this case when the tag says so.
                sourceWriter
                    .Line($"if (this.{TagFieldName} == {caseLayout.TagLiteral})")
                    .OpenBlock()
                    .Line($"value = {caseLayout.GetReadExpression(This)};")
                    .Line("return true;")
                    .CloseBlock()
                    .Line()
                    .Line("value = default!;")
                    .Line("return false;");
            }
            else
            {
                sourceWriter
                    .Line($"value = {caseLayout.GetReadExpression(This)};")
                    .Line($"return this.{TagFieldName} == {caseLayout.TagLiteral};");
            }

            sourceWriter
                .CloseBlock()
                .Line();
        }
    }

    private static void WriteTryGetValueOfRelatedCases(SourceWriter sourceWriter, IReadOnlyList<CaseLayout> cases, CaseLayout caseLayout)
    {
        // The compiler matches a union by the type of the value it contains, and it may ask any case that the type converts to.
        // So a case also hands out the values of the other cases that are of its type.
        sourceWriter
            .Line($"switch (this.{TagFieldName})")
            .OpenBlock()
            .Line($"case {caseLayout.TagLiteral}:")
            .Indent()
            .Line($"value = {caseLayout.GetReadExpression(This)};")
            .Line("return true;")
            .Unindent();
        foreach (var assignableCase in caseLayout.Case.AssignableCases.Select(x => cases[x]))
        {
            // A case without a payload is boxed once, so it is not boxed again each time it is handed out as a reference.
            var read = assignableCase.Storage == CaseStorage.None ? $"({caseLayout.Case.TypeName}){assignableCase.BoxedFieldName}" : assignableCase.GetReadExpression(This);
            sourceWriter
                .Line($"case {assignableCase.TagLiteral}:")
                .Indent()
                .Line($"value = {read};")
                .Line("return true;")
                .Unindent();
        }

        foreach (var castableCase in caseLayout.Case.CastableCases.Select(x => cases[x]))
        {
            var reference = castableCase.Storage == CaseStorage.ReferenceField ? ReferenceFieldName : castableCase.FieldName;
            sourceWriter
                .Line($"case {castableCase.TagLiteral} when this.{reference} is {caseLayout.Case.TypeName} {MatchedValueName}:")
                .Indent()
                .Line($"value = {MatchedValueName};")
                .Line("return true;")
                .Unindent();
        }

        sourceWriter
            .CloseBlock()
            .Line()
            .Line("value = default!;")
            .Line("return false;");
    }

    private static IReadOnlyList<IReadOnlyList<CaseLayout>> GetRelatedCases(IReadOnlyList<CaseLayout> cases)
    {
        // Cases whose types are related can contain the same value, so unions that contain different cases of such a group are compared by their values.
        var groups = Enumerable.Range(0, cases.Count).ToArray();
        for (var index = 0; index < cases.Count; index++)
        {
            foreach (var otherIndex in cases[index].Case.AssignableCases.Concat(cases[index].Case.CastableCases))
            {
                var group = groups[index];
                var otherGroup = groups[otherIndex];
                for (var groupIndex = 0; groupIndex < groups.Length; groupIndex++)
                {
                    if (groups[groupIndex] == otherGroup)
                    {
                        groups[groupIndex] = group;
                    }
                }
            }
        }

        return Enumerable.Range(0, cases.Count)
            .GroupBy(x => groups[x])
            .Where(x => x.Count() > 1)
            .Select(x => (IReadOnlyList<CaseLayout>)x.Select(index => cases[index]).ToArray())
            .ToArray();
    }

    private static void WriteEquals(SourceWriter sourceWriter, string unionName, IReadOnlyList<CaseLayout> cases, IReadOnlyList<IReadOnlyList<CaseLayout>> relatedCases)
    {
        var differentCasesAreEqual = "false";
        if (relatedCases.Count > 0)
        {
            var groups = relatedCases.Select(x => string.Join(Or, x.Select(y => y.TagLiteral))).Select(x => $"(this.{TagFieldName} is {x} && {Other}.{TagFieldName} is {x})").ToArray();
            differentCasesAreEqual = $"{(groups.Length > 1 ? $"({string.Join(OrElse, groups)})" : groups[0])} && object.Equals(this.Value, {Other}.Value)";
        }

        sourceWriter
            .Line("/// <summary>")
            .Line("/// Indicates whether this instance and the other contain the same case with equal values.")
            .Line("/// </summary>")
            .Line("/// <param name=\"other\">The other.</param>")
            .Line("/// <returns><c>true</c>, if this instance and the other are equal, otherwise <c>false</c>.</returns>")
            .Line(GeneratedCodeAttribute)
            .Line(DebuggerNonUserCodeAttribute)
            .Line($"public bool Equals({unionName} other)")
            .OpenBlock()
            .Line($"if (this.{TagFieldName} != other.{TagFieldName})")
            .OpenBlock()
            .Line($"return {differentCasesAreEqual};")
            .CloseBlock()
            .Line()
            .Line($"switch (this.{TagFieldName})")
            .OpenBlock();
        foreach (var caseLayout in cases.Where(x => x.Storage != CaseStorage.None))
        {
            sourceWriter
                .Line($"case {caseLayout.TagLiteral}:")
                .Indent()
                .Line($"return {EqualityComparer}<{caseLayout.Case.TypeName}>.Default.Equals({caseLayout.GetReadExpression(This)}, {caseLayout.GetReadExpression(Other)});")
                .Unindent();
        }

        sourceWriter
            .Line("default:")
            .Indent()
            .Line("return true;")
            .Unindent()
            .CloseBlock()
            .CloseBlock()
            .Line();
    }

    private static void WriteGetHashCode(SourceWriter sourceWriter, IReadOnlyList<CaseLayout> cases, IReadOnlyList<IReadOnlyList<CaseLayout>> relatedCases)
    {
        // Unions that contain different cases can be equal when the cases are related, so for those the hash code only depends on the value.
        var casesHashedByValue = relatedCases.SelectMany(x => x).ToArray();
        sourceWriter
            .Line("/// <inheritdoc/>")
            .Line(GeneratedCodeAttribute)
            .Line(DebuggerNonUserCodeAttribute)
            .Line("public override int GetHashCode()")
            .OpenBlock()
            .Line($"switch (this.{TagFieldName})")
            .OpenBlock();
        foreach (var caseLayout in cases.Where(x => x.Storage != CaseStorage.None || casesHashedByValue.Contains(x)))
        {
            var valueHashCode = $"{EqualityComparer}<{caseLayout.Case.TypeName}>.Default.GetHashCode({caseLayout.GetReadExpression(This)})";
            sourceWriter
                .Line($"case {caseLayout.TagLiteral}:")
                .Indent()
                .Line(casesHashedByValue.Contains(caseLayout) ? $"return {valueHashCode};" : $"return unchecked(({caseLayout.TagLiteral} * 397) ^ {valueHashCode});")
                .Unindent();
        }

        sourceWriter
            .Line("default:")
            .Indent()
            .Line($"return this.{TagFieldName};")
            .Unindent()
            .CloseBlock()
            .CloseBlock()
            .Line();
    }

    private static void WriteToString(SourceWriter sourceWriter, IReadOnlyList<CaseLayout> cases, CaseLayout? defaultCase)
    {
        sourceWriter
            .Line("/// <inheritdoc/>")
            .Line(GeneratedCodeAttribute)
            .Line(DebuggerNonUserCodeAttribute)
            .Line("public override string ToString()")
            .OpenBlock()
            .Line($"switch (this.{TagFieldName})")
            .OpenBlock();
        foreach (var caseLayout in cases.Where(x => !x.Case.IsDefault))
        {
            sourceWriter
                .Line($"case {caseLayout.TagLiteral}:")
                .Indent()
                .Line($"return {caseLayout.GetReadExpression(This)}.ToString() ?? string.Empty;")
                .Unindent();
        }

        sourceWriter
            .Line("default:")
            .Indent()
            .Line(defaultCase.HasValue ? $"return {defaultCase.Value.GetReadExpression(This)}.ToString() ?? string.Empty;" : "return string.Empty;")
            .Unindent()
            .CloseBlock()
            .CloseBlock()
            .Line();
    }

    private static void WriteUnmanagedCases(SourceWriter sourceWriter, IReadOnlyList<CaseLayout> cases, string unionDisplayName, string? separateTypeName)
    {
        var unmanagedCases = cases.Where(x => x.Storage == CaseStorage.UnmanagedField).ToArray();
        if (unmanagedCases.Length == 0)
        {
            return;
        }

        // Unmanaged cases contain no references, so the runtime allows them to overlap. The union is then as large as its largest case.
        if (separateTypeName != null)
        {
            sourceWriter
                .Line()
                .Line("/// <summary>")
                .Line($"/// Stores the cases of <c>{unionDisplayName}</c> that contain no references and do not depend on a type parameter, so that they can overlap.")
                .Line("/// It is a type of its own, because the runtime does not allow a generic type to lay out its fields explicitly.")
                .Line("/// </summary>")
                .Line("[global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Never)]");
        }

        sourceWriter
            .Line("[global::System.Runtime.InteropServices.StructLayoutAttribute(global::System.Runtime.InteropServices.LayoutKind.Explicit)]")
            .Line(GeneratedCodeAttribute)
            .Line(separateTypeName != null ? $"internal struct {separateTypeName}" : $"private struct {UnmanagedTypeName}")
            .OpenBlock();
        foreach (var caseLayout in unmanagedCases)
        {
            sourceWriter
                .Line("[global::System.Runtime.InteropServices.FieldOffsetAttribute(0)]")
                .Line($"public {caseLayout.Case.TypeName} {caseLayout.Identifier};")
                .Line();
        }

        sourceWriter.CloseBlock();
        if (separateTypeName == null)
        {
            sourceWriter.Line();
        }
    }
}
