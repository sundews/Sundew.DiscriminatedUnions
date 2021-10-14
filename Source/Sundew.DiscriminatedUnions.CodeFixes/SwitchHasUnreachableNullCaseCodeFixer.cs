// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SwitchHasUnreachableNullCaseCodeFixer.cs" company="Hukano">
// Copyright (c) Hukano. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Sundew.DiscriminatedUnions.CodeFixes
{
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp;
    using Microsoft.CodeAnalysis.CSharp.Syntax;
    using Sundew.DiscriminatedUnions.Analyzer;

    internal class SwitchHasUnreachableNullCaseCodeFixer : ICodeFixer
    {
        public string DiagnosticId => SundewDiscriminatedUnionsAnalyzer.HasUnreachableNullCaseDiagnosticId;

        public CodeFixStatus GetCodeFixState(SyntaxNode syntaxNode, SemanticModel semanticModel, CancellationToken cancellationToken)
        {
            return new CodeFixStatus.CanFix(
                CodeFixResources.HasUnreachableNullCase,
                nameof(SwitchHasUnreachableNullCaseCodeFixer));
        }

        public Task<Document> Fix(Document document, SyntaxNode root, SyntaxNode node, SemanticModel semanticModel, CancellationToken cancellationToken)
        {
            if (node is SwitchExpressionArmSyntax { Parent: SwitchExpressionSyntax switchExpressionSyntax } switchExpressionArmSyntax)
            {
                var arms = SyntaxFactory.SeparatedList(switchExpressionSyntax.Arms.Remove(switchExpressionArmSyntax));
                var armsWithSeparator = arms.GetWithSeparators();
                if (armsWithSeparator.LastOrDefault().IsNode)
                {
                    arms = SyntaxFactory.SeparatedList<SwitchExpressionArmSyntax>(
                        armsWithSeparator.Add(SyntaxFactory.Token(SyntaxKind.CommaToken)));
                }

                return Task.FromResult(
                    document.WithSyntaxRoot(
                        root.ReplaceNode(switchExpressionSyntax, switchExpressionSyntax.WithArms(arms))));
            }

            if (node is SwitchSectionSyntax { Parent: SwitchStatementSyntax switchStatementSyntax } switchSectionSyntax)
            {
                var newSwitchStatementSyntax = switchStatementSyntax.RemoveNode(switchSectionSyntax, SyntaxRemoveOptions.KeepDirectives);
                if (newSwitchStatementSyntax != null)
                {
                    return Task.FromResult(
                        document.WithSyntaxRoot(
                            root.ReplaceNode(switchStatementSyntax, newSwitchStatementSyntax)));
                }
            }

            return Task.FromResult(document);
        }
    }
}