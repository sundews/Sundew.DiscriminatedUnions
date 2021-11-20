// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DiscriminatedUnionsMustBeAbstractCodeFixer.cs" company="Hukano">
// Copyright (c) Hukano. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Sundew.DiscriminatedUnions.CodeFixes;

using System.Collections.Immutable;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Editing;
using Microsoft.CodeAnalysis.Formatting;
using Sundew.DiscriminatedUnions.Analyzer;

internal class DiscriminatedUnionsMustBeAbstractCodeFixer : ICodeFixer
{
    public string DiagnosticId => SundewDiscriminatedUnionsAnalyzer.ClassDiscriminatedUnionsMustBeAbstractRule.Id;

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

        var name = string.Format(CodeFixResources.MakeAbstract, declaredSymbol.Name);
        return new CodeFixStatus.CanFix(
            name,
            nameof(DiscriminatedUnionsMustBeAbstractCodeFixer));
    }

    public async Task<Document> Fix(
        Document document,
        SyntaxNode root,
        SyntaxNode node,
        ImmutableDictionary<string, string?> diagnosticProperties,
        SemanticModel semanticModel,
        CancellationToken cancellationToken)
    {
        var editor = await DocumentEditor.CreateAsync(document, cancellationToken).ConfigureAwait(false);
        var generator = editor.Generator;
        var declaration = generator.WithModifiers(node, generator.GetModifiers(node).WithIsAbstract(true)).WithAdditionalAnnotations(Formatter.Annotation);
        var newNode = root.ReplaceNode(node, declaration);
        return document.WithSyntaxRoot(newNode);
    }
}