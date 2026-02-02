// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DiscriminatedUnionOutputProvider.cs" company="Sundews">
// Copyright (c) Sundews. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Sundew.DiscriminatedUnions.Generator.OutputStage;

using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;
using Microsoft.CodeAnalysis;
using Sundew.Base.Collections;
using Sundew.Base.Collections.Immutable;
using Sundew.Base.Text;
using Sundew.DiscriminatedUnions.Generator;
using Sundew.DiscriminatedUnions.Generator.DeclarationStage;
using Sundew.DiscriminatedUnions.Generator.Extensions;
using Sundew.DiscriminatedUnions.Generator.Model;
using Sundew.DiscriminatedUnions.Generator.ModelStage;
using Sundew.DiscriminatedUnions.Text;
using static GeneratorConstants;

internal static class DiscriminatedUnionOutputProvider
{
    private const string Sa1601 = "SA1601";
    private const string GetPropertyDescription = "Gets the {0}";
    private const string ReturnsDescription = "The {0}";
    private const string SegregationExtensionMethodDescription = "Segregation extension method for {0}";
    private const string SegregateMethodDescription = "Segregates the items in the specified enumerable by type";
    private const string SegregateMethodReturnsDescription = "A new {0}Segregation";
    private const string FactoryMethodDescription = "Factory method for the {0} case";
    private const string FactoryPropertyDescription = "Gets the {0} case";
    private const string FactoryMethodReturnsDescription = "A new {0}";
    private const string FactoryPropertyReturnsDescription = "The {0}";
    private const string GeneratedSuffix = ".generated";
    private const string NullableEnable = "#nullable enable";
    private const string IDiscriminatedUnionTypeName = "global::Sundew.DiscriminatedUnions.IDiscriminatedUnion";
    private const string SystemTypeTypeName = "global::System.Type";
    private const string IReadonlylistOfSystemTypeTypeName = "global::System.Collections.Generic.IReadOnlyList<global::System.Type>";
    private const string GetsAllCasesInTheUnionText = "Gets all cases in the union";
    private const string? GetAllCasesReturnsText = "A readonly list of types";
    private const string CasesName = "Cases";

    public static void Generate(SourceProductionContext sourceProductionContext, (ImmutableArray<DiscriminatedUnionResult> DescriminatedUnionResults, FeatureSupport FeatureSupport) context)
    {
        var hashSet = new HashSet<string>();
        foreach (var discriminatedUnionResult in context.DescriminatedUnionResults)
        {
            sourceProductionContext.CancellationToken.ThrowIfCancellationRequested();
            if (discriminatedUnionResult.IsSuccess)
            {
                var discriminatedUnion = discriminatedUnionResult.DiscriminatedUnion;
                var discriminatedUnionNamespace = discriminatedUnion.Type.Namespace;
                var genericParametersForFileName = TryGetGenericParametersForFileName(discriminatedUnion.Type.TypeMetadata.TypeParameters);
                if (discriminatedUnion.IsPartial)
                {
                    sourceProductionContext.AddSource(
                        discriminatedUnionNamespace + GetNestedName(discriminatedUnion.Type.ContainingTypes) + '.' + discriminatedUnion.Type.Name + genericParametersForFileName + GeneratedSuffix,
                        GetUnionSource(in discriminatedUnion, discriminatedUnionNamespace, context.FeatureSupport));

                    foreach (var discriminatedUnionCase in discriminatedUnion.Cases)
                    {
                        if (hashSet.Add(discriminatedUnionCase.Type.TypeMetadata.FullName))
                        {
                            var discriminatedUnionCaseNamespace = discriminatedUnionCase.Type.Namespace;
                            var caseGenericParametersForFileName = TryGetGenericParametersForFileName(discriminatedUnionCase.Type.TypeMetadata.TypeParameters);
                            sourceProductionContext.AddSource(
                                discriminatedUnionCaseNamespace + GetNestedName(discriminatedUnionCase.Type.ContainingTypes) + '.' + discriminatedUnionCase.Type.Name + caseGenericParametersForFileName + GeneratedSuffix,
                                GetUnionCaseSource(in discriminatedUnionCase, discriminatedUnionCaseNamespace, context.FeatureSupport));
                        }
                    }
                }

                if (discriminatedUnion.GeneratorFeatures.HasFlag(GeneratorFeatures.Segregate))
                {
                    var segregationTypeName = discriminatedUnion.Type.Name + Segregation;
                    sourceProductionContext.AddSource(discriminatedUnionNamespace + '.' + segregationTypeName + genericParametersForFileName + GeneratedSuffix, GetUnionSegregationSource(in discriminatedUnion, segregationTypeName));
                    var extensionsTypeName = discriminatedUnion.Type.Name + Extensions;
                    sourceProductionContext.AddSource(
                        discriminatedUnionNamespace + '.' + extensionsTypeName + genericParametersForFileName + GeneratedSuffix,
                        GetUnionSegregateExtensionSource(in discriminatedUnion, extensionsTypeName, segregationTypeName));
                }
            }
            else
            {
                foreach (var declarationNotFound in discriminatedUnionResult.Errors)
                {
                    sourceProductionContext.ReportDiagnostic(Diagnostic.Create(Diagnostics.DiscriminatedUnionDeclarationNotFoundRule, Location.None, DiagnosticSeverity.Error, declarationNotFound.DiscriminatedUnionCaseDeclaration, declarationNotFound.Owner));
                }
            }
        }
    }

