// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SundewDiscriminatedUnionsCodeFixProvider.cs" company="Hukano">
// Copyright (c) Hukano. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Sundew.DiscriminatedUnions
{
    using System;
    using System.Collections.Generic;
    using System.Collections.Immutable;
    using System.Composition;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CodeActions;
    using Microsoft.CodeAnalysis.CodeFixes;
    using Microsoft.CodeAnalysis.CSharp;
    using Microsoft.CodeAnalysis.CSharp.Syntax;
    using Microsoft.CodeAnalysis.Rename;
    using Microsoft.CodeAnalysis.Text;
    using Sundew.DiscriminatedUnions.Analyzer;

    /// <summary>
    /// Code fix for diagnostics related to discriminated unions.
    /// </summary>
    /// <seealso cref="Microsoft.CodeAnalysis.CodeFixes.CodeFixProvider" />
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(SundewDiscriminatedUnionsCodeFixProvider))]
    [Shared]
    public class SundewDiscriminatedUnionsCodeFixProvider : CodeFixProvider
    {
        /// <summary>
        /// Gets the list of diagnostic IDs that this provider can provide fixes for.
        /// </summary>
        public sealed override ImmutableArray<string> FixableDiagnosticIds => ImmutableArray.Create(SundewDiscriminatedUnionsAnalyzer.AllCasesNotHandledDiagnosticId, SundewDiscriminatedUnionsAnalyzer.SwitchShouldThrowInDefaultCaseDiagnosticId);

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
            var root = await context.Document.GetSyntaxRootAsync(context.CancellationToken).ConfigureAwait(false);
        }
    }
}
