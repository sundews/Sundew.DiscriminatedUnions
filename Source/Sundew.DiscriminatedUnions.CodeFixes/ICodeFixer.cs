// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ICodeFixer.cs" company="Hukano">
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

    internal interface ICodeFixer
    {
        string DiagnosticId { get; }

        CodeFixStatus GetCodeFixState(
            SyntaxNode syntaxNode,
            SemanticModel semanticModel,
            Diagnostic diagnostic,
            CancellationToken cancellationToken);

        Task<Document> Fix(
            Document document,
            SyntaxNode root,
            SyntaxNode node,
            ImmutableDictionary<string, string?> diagnosticProperties,
            SemanticModel semanticModel,
            CancellationToken cancellationToken);
    }
}