    private static string GetUnionCaseSource(
        in DiscriminatedUnionCase discriminatedUnionCase, string discriminatedUnionCaseNamespace, FeatureSupport featureSupport)
    {
        var discriminatedUnionCaseType = discriminatedUnionCase.Type;
        var baseIndentationSpace = new string(' ', discriminatedUnionCaseType.ContainingTypes.Count * 4);
        var stringBuilder = new StringBuilder();
        var nestedClassIndentation = 0;
        stringBuilder
            .Append(NullableEnable)
            .AppendLine()
            .AppendLine()
            .Append(Namespace)
            .Append(' ')
            .Append(discriminatedUnionCaseNamespace)
            .AppendLine()
            .Append('{')
            .AppendLine()
            .AppendItems(
                discriminatedUnionCaseType.ContainingTypes,
                (builder, containingType) => builder
                    .Append(' ', 4 * nestedClassIndentation)
                    .Append(SpaceIndentedBy4)
                    .AppendAccessibility(containingType.Accessibility)
                    .Append(' ')
                    .Append(Partial)
                    .Append(' ')
                    .AppendUnderlyingType(containingType.UnderlyingType)
                    .Append(' ')
                    .Append(containingType.Name)
                    .If(
                        containingType.TypeParameters.HasAny,
                        builder1 => builder1
                            .Append('<')
                            .AppendItems(
                                containingType.TypeParameters,
                                (builder1, typeParameter) => builder1.Append(typeParameter),
                                GeneratorConstants.ListSeparator)
                            .Append('>'))
                    .AppendLine()
                    .Append(' ', 4 * nestedClassIndentation++)
                    .Append(SpaceIndentedBy4)
                    .Append('{')
                    .AppendLine())
            .AppendPragmaWarning(false, Sa1601)
            .AppendTypeAttributes(false, baseIndentationSpace)
            .Append(baseIndentationSpace)
            .Append(SpaceIndentedBy4)
            .AppendAccessibility(discriminatedUnionCase.Accessibility)
            .Append(' ')
            .Append(Partial)
            .Append(' ')
            .AppendUnderlyingType(discriminatedUnionCase.UnderlyingType)
            .Append(' ')
            .AppendType(discriminatedUnionCase.Type, false, false, true)
            .AppendLine()
            .TryAppendConstraints(discriminatedUnionCase.Type.TypeMetadata.TypeParameters, SpaceIndentedBy8)
            .AppendPragmaWarning(true, Sa1601)
            .Append(baseIndentationSpace)
            .Append(SpaceIndentedBy4)
            .Append('{');

        stringBuilder
            .AppendLine()
            .AppendDocumentation(baseIndentationSpace, SpaceIndentedBy8, GetsAllCasesInTheUnionText, returns: GetAllCasesReturnsText)
            .Append(SpaceIndentedBy8)
            .Append(' ', 4 * nestedClassIndentation)
            .Append(Public)
            .Append(' ')
            .If(featureSupport.IsDefaultInterfaceMembersSupported && discriminatedUnionCase.HasImplementationTypeOwner, builder => builder.Append(New).Append(' '))
            .Append(Static)
            .Append(' ')
            .Append(IReadonlylistOfSystemTypeTypeName)
            .Append(' ')
            .Append(CasesName)
            .Append(' ')
            .Append(Get)
            .AppendLine()
            .Append(SpaceIndentedBy12)
            .Append(' ', 4 * nestedClassIndentation)
            .Append('=')
            .Append(' ')
            .Append(New)
            .Append(' ')
            .Append(SystemTypeTypeName).Append('[').Append(']').Append(' ').Append('{').Append(' ')
            .Append(Typeof).Append('(').AppendType(discriminatedUnionCase.Type).Append(')').Append(' ').Append('}')
            .Append(';')
            .AppendLine();

        stringBuilder
            .Append(baseIndentationSpace)
            .Append(SpaceIndentedBy4)
            .Append('}')
            .AppendLine()
            .AppendItems(
                discriminatedUnionCaseType.ContainingTypes,
                (builder, containingType) => builder
                    .Append(' ', 4 * --nestedClassIndentation)
                    .Append(SpaceIndentedBy4)
                    .Append('}')
                    .AppendLine())
            .Append('}')
            .AppendLine();
        return stringBuilder.ToString();
    }

