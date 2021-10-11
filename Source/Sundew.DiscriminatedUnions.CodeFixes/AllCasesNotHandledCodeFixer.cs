// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AllCasesNotHandledCodeFixer.cs" company="Hukano">
// Copyright (c) Hukano. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Sundew.DiscriminatedUnions.CodeFixes
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp;
    using Microsoft.CodeAnalysis.CSharp.Syntax;
    using Microsoft.CodeAnalysis.Editing;
    using Microsoft.CodeAnalysis.Formatting;
    using Microsoft.CodeAnalysis.Operations;
    using Sundew.DiscriminatedUnions.Analyzer;

    internal class AllCasesNotHandledCodeFixer : ICodeFixer
    {
        public string DiagnosticId => SundewDiscriminatedUnionsAnalyzer.AllCasesNotHandledDiagnosticId;

        public CodeFixStatus GetCodeFixState(SyntaxNode syntaxNode, SemanticModel semanticModel, CancellationToken cancellationToken)
        {
            return new CodeFixStatus.CanFix(CodeFixResources.PopulateMissingCases, nameof(AllCasesNotHandledCodeFixer));
        }

        public async Task<Document> Fix(Document document, SyntaxNode root, SyntaxNode node, SemanticModel semanticModel, CancellationToken cancellationToken)
        {
            var editor = await DocumentEditor.CreateAsync(document, cancellationToken).ConfigureAwait(false);
            var generator = editor.Generator;
            if (semanticModel.GetOperation(node) is ISwitchExpressionOperation switchExpressionOperation)
            {
                return Fix(document, root, node, semanticModel, switchExpressionOperation, generator);
            }

            if (semanticModel.GetOperation(node) is ISwitchOperation switchOperation)
            {
                return Fix(document, root, node, semanticModel, switchOperation, generator);
            }

            return document;
        }

        private static Document Fix(
            Document document,
            SyntaxNode root,
            SyntaxNode node,
            SemanticModel semanticModel,
            ISwitchExpressionOperation switchExpressionOperation,
            SyntaxGenerator generator)
        {
            var switchType = switchExpressionOperation.Value.Type;
            if (!DiscriminatedUnionHelper.IsDiscriminatedUnion(switchType))
            {
                return document;
            }

            var cases = DiscriminatedUnionHelper.GetAllCaseTypes(switchType).Select((caseType, index) => (caseType, index)).ToList();
            var handledCaseTypes = DiscriminatedUnionHelper.GetHandledCaseTypes(switchExpressionOperation);

            if (switchExpressionOperation.Syntax is not SwitchExpressionSyntax switchExpressionSyntax)
            {
                return document;
            }

            var arms = switchExpressionSyntax.Arms;
            foreach (var (missingCaseType, index) in cases)
            {
                var caseInfo = FindIndex(handledCaseTypes, missingCaseType, index);
                if (!caseInfo.WasHandled)
                {
                    arms = arms.Insert(
                        caseInfo.Index,
                        SyntaxFactory.SwitchExpressionArm(
                                SyntaxFactory.DeclarationPattern(
                                    (TypeSyntax)generator.TypeExpression(missingCaseType),
                                    SyntaxFactory.SingleVariableDesignation(
                                        SyntaxFactory.ParseToken(Uncapitalize(missingCaseType.Name)))),
                                ThrowNotImplementExceptionExpression(generator))
                            .WithAdditionalAnnotations(Formatter.Annotation));
                }
            }

            var unionTypeInfo = semanticModel.GetTypeInfo(switchExpressionOperation.Value.Syntax);
            if (unionTypeInfo.ConvertedNullability.FlowState != NullableFlowState.NotNull &&
                !DiscriminatedUnionHelper.HasNullCase(switchExpressionOperation))
            {
                arms = arms.Add(SyntaxFactory.SwitchExpressionArm(
                        SyntaxFactory.ConstantPattern(
                            (ExpressionSyntax)generator.NullLiteralExpression()),
                        ThrowNotImplementExceptionExpression(generator))
                    .WithAdditionalAnnotations(Formatter.Annotation));
            }

            var armsWithSeparator = arms.GetWithSeparators();
            if (armsWithSeparator.LastOrDefault().IsNode)
            {
                arms = SyntaxFactory.SeparatedList<SwitchExpressionArmSyntax>(
                    armsWithSeparator.Add(SyntaxFactory.Token(SyntaxKind.CommaToken)));
            }

            return document.WithSyntaxRoot(root.ReplaceNode(node, switchExpressionSyntax.WithArms(arms)));
        }

        private static Document Fix(
            Document document,
            SyntaxNode root,
            SyntaxNode node,
            SemanticModel semanticModel,
            ISwitchOperation switchOperation,
            SyntaxGenerator generator)
        {
            var switchType = switchOperation.Value.Type;
            if (!DiscriminatedUnionHelper.IsDiscriminatedUnion(switchType))
            {
                return document;
            }

            var cases = DiscriminatedUnionHelper.GetAllCaseTypes(switchType).Select((caseType, index) => (caseType, index)).ToList();
            var handledCaseTypes = DiscriminatedUnionHelper.GetHandledCaseTypes(switchOperation);

            if (switchOperation.Syntax is not SwitchStatementSyntax switchStatementSyntax)
            {
                return document;
            }

            var sections = switchStatementSyntax.Sections;
            foreach (var (missingCaseType, index) in cases)
            {
                var caseInfo = FindIndex(handledCaseTypes, missingCaseType, index);
                if (!caseInfo.WasHandled)
                {
                    sections = sections.Insert(
                        caseInfo.Index,
                        SyntaxFactory.SwitchSection(
                            SyntaxFactory.List(
                                new SwitchLabelSyntax[]
                                {
                                    SyntaxFactory.CasePatternSwitchLabel(
                                        SyntaxFactory.DeclarationPattern(
                                            (TypeSyntax)generator.TypeExpression(missingCaseType),
                                            SyntaxFactory.SingleVariableDesignation(
                                                SyntaxFactory.ParseToken(Uncapitalize(missingCaseType.Name)))),
                                        SyntaxFactory.Token(SyntaxKind.ColonToken)),
                                }),
                            SyntaxFactory.List(new[] { ThrowNotImplementExceptionStatement(generator), })));
                }
            }

            var unionTypeInfo = semanticModel.GetTypeInfo(switchOperation.Value.Syntax);
            if (unionTypeInfo.ConvertedNullability.FlowState != NullableFlowState.NotNull &&
                !DiscriminatedUnionHelper.HasNullCase(switchOperation))
            {
                sections = sections.Insert(
                    HasDefaultCase(sections) ? sections.Count - 1 : sections.Count,
                    SyntaxFactory.SwitchSection(
                    SyntaxFactory.List(
                        new SwitchLabelSyntax[]
                        {
                            SyntaxFactory.CaseSwitchLabel((ExpressionSyntax)generator.NullLiteralExpression()),
                        }),
                    SyntaxFactory.List(new[] { ThrowNotImplementExceptionStatement(generator), })));
            }

            return document.WithSyntaxRoot(
                root.ReplaceNode(
                node,
                switchStatementSyntax.WithSections(SyntaxFactory.List(sections))
                    .WithAdditionalAnnotations(Formatter.Annotation)));
        }

        private static bool HasDefaultCase(SyntaxList<SwitchSectionSyntax> sections)
        {
            return sections.LastOrDefault()?.Labels.Any(x => x is DefaultSwitchLabelSyntax) ?? false;
        }

        private static (int Index, bool WasHandled) FindIndex(IEnumerable<(ITypeSymbol Type, int Index, bool HandlesCase)> handledCases, ITypeSymbol caseType, int caseIndex)
        {
            var index = 0;
            var hadMatch = false;
            var insertionIndex = 0;
            foreach (var pair in handledCases)
            {
                var isMatch = SymbolEqualityComparer.Default.Equals(pair.Type, caseType);
                if (isMatch && pair.HandlesCase)
                {
                    return (-1, true);
                }

                if (hadMatch && !isMatch)
                {
                    insertionIndex = index + 1;
                }

                if (isMatch)
                {
                    hadMatch = true;
                }

                index++;
            }

            if (!hadMatch)
            {
                return (caseIndex, false);
            }

            return (insertionIndex, false);
        }

        private static ExpressionSyntax ThrowNotImplementExceptionExpression(SyntaxGenerator generator)
        {
            return (ExpressionSyntax)generator.ThrowExpression(generator.ObjectCreationExpression(SyntaxFactory.ParseTypeName(typeof(NotImplementedException).FullName)));
        }

        private static SyntaxNode ThrowNotImplementExceptionStatement(SyntaxGenerator generator)
        {
            return generator.ThrowStatement(generator.ObjectCreationExpression(SyntaxFactory.ParseTypeName(typeof(NotImplementedException).FullName)));
        }

        private static string Uncapitalize(string text)
        {
            var span = new Span<char>(text.ToCharArray());
            if (span.Length > 0)
            {
                span[0] = char.ToLowerInvariant(span[0]);
            }

            return span.ToString();
        }
    }
}