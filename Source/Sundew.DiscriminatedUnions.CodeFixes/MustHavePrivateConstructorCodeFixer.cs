// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MustHavePrivateConstructorCodeFixer.cs" company="Hukano">
// Copyright (c) Hukano. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Sundew.DiscriminatedUnions.CodeFixes
{
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp;
    using Microsoft.CodeAnalysis.Editing;
    using Microsoft.CodeAnalysis.Formatting;
    using Sundew.DiscriminatedUnions.Analyzer;

    internal class MustHavePrivateConstructorCodeFixer : ICodeFixer
    {
        public string DiagnosticId => SundewDiscriminatedUnionsAnalyzer.MustHavePrivateConstructorDiagnosticId;

        public CodeFixStatus GetCodeFixState(SyntaxNode syntaxNode, SemanticModel semanticModel, CancellationToken cancellationToken)
        {
            var name = CodeFixResources.CreatePrivateDefaultConstructor;
            return new CodeFixStatus.CanFix(name, nameof(MustHavePrivateConstructorCodeFixer));
        }

        public async Task<Document> Fix(Document document, SyntaxNode root, SyntaxNode node, SemanticModel semanticModel, CancellationToken cancellationToken)
        {
            var documentEditor = await DocumentEditor.CreateAsync(document, cancellationToken);
            var generator = documentEditor.Generator;
            var declaredSymbol = semanticModel.GetDeclaredSymbol(node);
            var name = declaredSymbol?.Name;
            if (name == null)
            {
                return document;
            }

            var index = GetMemberInsertionIndex(generator.GetMembers(node));
            var newNode = generator.InsertMembers(node, index, generator.ConstructorDeclaration(name, null, Accessibility.Private)).WithAdditionalAnnotations(Formatter.Annotation);
            return document.WithSyntaxRoot(root.ReplaceNode(node, newNode));
        }

        private static int GetMemberInsertionIndex(IReadOnlyList<SyntaxNode> members)
        {
            var i = 0;
            for (; i < members.Count; i++)
            {
                var member = members[i];
                if (!member.IsKind(SyntaxKind.FieldDeclaration))
                {
                    return i;
                }
            }

            return i;
        }
    }
}