    private static string GetNestedName(ValueArray<ContainingType> containingTypes)
    {
        if (!containingTypes.HasAny)
        {
            return string.Empty;
        }

        const string dot = ".";
        var stringBuilder = new StringBuilder(dot);
        stringBuilder.AppendItems(
            containingTypes,
            (builder, containingType) =>
                builder.Append(containingType.Name)
                    .If(containingType.TypeParameters.HasAny, builder1 => builder1
                        .Append('{')
                        .AppendItems(containingType.TypeParameters, (builder2, name) => builder2.Append(name), ',')
                        .Append('}')),
            '.');
        return stringBuilder.ToString();
    }

    private static string TryGetGenericParametersForFileName(ValueArray<TypeParameter> typeParameters)
    {
        if (typeParameters.IsDefault)
        {
            return string.Empty;
        }

        return new StringBuilder()
            .AppendItems(
                typeParameters,
                (sb, item) => sb.Append('{').Append(item.Name),
                (sb, item) => sb.Append(item.Name),
                sb => sb.Append('}'),
                ',').ToString();
    }

    private static string GetUnionSource(in DiscriminatedUnion discriminatedUnion, string discriminatedUnionNamespace, FeatureSupport featureSupport)
    {
        var discriminatedUnionType = discriminatedUnion.Type;
        var baseIndentationSpace = new string(' ', discriminatedUnionType.ContainingTypes.Count * 4);
        var isInterface = discriminatedUnion.UnderlyingType == UnderlyingType.Interface;
        var stringBuilder = new StringBuilder();
        var nestedClassIndentation = 0;
        stringBuilder
            .Append(NullableEnable)
            .AppendLine()
            .AppendLine()
            .Append(Namespace)
            .Append(' ')
            .Append(discriminatedUnionNamespace)
            .AppendLine()
            .Append('{')
            .AppendLine()
            .AppendItems(
                discriminatedUnionType.ContainingTypes,
                (builder, containingType) => builder
                    .Append(' ', 4 * nestedClassIndentation)
                    .Append(SpaceIndentedBy4)
                    .AppendAccessibility(containingType.Accessibility)
                    .Append(' ')
                    .Append(Partial)
                    .Append(' ')
                    .AppendUnderlyingType(containingType.UnderlyingType)
                    .Append(' ')
                    .Append(containingType.Name)
                    .If(
                        containingType.TypeParameters.HasAny,
                        builder1 => builder1
                            .Append('<')
                            .AppendItems(
                                containingType.TypeParameters,
                                (builder1, typeParameter) => builder1.Append(typeParameter),
                                GeneratorConstants.ListSeparator)
                            .Append('>'))
                    .AppendLine()
                    .Append(' ', 4 * nestedClassIndentation++)
                    .Append(SpaceIndentedBy4)
                    .Append('{')
                    .AppendLine())
            .AppendPragmaWarning(false, Sa1601)
            .AppendTypeAttributes(!isInterface, baseIndentationSpace)
            .Append(baseIndentationSpace)
            .Append(SpaceIndentedBy4)
            .AppendAccessibility(discriminatedUnion.Accessibility)
            .Append(' ')
            .Append(Partial)
            .Append(' ')
            .AppendUnderlyingType(discriminatedUnion.UnderlyingType)
            .Append(' ')
            .AppendType(discriminatedUnion.Type, false, false, true)
            .If(featureSupport.IsDefaultInterfaceMembersSupported, builder => builder.Append(' ').Append(':').Append(' ').Append(IDiscriminatedUnionTypeName))
            .AppendLine()
            .TryAppendConstraints(discriminatedUnion.Type.TypeMetadata.TypeParameters, SpaceIndentedBy8)
            .AppendPragmaWarning(true, Sa1601)
            .Append(baseIndentationSpace)
            .Append(SpaceIndentedBy4)
            .Append('{');
        foreach (var discriminatedUnionOwnedCase in discriminatedUnion.Cases)
        {
            var implementAsMethod = discriminatedUnionOwnedCase.Parameters.Any();
            stringBuilder.AppendLine()
                .AppendDocumentation(baseIndentationSpace, SpaceIndentedBy8, implementAsMethod ? FactoryMethodDescription : FactoryPropertyDescription, discriminatedUnionOwnedCase.Type.Name, default, discriminatedUnionOwnedCase.Parameters.Select(x => x.Name), implementAsMethod ? FactoryMethodReturnsDescription : FactoryPropertyReturnsDescription)
                .Append(baseIndentationSpace)
                .Append(SpaceIndentedBy8)
                .Append('[')
                .Append(SundewDiscriminatedUnionsCaseType)
                .Append('(')
                .Append(Typeof)
                .Append('(')
                .AppendType(discriminatedUnionOwnedCase.Type, true, true)
                .Append(')')
                .Append(')')
                .Append(']')
                .AppendLine()
                .If(isInterface, builder => builder.AppendDebuggerCodeAttribute(8, baseIndentationSpace))
                .Append(baseIndentationSpace)
                .Append(SpaceIndentedBy8)
                .Append(Public)
                .Append(' ');
            if (discriminatedUnion.IsConstrainingUnion)
            {
                stringBuilder.Append(New)
                    .Append(' ');
            }

            const string _Case = "_";
            var unionFactoryMethodName = discriminatedUnionOwnedCase.Type.Name;
            if (discriminatedUnionOwnedCase.HasConflictingName)
            {
                unionFactoryMethodName = _Case + unionFactoryMethodName;
            }

            var union = discriminatedUnion;
            var outsideTypeParameters = discriminatedUnionOwnedCase.Type.TypeMetadata.TypeParameters.Where(x => union.Type.TypeMetadata.TypeParameters.All(y => x.Name != y.Name)).ToArray();

            stringBuilder
                .Append(Static)
                .Append(' ')
                .AppendType(discriminatedUnionOwnedCase.ReturnType)
                .Append(' ')
                .Append(unionFactoryMethodName)
                .If(
                    !outsideTypeParameters.IsEmpty,
                    builder =>
                    {
                        return builder
                            .Append('<')
                            .AppendItems(outsideTypeParameters, (builder, typeParameter) => builder.Append(typeParameter.Name), ListSeparator)
                            .Append('>');
                    });
            if (implementAsMethod)
            {
                stringBuilder
                    .Append('(')
                    .AppendItems(
                        discriminatedUnionOwnedCase.Parameters,
                        (sb, parameter) =>
                        {
                            sb.Append(parameter.TypeName)
                                .Append(' ')
                                .Append(parameter.Name);
                            if (!string.IsNullOrEmpty(parameter.DefaultValue))
                            {
                                sb.Append(' ')
                                    .Append('=')
                                    .Append(' ')
                                    .Append(parameter.DefaultValue);
                            }
                        },
                        ListSeparator)
                    .Append(')')
                    .AppendLine()
                    .If(
                        !outsideTypeParameters.IsEmpty,
                        builder => builder.AppendItems(
                            outsideTypeParameters,
                            (builder, typeParameter) => builder
                                .Append(baseIndentationSpace)
                                .Append(SpaceIndentedBy12)
                                .Append(Where)
                                .Append(' ')
                                .Append(typeParameter.Name)
                                .Append(' ').Append(':').Append(' ')
                                .AppendItems(
                                    typeParameter.Constraints,
                                    (builder, constraint) => builder.Append(constraint),
                                    ListSeparator))
                            .AppendLine())
                    .Append(baseIndentationSpace)
                    .Append(SpaceIndentedBy12)
                    .Append(Lambda);
            }
            else
            {
                stringBuilder.Append(' ').Append(Get).AppendLine().Append(baseIndentationSpace).Append(SpaceIndentedBy12).Append('=');
            }

            stringBuilder
                .Append(' ')
                .Append(New)
                .Append(' ')
                .AppendType(discriminatedUnionOwnedCase.Type)
                .Append('(')
                .AppendItems(discriminatedUnionOwnedCase.Parameters, (sb, parameter) => sb.Append(parameter.Name), ListSeparator)
                .Append(')')
                .Append(';');
            stringBuilder.AppendLine();
        }

        stringBuilder
            .AppendLine()
            .AppendDocumentation(baseIndentationSpace, SpaceIndentedBy8, GetsAllCasesInTheUnionText, returns: GetAllCasesReturnsText)
            .Append(SpaceIndentedBy8)
            .Append(' ', 4 * nestedClassIndentation)
            .Append(Public)
            .Append(' ')
            .If(featureSupport.IsDefaultInterfaceMembersSupported && (isInterface || discriminatedUnion.IsConstrainingUnion), builder => builder.Append(New).Append(' '))
            .Append(Static)
            .Append(' ')
            .Append(IReadonlylistOfSystemTypeTypeName)
            .Append(' ')
            .Append(CasesName)
            .Append(' ')
            .Append(Get)
            .AppendLine()
            .Append(SpaceIndentedBy12)
            .Append(' ', 4 * nestedClassIndentation)
            .Append('=')
            .Append(' ')
            .Append(New)
            .Append(' ')
            .Append(SystemTypeTypeName).Append('[').Append(']').Append(' ').Append('{').Append(' ')
            .AppendItems(
                discriminatedUnion.Cases,
                (builder, x) => builder.Append(Typeof).Append('(').AppendType(x.Type).Append(')'),
                ListSeparator)
            .Append(' ')
            .Append('}')
            .Append(';')
            .AppendLine();

        stringBuilder
            .If(
                featureSupport.IsDefaultInterfaceMembersSupported && isInterface,
                builder => builder.AppendLine()
                .AppendDocumentation(baseIndentationSpace, SpaceIndentedBy8, GetsAllCasesInTheUnionText, returns: GetAllCasesReturnsText)
                .Append(SpaceIndentedBy8)
                .Append(' ', 4 * nestedClassIndentation)
                .Append(Static)
                .Append(' ')
                .Append(IReadonlylistOfSystemTypeTypeName)
                .Append(' ')
                .Append(IDiscriminatedUnionTypeName)
                .Append('.')
                .Append(CasesName)
                .Append(' ')
                .Append('=')
                .Append('>')
                .Append(' ')
                .Append(CasesName)
                .Append(';')
                .AppendLine());

        stringBuilder
            .Append(baseIndentationSpace)
            .Append(SpaceIndentedBy4)
            .Append('}')
            .AppendLine()
            .AppendItems(
                discriminatedUnionType.ContainingTypes,
                (builder, containingType) => builder
                    .Append(' ', 4 * --nestedClassIndentation)
                    .Append(SpaceIndentedBy4)
                    .Append('}')
                    .AppendLine())
            .Append('}')
            .AppendLine();
        return stringBuilder.ToString();
    }

