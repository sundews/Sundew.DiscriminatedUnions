// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DiscriminatedUnionOutputProvider.cs" company="Sundews">
// Copyright (c) Sundews. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Sundew.DiscriminatedUnions.Generator.OutputStage;

using System.Collections.Immutable;
using System.Linq;
using System.Text;
using Microsoft.CodeAnalysis;
using Sundew.Base;
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

    public static void Generate(SourceProductionContext sourceProductionContext, ImmutableArray<DiscriminatedUnionResult> discriminatedUnionResults)
    {
        foreach (var discriminatedUnionResult in discriminatedUnionResults)
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
                        discriminatedUnionNamespace + '.' + discriminatedUnion.Type.Name + genericParametersForFileName,
                        GetUnionSource(in discriminatedUnion, discriminatedUnionNamespace));
                }

                if (discriminatedUnion.GeneratorFeatures.HasFlag(GeneratorFeatures.Segregate))
                {
                    var segregationTypeName = discriminatedUnion.Type.Name + Segregation;
                    sourceProductionContext.AddSource(discriminatedUnionNamespace + '.' + segregationTypeName + genericParametersForFileName, GetUnionSegregationSource(in discriminatedUnion, segregationTypeName));
                    var extensionsTypeName = discriminatedUnion.Type.Name + Extensions;
                    sourceProductionContext.AddSource(
                        discriminatedUnionNamespace + '.' + extensionsTypeName + genericParametersForFileName,
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

    private static string GetUnionSource(in DiscriminatedUnion discriminatedUnion, string discriminatedUnionNamespace)
    {
        var stringBuilder = new StringBuilder();
        stringBuilder.Append(Namespace)
            .Append(' ')
            .Append(discriminatedUnionNamespace)
            .AppendLine()
            .Append('{')
            .AppendLine()
            .AppendPragmaWarning(false, Sa1601)
            .AppendTypeAttributes()
            .Append(SpaceIndentedBy4)
            .AppendAccessibility(discriminatedUnion.Accessibility)
            .Append(' ')
            .Append(Partial)
            .Append(' ')
            .AppendUnderlyingType(discriminatedUnion.UnderlyingType)
            .Append(' ')
            .AppendType(discriminatedUnion.Type, false)
            .AppendLine()
            .TryAppendConstraints(discriminatedUnion.Type.TypeMetadata.TypeParameters, SpaceIndentedBy8)
            .AppendPragmaWarning(true, Sa1601)
            .Append(SpaceIndentedBy4)
            .Append('{');
        foreach (var discriminatedUnionOwnedCase in discriminatedUnion.Cases.OrderBy(x => x.Parameters.Any()).ThenBy(x => x.Type.Name))
        {
            var implementAsMethod = discriminatedUnionOwnedCase.Parameters.Any();
            stringBuilder.AppendLine()
                .AppendDocumentation(SpaceIndentedBy8, implementAsMethod ? FactoryMethodDescription : FactoryPropertyDescription, discriminatedUnionOwnedCase.Type.Name, default, discriminatedUnionOwnedCase.Parameters.Select(x => x.Name), implementAsMethod ? FactoryMethodReturnsDescription : FactoryPropertyReturnsDescription)
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
                .Append(SpaceIndentedBy8)
                .Append(Public)
                .Append(' ');
            if (discriminatedUnion.IsConstrainingUnion)
            {
                stringBuilder.Append(New)
                    .Append(' ');
            }

            const string Case = "Case";
            var unionFactoryMethodName = discriminatedUnionOwnedCase.Type.Name;
            if (discriminatedUnionOwnedCase.HasConflictingName)
            {
                unionFactoryMethodName += Case;
            }

            stringBuilder.Append(Static)
                .Append(' ')
                .AppendType(discriminatedUnion.Type)
                .Append(' ')
                .Append(unionFactoryMethodName);
            if (implementAsMethod)
            {
                stringBuilder.Append('(')
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
                    .Append(' ')
                    .Append(Lambda);
            }
            else
            {
                stringBuilder.Append(' ').Append(Get).Append(' ').Append('=');
            }

            stringBuilder.Append(' ')
                .Append(New)
                .Append(' ')
                .AppendType(discriminatedUnionOwnedCase.Type)
                .Append('(')
                .AppendItems(discriminatedUnionOwnedCase.Parameters, (sb, parameter) => sb.Append(parameter.Name), ListSeparator)
                .Append(')')
                .Append(';');
            stringBuilder.AppendLine();
        }

        stringBuilder.Append(SpaceIndentedBy4)
            .Append('}')
            .AppendLine()
            .Append('}')
            .AppendLine();
        return stringBuilder.ToString();
    }

    private static string GetUnionSegregationSource(in DiscriminatedUnion discriminatedUnion, string segregationTypeName)
    {
        var stringBuilder = new StringBuilder();
        stringBuilder.Append(Namespace)
            .Append(' ')
            .Append(discriminatedUnion.Type.Namespace)
            .AppendLine()
            .Append('{')
            .AppendLine()
            .AppendDocumentation(SpaceIndentedBy4, $"Contains individual lists of the different cases of the discriminated union {discriminatedUnion.Type.Name}", segregationTypeName)
            .AppendTypeAttributes()
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
                    .AppendDocumentation(SpaceIndentedBy8, GetPropertyDescription, caseItem.PropertyName, default, default, ReturnsDescription)
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
        stringBuilder.Append(Namespace)
            .Append(' ')
            .Append(discriminatedUnion.Type.Namespace)
            .AppendLine()
            .Append('{')
            .AppendLine()
            .AppendDocumentation(SpaceIndentedBy4, SegregationExtensionMethodDescription, discriminatedUnion.Type.Name)
            .AppendTypeAttributes()
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
            .AppendDocumentation(SpaceIndentedBy8, SegregateMethodDescription, discriminatedUnion.Type.Name, default, new[] { unionsParameterName }, SegregateMethodReturnsDescription)
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