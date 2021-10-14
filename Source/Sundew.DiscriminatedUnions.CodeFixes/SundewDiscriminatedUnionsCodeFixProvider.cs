// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SundewDiscriminatedUnionsCodeFixProvider.cs" company="Hukano">
// Copyright (c) Hukano. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Sundew.DiscriminatedUnions.CodeFixes
{
    using System.Collections.Generic;
    using System.Collections.Immutable;
    using System.Composition;
    using System.Linq;
    using System.Threading.Tasks;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CodeActions;
    using Microsoft.CodeAnalysis.CodeFixes;

    /// <summary>
    /// Code fix for diagnostics related to discriminated unions.
    /// </summary>
    /// <seealso cref="Microsoft.CodeAnalysis.CodeFixes.CodeFixProvider" />
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(SundewDiscriminatedUnionsCodeFixProvider))]
    [Shared]
    public class SundewDiscriminatedUnionsCodeFixProvider : CodeFixProvider
    {
        private readonly Dictionary<string, ICodeFixer> codeFixers;

        /// <summary>
        /// Initializes a new instance of the <see cref="SundewDiscriminatedUnionsCodeFixProvider"/> class.
        /// </summary>
        public SundewDiscriminatedUnionsCodeFixProvider()
        {
            this.codeFixers = new ICodeFixer[]
            {
                new AllCasesNotHandledCodeFixer(),
                new SwitchShouldNotHaveDefaultCaseCodeFixer(),
                new CaseNotSealedCodeFixer(),
                new MustHavePrivateConstructorCodeFixer(),
                new SwitchHasUnreachableNullCaseCodeFixer(),
                new DiscriminatedUnionCanOnlyHavePrivateConstructorsCodeFixer(),
            }.ToDictionary(x => x.DiagnosticId);
            this.FixableDiagnosticIds = ImmutableArray.CreateRange(this.codeFixers.Keys);
        }

        /// <summary>
        /// Gets the list of diagnostic IDs that this provider can provide fixes for.
        /// </summary>
        public sealed override ImmutableArray<string> FixableDiagnosticIds { get; }

        /// <summary>
        /// Gets an optional <see cref="T:Microsoft.CodeAnalysis.CodeFixes.FixAllProvider" /> that can fix all/multiple occurrences of diagnostics fixed by this code fix provider.
        /// Return null if the provider doesn't support fix all/multiple occurrences.
        /// Otherwise, you can return any of the well known fix all providers from <see cref="T:Microsoft.CodeAnalysis.CodeFixes.WellKnownFixAllProviders" /> or implement your own fix all provider.
        /// </summary>
        /// <returns>A fix all provider.</returns>
        public sealed override FixAllProvider GetFixAllProvider()
        {
            // See https://github.com/dotnet/roslyn/blob/master/docs/analyzers/FixAllProvider.md for more information on Fix All Providers
            return WellKnownFixAllProviders.BatchFixer;
        }

        /// <summary>
        /// Computes one or more fixes for the specified <see cref="T:Microsoft.CodeAnalysis.CodeFixes.CodeFixContext" />.
        /// </summary>
        /// <param name="context">A <see cref="T:Microsoft.CodeAnalysis.CodeFixes.CodeFixContext" /> containing context information about the diagnostics to fix.
        /// The context must only contain diagnostics with a <see cref="P:Microsoft.CodeAnalysis.Diagnostic.Id" /> included in the <see cref="P:Microsoft.CodeAnalysis.CodeFixes.CodeFixProvider.FixableDiagnosticIds" /> for the current provider.</param>
        /// <returns>An async task.</returns>
        public sealed override async Task RegisterCodeFixesAsync(CodeFixContext context)
        {
            var diagnostic = context.Diagnostics.First();
            var document = context.Document;
            var root = await document.GetSyntaxRootAsync(context.CancellationToken).ConfigureAwait(false);
            if (root == null)
            {
                return;
            }

            var node = root.FindNode(diagnostic.Location.SourceSpan);
            if (this.codeFixers.TryGetValue(diagnostic.Id, out var codeFixer))
            {
                var semanticModel = await document.GetSemanticModelAsync(context.CancellationToken).ConfigureAwait(false);
                if (semanticModel == null)
                {
                    return;
                }

                var codeFixState = codeFixer.GetCodeFixState(node, semanticModel, context.CancellationToken);
                switch (codeFixState)
                {
                    case CodeFixStatus.CanFix canFix:
                        context.RegisterCodeFix(
                            CodeAction.Create(
                                canFix.Title,
                                cancellationToken => codeFixer.Fix(document, root, node, semanticModel, cancellationToken),
                                canFix.EquivalenceKey),
                            diagnostic);
                        return;
                    case CodeFixStatus.CannotFix cannotFix:
                        break;
                }
            }
        }
    }
}
