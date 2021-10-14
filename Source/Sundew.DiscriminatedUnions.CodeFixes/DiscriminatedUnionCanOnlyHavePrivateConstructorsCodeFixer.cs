// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DiscriminatedUnionCanOnlyHavePrivateConstructorsCodeFixer.cs" company="Hukano">
// Copyright (c) Hukano. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Sundew.DiscriminatedUnions.CodeFixes
{
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.Editing;
    using Microsoft.CodeAnalysis.Formatting;
    using Sundew.DiscriminatedUnions.Analyzer;

    internal class DiscriminatedUnionCanOnlyHavePrivateConstructorsCodeFixer : ICodeFixer
    {
        public string DiagnosticId => SundewDiscriminatedUnionsAnalyzer.DiscriminatedUnionCanOnlyHavePrivateConstructorsDiagnosticId;

        public CodeFixStatus GetCodeFixState(SyntaxNode syntaxNode, SemanticModel semanticModel, CancellationToken cancellationToken)
        {
            return new CodeFixStatus.CanFix(
                CodeFixResources.DiscriminatedUnionCanOnlyHavePrivateConstructors,
                nameof(DiscriminatedUnionCanOnlyHavePrivateConstructorsCodeFixer));
        }

        public async Task<Document> Fix(Document document, SyntaxNode root, SyntaxNode node, SemanticModel semanticModel, CancellationToken cancellationToken)
        {
            var editor = await DocumentEditor.CreateAsync(document, cancellationToken).ConfigureAwait(false);
            var generator = editor.Generator;
            var declaration = generator.WithAccessibility(node, Accessibility.Private).WithAdditionalAnnotations(Formatter.Annotation);
            var newNode = root.ReplaceNode(node, declaration);
            return document.WithSyntaxRoot(newNode);
        }
    }
}