    private static string GetUnionSegregationSource(in DiscriminatedUnion discriminatedUnion, string segregationTypeName)
    {
        var stringBuilder = new StringBuilder();
        stringBuilder.Append(NullableEnable)
            .AppendLine()
            .AppendLine()
            .Append(Namespace)
            .Append(' ')
            .Append(discriminatedUnion.Type.Namespace)
            .AppendLine()
            .Append('{')
            .AppendLine()
            .AppendDocumentation(string.Empty, SpaceIndentedBy4, $"Contains individual lists of the different cases of the discriminated union {discriminatedUnion.Type.Name}", segregationTypeName)
            .AppendTypeAttributes(true, string.Empty)
            .Append(SpaceIndentedBy4)
            .AppendAccessibility(discriminatedUnion.Accessibility)
            .Append(' ')
            .Append(Sealed)
            .Append(' ')
            .Append(Partial)
            .Append(' ')
            .Append(Class)
            .Append(' ')
            .Append(segregationTypeName)
            .TryAppendGenericQualifier(discriminatedUnion.Type)
            .AppendLine()

            .TryAppendConstraints(discriminatedUnion.Type.TypeMetadata.TypeParameters, SpaceIndentedBy8)

            .Append(SpaceIndentedBy4)
            .Append('{')
            .AppendLine()
            .Append(SpaceIndentedBy8)
            .Append(Internal)
            .Append(' ')
            .Append(segregationTypeName)
            .Append('(');
        var caseData = discriminatedUnion.Cases.Select(x => (Case: x, PropertyName: x.Type.Name.Pluralize())).Select(x => (x.Case, x.PropertyName, ParameterName: x.PropertyName.Uncapitalize().AvoidKeywordCollision())).ToArray();
        stringBuilder.AppendItems(
            caseData,
            (sb, caseItem) =>
            {
                sb.Append(SystemCollectionsGenericIReadonlyList)
                    .Append('<')
                    .AppendType(caseItem.Case.Type)
                    .Append('>')
                    .Append(' ')
                    .Append(caseItem.ParameterName);
            },
            ListSeparator)
            .Append(')')
            .AppendLine()
            .Append(SpaceIndentedBy8)
            .Append('{')
            .AppendLine();

        foreach (var discriminatedUnionOwnedCase in caseData)
        {
            stringBuilder.Append(SpaceIndentedBy12)
                .Append(This)
                .Append('.')
                .Append(discriminatedUnionOwnedCase.PropertyName)
                .Append(' ')
                .Append('=')
                .Append(' ')
                .Append(discriminatedUnionOwnedCase.ParameterName)
                .Append(';')
                .AppendLine();
        }

        stringBuilder
            .Append(SpaceIndentedBy8)
            .Append('}')
            .AppendLine()
            .AppendLine()
            .AppendItems(
            caseData,
            (sb, caseItem) =>
            {
                sb
                    .AppendDocumentation(string.Empty, SpaceIndentedBy8, GetPropertyDescription, caseItem.PropertyName, default, default, ReturnsDescription)
                    .Append(SpaceIndentedBy8)
                    .Append(Public)
                    .Append(' ')
                    .Append(SystemCollectionsGenericIReadonlyList)
                    .Append('<')
                    .AppendType(caseItem.Case.Type)
                    .Append('>')
                    .Append(' ')
                    .Append(caseItem.PropertyName)
                    .Append(' ')
                    .Append(Get);
            },
            NewLine + NewLine)
            .AppendLine()
            .Append(SpaceIndentedBy4)
            .Append('}')
            .AppendLine()
            .Append('}');
        return stringBuilder.ToString();
    }

