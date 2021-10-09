// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CaseNotSealedCodeFixer.cs" company="Hukano">
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

    internal class CaseNotSealedCodeFixer : ICodeFixer
    {
        public string DiagnosticId => SundewDiscriminatedUnionsAnalyzer.CasesShouldBeSealedDiagnosticId;

        public (string Title, string EquivalenceKey) GetNames(SyntaxNode syntaxNode, SemanticModel semanticModel)
        {
            var typeInfo = semanticModel.GetTypeInfo(syntaxNode);
            var name = string.Format(CodeFixResources.SealCase, typeInfo.Type?.Name ?? "unknown error");
            return (name, name);
        }

        public async Task<Document> Fix(Document document, SyntaxNode root, SyntaxNode node, SemanticModel semanticModel, CancellationToken cancellationToken)
        {
            var editor = await DocumentEditor.CreateAsync(document, cancellationToken).ConfigureAwait(false);
            var generator = editor.Generator;
            var declaration = generator.WithModifiers(node, generator.GetModifiers(node).WithIsSealed(true)).WithAdditionalAnnotations(Formatter.Annotation);
            var newNode = root.ReplaceNode(node, declaration);
            return document.WithSyntaxRoot(newNode);
        }
    }
}