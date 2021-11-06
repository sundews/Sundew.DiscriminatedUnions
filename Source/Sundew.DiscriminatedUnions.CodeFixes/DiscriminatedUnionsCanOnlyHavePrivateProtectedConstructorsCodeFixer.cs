﻿// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DiscriminatedUnionsCanOnlyHavePrivateProtectedConstructorsCodeFixer.cs" company="Hukano">
// Copyright (c) Hukano. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Sundew.DiscriminatedUnions.CodeFixes
{
    using System.Collections.Immutable;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.Editing;
    using Microsoft.CodeAnalysis.Formatting;
    using Sundew.DiscriminatedUnions.Analyzer;

    internal class DiscriminatedUnionsCanOnlyHavePrivateProtectedConstructorsCodeFixer : ICodeFixer
    {
        public string DiagnosticId => SundewDiscriminatedUnionsAnalyzer
            .DiscriminatedUnionsCanOnlyHavePrivateProtectedConstructorsRule.Id;

        public CodeFixStatus GetCodeFixState(
            SyntaxNode syntaxNode,
            SemanticModel semanticModel,
            Diagnostic diagnostic,
            CancellationToken cancellationToken)
        {
            return new CodeFixStatus.CanFix(
                CodeFixResources.CanOnlyHavePrivateProtectedConstructors,
                nameof(DiscriminatedUnionsCanOnlyHavePrivateProtectedConstructorsCodeFixer));
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
            var declaration = generator.WithAccessibility(node, Accessibility.ProtectedAndInternal)
                .WithAdditionalAnnotations(Formatter.Annotation);
            var newNode = root.ReplaceNode(node, declaration);
            return document.WithSyntaxRoot(newNode);
        }
    }
}