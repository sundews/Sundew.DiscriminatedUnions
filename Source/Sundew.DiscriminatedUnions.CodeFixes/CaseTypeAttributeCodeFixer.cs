// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CaseTypeAttributeCodeFixer.cs" company="Hukano">
// Copyright (c) Hukano. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Sundew.DiscriminatedUnions.CodeFixes;

using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Editing;
using Sundew.DiscriminatedUnions.Analyzer;

internal class CaseTypeAttributeCodeFixer : ICodeFixer
{
    public string DiagnosticId => DiscriminatedUnionsAnalyzer.FactoryMethodShouldHaveMatchingCaseTypeAttributeDiagnosticId;

    public CodeFixStatus GetCodeFixState(
        SyntaxNode syntaxNode,
        SemanticModel semanticModel,
        Diagnostic diagnostic,
        CancellationToken cancellationToken)
    {
        var declaredSymbol = semanticModel.GetDeclaredSymbol(syntaxNode, cancellationToken);
        if (declaredSymbol == null)
        {
            return new CodeFixStatus.CannotFix();
        }

        var name = string.Format(CodeFixResources.InsertCorrectCaseTypeAttribute, declaredSymbol.Name);
        return new CodeFixStatus.CanFix(
            name,
            nameof(CaseTypeAttributeCodeFixer));
    }

    public async Task<Document> Fix(
        Document document,
        SyntaxNode root,
        SyntaxNode node,
        IReadOnlyList<Location> additionalLocations,
        ImmutableDictionary<string, string?> diagnosticProperties,
        SemanticModel semanticModel,
        CancellationToken cancellationToken)
    {
        var editor = await DocumentEditor.CreateAsync(document, cancellationToken).ConfigureAwait(false);
        var generator = editor.Generator;
        if (semanticModel.GetDeclaredSymbol(node) is IMethodSymbol factoryMethod)
        {
            var attributes = factoryMethod.GetAttributes();
            var attributeList = attributes.Select(x => x.ApplicationSyntaxReference?.GetSyntax()).Where(x => x != null).Select(x => x!).ToImmutableArray();
            var caseSyntaxReference = attributes.FirstOrDefault(x => x.AttributeClass?.ToDisplayString() == typeof(CaseTypeAttribute).FullName)?.ApplicationSyntaxReference;
            var existingCaseTypeSyntaxNode = caseSyntaxReference != null ? await caseSyntaxReference.GetSyntaxAsync(cancellationToken) : default;

            var attributeCaseType = UnionHelper.GetInstantiatedCaseTypeSymbol(factoryMethod, semanticModel.Compilation);
            if (attributeCaseType != null)
            {
                var newCaseTypeAttributeSyntaxNode = SyntaxFactory.Attribute(
                SyntaxFactory.ParseName(typeof(CaseTypeAttribute).FullName),
                SyntaxFactory.AttributeArgumentList(
                    SyntaxFactory.SingletonSeparatedList(
                        SyntaxFactory.AttributeArgument(SyntaxFactory.TypeOfExpression(
                            SyntaxFactory.ParseTypeName(
                                attributeCaseType.ToDisplayString(SymbolDisplayFormat
                                    .MinimallyQualifiedFormat)))))));
                if (existingCaseTypeSyntaxNode != null)
                {
                    attributeList = attributeList.Replace(existingCaseTypeSyntaxNode, newCaseTypeAttributeSyntaxNode);
                }
                else
                {
                    attributeList = attributeList.Add(newCaseTypeAttributeSyntaxNode);
                }

                var newNode = generator.RemoveAllAttributes(node);
                newNode = generator.AddAttributes(newNode, attributeList);
                return document.WithSyntaxRoot(root.ReplaceNode(node, newNode));
            }
        }

        return document;
    }
}