    private static string GetUnionSegregateExtensionSource(in DiscriminatedUnion discriminatedUnion, string extensionsTypeName, string segregationTypeName)
    {
        var unionParameterName = discriminatedUnion.Type.Name.Uncapitalize();
        var unionsParameterName = unionParameterName.Pluralize();
        var stringBuilder = new StringBuilder();
        stringBuilder.Append(NullableEnable)
            .AppendLine()
            .AppendLine()
            .Append(Namespace)
            .Append(' ')
            .Append(discriminatedUnion.Type.Namespace)
            .AppendLine()
            .Append('{')
            .AppendLine()
            .AppendDocumentation(string.Empty, SpaceIndentedBy4, SegregationExtensionMethodDescription, discriminatedUnion.Type.Name)
            .AppendTypeAttributes(true, string.Empty)
            .Append(SpaceIndentedBy4)
            .AppendAccessibility(discriminatedUnion.Accessibility)
            .Append(' ')
            .Append(Static)
            .Append(' ')
            .Append(Partial)
            .Append(' ')
            .Append(Class)
            .Append(' ')
            .Append(extensionsTypeName)
            .AppendLine()
            .Append(SpaceIndentedBy4)
            .Append('{')
            .AppendLine()
            .AppendDocumentation(string.Empty, SpaceIndentedBy8, SegregateMethodDescription, discriminatedUnion.Type.Name, default, new[] { unionsParameterName }, SegregateMethodReturnsDescription)
            .Append(SpaceIndentedBy8)
            .Append(Public)
            .Append(' ')
            .Append(Static)
            .Append(' ')
            .Append(segregationTypeName)
            .TryAppendGenericQualifier(discriminatedUnion.Type)
            .Append(' ')
            .Append(Segregate)

            .TryAppendGenericQualifier(discriminatedUnion.Type)

            .Append('(')
            .Append(This)
            .Append(' ')
            .Append(SystemCollectionsGenericIEnumerable)
            .Append('<')
            .AppendType(discriminatedUnion.Type)
            .Append('>')
            .Append(' ')
            .Append(unionsParameterName)
            .Append(')')
            .AppendLine()
            .TryAppendConstraints(discriminatedUnion.Type.TypeMetadata.TypeParameters, SpaceIndentedBy12)
            .Append(SpaceIndentedBy8)
            .Append('{')
            .AppendLine();

        var caseData = discriminatedUnion.Cases.Select(x => (Case: x, VariableName: x.Type.Name.Uncapitalize(), ListVariableName: x.Type.Name.Uncapitalize().Pluralize(true).AvoidKeywordCollision())).ToArray();
        foreach (var discriminatedUnionOwnedCase in caseData)
        {
            stringBuilder.Append(SpaceIndentedBy12)
            .Append(Var)
            .Append(' ')
            .Append(discriminatedUnionOwnedCase.ListVariableName)
            .Append(' ')
            .Append('=')
            .Append(' ')
            .Append(New)
            .Append(' ')
            .Append(SystemCollectionsGenericList)
            .Append('<')
            .AppendType(discriminatedUnionOwnedCase.Case.Type)
            .Append('>')
            .Append('(')
            .Append(')')
            .Append(';')
            .AppendLine();
        }

        stringBuilder
            .AppendLine()
            .Append(SpaceIndentedBy12)
            .Append(Foreach)
            .Append(' ')
            .Append('(')
            .Append(Var)
            .Append(' ')
            .Append(Value)
            .Append(' ')
            .Append(In)
            .Append(' ')
            .Append(unionsParameterName)
            .Append(')')
            .AppendLine()
            .Append(SpaceIndentedBy12)
            .Append('{')
            .AppendLine()
            .Append(SpaceIndentedBy16)
            .Append(Switch)
            .Append(' ')
            .Append('(')
            .Append(Value)
            .Append(')')
            .AppendLine()
            .Append(SpaceIndentedBy16)
            .Append('{')
            .AppendLine();

        foreach (var discriminatedUnionOwnedCase in caseData)
        {
            stringBuilder
                .Append(SpaceIndentedBy20)
                .Append(Case)
                .Append(' ')
                .AppendType(discriminatedUnionOwnedCase.Case.Type)
                .Append(' ')
                .Append(discriminatedUnionOwnedCase.VariableName)
                .Append(':')
                .AppendLine()
                .Append(SpaceIndentedBy24)
                .Append(discriminatedUnionOwnedCase.ListVariableName)
                .Append('.')
                .Append(Add)
                .Append('(')
                .Append(discriminatedUnionOwnedCase.VariableName)
                .Append(')')
                .Append(';')
                .AppendLine()
                .Append(SpaceIndentedBy24)
                .Append(Break)
                .Append(';')
                .AppendLine();
        }

        stringBuilder
            .Append(SpaceIndentedBy16)
            .Append('}')
            .AppendLine()

            .Append(SpaceIndentedBy12)
            .Append('}')
            .AppendLine()
            .AppendLine()

            .Append(SpaceIndentedBy12)
            .Append(Return)
            .Append(' ')
            .Append(New)
            .Append(' ')
            .Append(GlobalAssemblyAlias)
            .Append(DoubleColon)
            .Append(discriminatedUnion.Type.Namespace)
            .Append('.')
            .Append(segregationTypeName)
            .TryAppendGenericQualifier(discriminatedUnion.Type)
            .Append('(')
            .AppendItems(caseData, (sb, caseItem) => sb.Append(caseItem.ListVariableName), ListSeparator)
            .Append(')')
            .Append(';')

            .AppendLine()
            .Append(SpaceIndentedBy8)
            .Append('}')
            .AppendLine()

            .Append(SpaceIndentedBy4)
            .Append('}')
            .AppendLine()
            .Append('}');
        return stringBuilder.ToString();
    }
}