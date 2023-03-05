// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DiscriminatedUnionOutputProvider.cs" company="Hukano">
// Copyright (c) Hukano. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Sundew.DiscriminatedUnions.Generator.OutputStage;

using System.Collections.Immutable;
using System.Linq;
using System.Text;
using Microsoft.CodeAnalysis;
using Sundew.Base;
using Sundew.DiscriminatedUnions.Generator;
using Sundew.DiscriminatedUnions.Generator.Extensions;
using Sundew.DiscriminatedUnions.Generator.Model;
using Sundew.DiscriminatedUnions.Generator.ModelStage;
using Sundew.DiscriminatedUnions.Text;
using static GeneratorConstants;

internal static class DiscriminatedUnionOutputProvider
{
    public static void Generate(SourceProductionContext sourceProductionContext, ImmutableArray<DiscriminatedUnionResult> discriminatedUnionResults)
    {
        foreach (var discriminatedUnionResult in discriminatedUnionResults)
        {
            sourceProductionContext.CancellationToken.ThrowIfCancellationRequested();
            if (discriminatedUnionResult.IsSuccess)
            {
                var discriminatedUnion = discriminatedUnionResult.DiscriminatedUnion;
                var discriminatedUnionNamespace = discriminatedUnion.Type.Namespace;
                if (discriminatedUnion.IsPartial)
                {
                    sourceProductionContext.AddSource(
                        discriminatedUnionNamespace + '.' + discriminatedUnion.Type.Name,
                        GetUnionSource(discriminatedUnion, discriminatedUnionNamespace));
                }

                if (discriminatedUnion.GeneratorFeatures.HasFlag(GeneratorFeatures.Segregate))
                {
                    var segregationTypeName = discriminatedUnion.Type.Name + Segregation;
                    sourceProductionContext.AddSource(discriminatedUnionNamespace + '.' + segregationTypeName, GetUnionSegregationSource(discriminatedUnion, segregationTypeName));
                    var extensionsTypeName = discriminatedUnion.Type.Name + Extensions;
                    sourceProductionContext.AddSource(
                        discriminatedUnionNamespace + '.' + discriminatedUnion.Type.Name + Extensions,
                        GetUnionSegregateExtensionSource(discriminatedUnion, extensionsTypeName, segregationTypeName));
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

    private static string GetUnionSource(DiscriminatedUnion discriminatedUnion, string discriminatedUnionNamespace)
    {
        var stringBuilder = new StringBuilder();
        stringBuilder.Append(Namespace)
            .Append(' ')
            .Append(discriminatedUnionNamespace)
            .AppendLine()
            .Append('{')
            .AppendLine()
            .AppendTypeAttributes()
            .Append(SpaceIndentedBy4)
            .Append(Partial)
            .Append(' ')
            .AppendUnderlyingType(discriminatedUnion.UnderlyingType)
            .Append(' ')
            .AppendType(discriminatedUnion.Type, false)
            .AppendLine()
            .TryAppendConstraints(discriminatedUnion.Type.TypeMetadata.TypeParameters, SpaceIndentedBy8)
            .Append(SpaceIndentedBy4)
            .Append('{');
        foreach (var discriminatedUnionOwnedCase in discriminatedUnion.Cases)
        {
            stringBuilder.AppendLine()
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

            stringBuilder.Append(Static)
                .Append(' ')
                .AppendType(discriminatedUnion.Type)
                .Append(' ')
                .Append(discriminatedUnionOwnedCase.Type.Name)
                .Append('(');

            discriminatedUnionOwnedCase.Parameters.JoinToStringBuilder(
                stringBuilder,
                (stringBuilder, parameter) =>
                {
                    stringBuilder.AppendType(parameter.Type);
                    stringBuilder.Append(' ');
                    stringBuilder.Append(parameter.Name);
                },
                ListSeparator);

            stringBuilder.Append(')')
                .Append(' ')
                .Append(Lambda)
                .Append(' ')
                .Append(New)
                .Append(' ')
                .AppendType(discriminatedUnionOwnedCase.Type)
                .Append('(');

            discriminatedUnionOwnedCase.Parameters.JoinToStringBuilder(stringBuilder, (stringBuilder, parameter) => stringBuilder.Append(parameter.Name), ListSeparator);

            stringBuilder.Append(')')
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

    private static string GetUnionSegregationSource(DiscriminatedUnion discriminatedUnion, string segregationTypeName)
    {
        var stringBuilder = new StringBuilder();
        stringBuilder.Append(Namespace)
            .Append(' ')
            .Append(discriminatedUnion.Type.Namespace)
            .AppendLine()
            .Append('{')
            .AppendLine()
            .AppendTypeAttributes()
            .Append(SpaceIndentedBy4)
            .AppendAccessibility(discriminatedUnion.Accessibility)
            .Append(' ')
            .Append(Sealed)
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
        caseData.JoinToStringBuilder(
            stringBuilder,
            (stringBuilder, caseItem) =>
            {
                stringBuilder.Append(SystemCollectionsGenericIReadonlyList)
                    .Append('<')
                    .AppendType(caseItem.Case.Type)
                    .Append('>')
                    .Append(' ')
                    .Append(caseItem.ParameterName);
            },
            ListSeparator);

        stringBuilder.Append(')')
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
            .AppendLine();

        caseData.JoinToStringBuilder(
            stringBuilder,
            (stringBuilder, caseItem) =>
            {
                stringBuilder
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
            NewLine + NewLine);

        stringBuilder.AppendLine()
            .Append(SpaceIndentedBy4)
            .Append('}')
            .AppendLine()
            .Append('}');
        return stringBuilder.ToString();
    }

    private static string GetUnionSegregateExtensionSource(DiscriminatedUnion discriminatedUnion, string extensionsTypeName, string segregationTypeName)
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
            .AppendTypeAttributes()
            .Append(SpaceIndentedBy4)
            .AppendAccessibility(discriminatedUnion.Accessibility)
            .Append(' ')
            .Append(Static)
            .Append(' ')
            .Append(Class)
            .Append(' ')
            .Append(extensionsTypeName)
            .AppendLine()

            .Append(SpaceIndentedBy4)
            .Append('{')
            .AppendLine()

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

        var caseData = discriminatedUnion.Cases.Select(x => (Case: x, VariableName: x.Type.Name.Uncapitalize(), ListVariableName: x.Type.Name.Uncapitalize().Pluralize().AvoidKeywordCollision())).ToArray();
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
            .Append(segregationTypeName)
            .TryAppendGenericQualifier(discriminatedUnion.Type)
            .Append('(');

        caseData.JoinToStringBuilder(stringBuilder, (stringBuilder, caseItem) => stringBuilder.Append(caseItem.ListVariableName), ListSeparator);

        stringBuilder.Append(')')
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