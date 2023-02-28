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
                if (discriminatedUnion.IsPartial)
                {
                    sourceProductionContext.AddSource(
                        discriminatedUnion.Type.Namespace + '.' + discriminatedUnion.Type.Name,
                        GetUnionSource(discriminatedUnion));
                }

                if (discriminatedUnion.GeneratorFeatures.HasFlag(GeneratorFeatures.Segregate))
                {
                    var segregationTypeName = discriminatedUnion.Type.Name + Segregation;
                    sourceProductionContext.AddSource(discriminatedUnion.Type.Namespace + '.' + segregationTypeName, GetUnionSegregationSource(discriminatedUnion, segregationTypeName));
                    var extensionsTypeName = discriminatedUnion.Type.Name + Extensions;
                    sourceProductionContext.AddSource(
                        discriminatedUnion.Type.Namespace + '.' + discriminatedUnion.Type.Name + Extensions,
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

    private static string GetUnionSource(DiscriminatedUnion discriminatedUnion)
    {
        var stringBuilder = new StringBuilder();
        stringBuilder.Append(Namespace);
        stringBuilder.Append(' ');
        stringBuilder.Append(discriminatedUnion.Type.Namespace);
        stringBuilder.AppendLine();
        stringBuilder.Append('{');
        stringBuilder.AppendLine();
        stringBuilder.Append(SpaceIndentedBy4);
        stringBuilder.Append(Partial);
        stringBuilder.Append(' ');
        stringBuilder.AppendUnderlyingType(discriminatedUnion.UnderlyingType);
        stringBuilder.Append(' ');
        stringBuilder.Append(discriminatedUnion.Type.Name);
        stringBuilder.AppendLine();
        stringBuilder.Append(SpaceIndentedBy4);
        stringBuilder.Append('{');
        foreach (var discriminatedUnionOwnedCase in discriminatedUnion.Cases)
        {
            stringBuilder.AppendLine();
            stringBuilder.Append(SpaceIndentedBy8);
            stringBuilder.Append('[');
            stringBuilder.Append(SundewDiscriminatedUnionsCaseType);
            stringBuilder.Append('(');
            stringBuilder.Append(Typeof);
            stringBuilder.Append('(');
            stringBuilder.AppendType(discriminatedUnionOwnedCase.Type);
            stringBuilder.Append(')');
            stringBuilder.Append(')');
            stringBuilder.Append(']');
            stringBuilder.AppendLine();
            stringBuilder.Append(SpaceIndentedBy8);
            stringBuilder.Append(Public);
            stringBuilder.Append(' ');
            if (discriminatedUnion.IsConstrainingUnion)
            {
                stringBuilder.Append(New);
                stringBuilder.Append(' ');
            }

            stringBuilder.Append(Static);
            stringBuilder.Append(' ');
            stringBuilder.AppendType(discriminatedUnion.Type);
            stringBuilder.Append(' ');
            stringBuilder.Append(discriminatedUnionOwnedCase.Type.Name);
            stringBuilder.Append('(');

            discriminatedUnionOwnedCase.Parameters.JoinToStringBuilder(
                stringBuilder,
                (stringBuilder, parameter) =>
                {
                    stringBuilder.AppendType(parameter.Type);
                    stringBuilder.Append(' ');
                    stringBuilder.Append(parameter.Name);
                },
                ListSeparator);

            stringBuilder.Append(')');
            stringBuilder.Append(' ');
            stringBuilder.Append(Lambda);
            stringBuilder.Append(' ');
            stringBuilder.Append(New);
            stringBuilder.Append(' ');
            stringBuilder.AppendType(discriminatedUnionOwnedCase.Type);
            stringBuilder.Append('(');

            discriminatedUnionOwnedCase.Parameters.JoinToStringBuilder(stringBuilder, (stringBuilder, parameter) => stringBuilder.Append(parameter.Name), ListSeparator);

            stringBuilder.Append(')');
            stringBuilder.Append(';');
            stringBuilder.AppendLine();
        }

        stringBuilder.Append(SpaceIndentedBy4);
        stringBuilder.Append('}');
        stringBuilder.AppendLine();
        stringBuilder.Append('}');
        stringBuilder.AppendLine();
        return stringBuilder.ToString();
    }

    private static string GetUnionSegregationSource(DiscriminatedUnion discriminatedUnion, string segregationTypeName)
    {
        var stringBuilder = new StringBuilder();
        stringBuilder.Append(Namespace);
        stringBuilder.Append(' ');
        stringBuilder.Append(discriminatedUnion.Type.Namespace);
        stringBuilder.AppendLine();
        stringBuilder.Append('{');
        stringBuilder.AppendLine();
        stringBuilder.Append(SpaceIndentedBy4);
        stringBuilder.AppendAccessibility(discriminatedUnion.Accessibility);
        stringBuilder.Append(' ');
        stringBuilder.Append(Sealed);
        stringBuilder.Append(' ');
        stringBuilder.Append(Class);
        stringBuilder.Append(' ');
        stringBuilder.Append(segregationTypeName);
        stringBuilder.AppendLine();
        stringBuilder.Append(SpaceIndentedBy4);
        stringBuilder.Append('{');
        stringBuilder.AppendLine();

        stringBuilder.Append(SpaceIndentedBy8);
        stringBuilder.Append(Internal);
        stringBuilder.Append(' ');
        stringBuilder.Append(segregationTypeName);
        stringBuilder.Append('(');
        var caseData = discriminatedUnion.Cases.Select(x => (Case: x, PropertyName: x.Type.Name.Pluralize())).Select(x => (x.Case, x.PropertyName, ParameterName: x.PropertyName.Uncapitalize().AvoidKeywordCollision())).ToArray();
        caseData.JoinToStringBuilder(
            stringBuilder,
            (stringBuilder, caseItem) =>
            {
                stringBuilder.Append(SystemCollectionsGenericIReadonlyList);
                stringBuilder.Append('<');
                stringBuilder.AppendType(caseItem.Case.Type);
                stringBuilder.Append('>');
                stringBuilder.Append(' ');
                stringBuilder.Append(caseItem.ParameterName);
            },
            ListSeparator);

        stringBuilder.Append(')');
        stringBuilder.AppendLine();
        stringBuilder.Append(SpaceIndentedBy8);
        stringBuilder.Append('{');
        stringBuilder.AppendLine();

        foreach (var discriminatedUnionOwnedCase in caseData)
        {
            stringBuilder.Append(SpaceIndentedBy12);
            stringBuilder.Append(This);
            stringBuilder.Append('.');
            stringBuilder.Append(discriminatedUnionOwnedCase.PropertyName);
            stringBuilder.Append(' ');
            stringBuilder.Append('=');
            stringBuilder.Append(' ');
            stringBuilder.Append(discriminatedUnionOwnedCase.ParameterName);
            stringBuilder.Append(';');
            stringBuilder.AppendLine();
        }

        stringBuilder.Append(SpaceIndentedBy8);
        stringBuilder.Append('}');
        stringBuilder.AppendLine();
        stringBuilder.AppendLine();

        caseData.JoinToStringBuilder(
            stringBuilder,
            (stringBuilder, caseItem) =>
            {
                stringBuilder.Append(SpaceIndentedBy8);
                stringBuilder.Append(Public);
                stringBuilder.Append(' ');
                stringBuilder.Append(SystemCollectionsGenericIReadonlyList);
                stringBuilder.Append('<');
                stringBuilder.AppendType(caseItem.Case.Type);
                stringBuilder.Append('>');
                stringBuilder.Append(' ');
                stringBuilder.Append(caseItem.PropertyName);
                stringBuilder.Append(' ');
                stringBuilder.Append(Get);
            },
            NewLine + NewLine);

        stringBuilder.AppendLine();
        stringBuilder.Append(SpaceIndentedBy4);
        stringBuilder.Append('}');
        stringBuilder.AppendLine();
        stringBuilder.Append('}');
        return stringBuilder.ToString();
    }

    private static string GetUnionSegregateExtensionSource(DiscriminatedUnion discriminatedUnion, string extensionsTypeName, string segregationTypeName)
    {
        var unionParameterName = discriminatedUnion.Type.Name.Uncapitalize();
        var unionsParameterName = unionParameterName.Pluralize();
        var stringBuilder = new StringBuilder();
        stringBuilder.Append(Namespace);
        stringBuilder.Append(' ');
        stringBuilder.Append(discriminatedUnion.Type.Namespace);
        stringBuilder.AppendLine();
        stringBuilder.Append('{');
        stringBuilder.AppendLine();
        stringBuilder.Append(SpaceIndentedBy4);
        stringBuilder.AppendAccessibility(discriminatedUnion.Accessibility);
        stringBuilder.Append(' ');
        stringBuilder.Append(Static);
        stringBuilder.Append(' ');
        stringBuilder.Append(Class);
        stringBuilder.Append(' ');
        stringBuilder.Append(extensionsTypeName);
        stringBuilder.AppendLine();
        stringBuilder.Append(SpaceIndentedBy4);
        stringBuilder.Append('{');
        stringBuilder.AppendLine();

        stringBuilder.Append(SpaceIndentedBy8);
        stringBuilder.Append(Public);
        stringBuilder.Append(' ');
        stringBuilder.Append(Static);
        stringBuilder.Append(' ');
        stringBuilder.Append(segregationTypeName);
        stringBuilder.Append(' ');
        stringBuilder.Append(Segregate);
        stringBuilder.Append('(');
        stringBuilder.Append(This);
        stringBuilder.Append(' ');
        stringBuilder.Append(SystemCollectionsGenericIEnumerable);
        stringBuilder.Append('<');
        stringBuilder.AppendType(discriminatedUnion.Type);
        stringBuilder.Append('>');
        stringBuilder.Append(' ');
        stringBuilder.Append(unionsParameterName);
        stringBuilder.Append(')');
        stringBuilder.AppendLine();
        stringBuilder.Append(SpaceIndentedBy8);
        stringBuilder.Append('{');
        stringBuilder.AppendLine();

        var caseData = discriminatedUnion.Cases.Select(x => (Case: x, VariableName: x.Type.Name.Uncapitalize(), ListVariableName: x.Type.Name.Uncapitalize().Pluralize().AvoidKeywordCollision())).ToArray();
        foreach (var discriminatedUnionOwnedCase in caseData)
        {
            stringBuilder.Append(SpaceIndentedBy12);
            stringBuilder.Append(Var);
            stringBuilder.Append(' ');
            stringBuilder.Append(discriminatedUnionOwnedCase.ListVariableName);
            stringBuilder.Append(' ');
            stringBuilder.Append('=');
            stringBuilder.Append(' ');
            stringBuilder.Append(New);
            stringBuilder.Append(' ');
            stringBuilder.Append(SystemCollectionsGenericList);
            stringBuilder.Append('<');
            stringBuilder.AppendType(discriminatedUnionOwnedCase.Case.Type);
            stringBuilder.Append('>');
            stringBuilder.Append('(');
            stringBuilder.Append(')');
            stringBuilder.Append(';');
            stringBuilder.AppendLine();
        }

        stringBuilder.AppendLine();
        stringBuilder.Append(SpaceIndentedBy12);
        stringBuilder.Append(Foreach);
        stringBuilder.Append(' ');
        stringBuilder.Append('(');
        stringBuilder.Append(Var);
        stringBuilder.Append(' ');
        stringBuilder.Append(Value);
        stringBuilder.Append(' ');
        stringBuilder.Append(In);
        stringBuilder.Append(' ');
        stringBuilder.Append(unionsParameterName);
        stringBuilder.Append(')');
        stringBuilder.AppendLine();
        stringBuilder.Append(SpaceIndentedBy12);
        stringBuilder.Append('{');
        stringBuilder.AppendLine();
        stringBuilder.Append(SpaceIndentedBy16);
        stringBuilder.Append(Switch);
        stringBuilder.Append(' ');
        stringBuilder.Append('(');
        stringBuilder.Append(Value);
        stringBuilder.Append(')');
        stringBuilder.AppendLine();
        stringBuilder.Append(SpaceIndentedBy16);
        stringBuilder.Append('{');
        stringBuilder.AppendLine();

        foreach (var discriminatedUnionOwnedCase in caseData)
        {
            stringBuilder.Append(SpaceIndentedBy20);
            stringBuilder.Append(Case);
            stringBuilder.Append(' ');
            stringBuilder.AppendType(discriminatedUnionOwnedCase.Case.Type);
            stringBuilder.Append(' ');
            stringBuilder.Append(discriminatedUnionOwnedCase.VariableName);
            stringBuilder.Append(':');
            stringBuilder.AppendLine();
            stringBuilder.Append(SpaceIndentedBy24);
            stringBuilder.Append(discriminatedUnionOwnedCase.ListVariableName);
            stringBuilder.Append('.');
            stringBuilder.Append(Add);
            stringBuilder.Append('(');
            stringBuilder.Append(discriminatedUnionOwnedCase.VariableName);
            stringBuilder.Append(')');
            stringBuilder.Append(';');
            stringBuilder.AppendLine();
            stringBuilder.Append(SpaceIndentedBy24);
            stringBuilder.Append(Break);
            stringBuilder.Append(';');
            stringBuilder.AppendLine();
        }

        stringBuilder.Append(SpaceIndentedBy16);
        stringBuilder.Append('}');
        stringBuilder.AppendLine();

        stringBuilder.Append(SpaceIndentedBy12);
        stringBuilder.Append('}');
        stringBuilder.AppendLine();
        stringBuilder.AppendLine();

        stringBuilder.Append(SpaceIndentedBy12);
        stringBuilder.Append(Return);
        stringBuilder.Append(' ');
        stringBuilder.Append(New);
        stringBuilder.Append(' ');
        stringBuilder.Append(segregationTypeName);
        stringBuilder.Append('(');

        caseData.JoinToStringBuilder(stringBuilder, (stringBuilder, caseItem) => stringBuilder.Append(caseItem.ListVariableName), ListSeparator);

        stringBuilder.Append(')');
        stringBuilder.Append(';');

        stringBuilder.AppendLine();
        stringBuilder.Append(SpaceIndentedBy8);
        stringBuilder.Append('}');
        stringBuilder.AppendLine();

        stringBuilder.Append(SpaceIndentedBy4);
        stringBuilder.Append('}');
        stringBuilder.AppendLine();
        stringBuilder.Append('}');
        return stringBuilder.ToString();
    }
}