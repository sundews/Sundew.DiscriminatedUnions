// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AllCasesNotHandledCodeFixer.cs" company="Hukano">
// Copyright (c) Hukano. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Sundew.DiscriminatedUnions.CodeFixes
{
    using System;
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

            var caseTypes = DiscriminatedUnionHelper.GetAllCaseTypes(switchType);
            var handledCaseTypes = DiscriminatedUnionHelper.GetHandledCaseTypes(switchExpressionOperation);

            if (switchExpressionOperation.Syntax is not SwitchExpressionSyntax switchExpressionSyntax)
            {
                return document;
            }

            var arms = switchExpressionSyntax.Arms;
            var missingCaseTypes = caseTypes.Except<ITypeSymbol>(handledCaseTypes, SymbolEqualityComparer.Default);
            arms = arms.AddRange(missingCaseTypes.Select(missingCaseType => SyntaxFactory.SwitchExpressionArm(
                    SyntaxFactory.DeclarationPattern(
                        (TypeSyntax)generator.TypeExpression(missingCaseType),
                        SyntaxFactory.SingleVariableDesignation(
                            SyntaxFactory.ParseToken(Uncapitalize(missingCaseType.Name)))),
                    ThrowNotImplementExceptionExpression(generator))
                .WithAdditionalAnnotations(Formatter.Annotation)));

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

            var caseTypes = DiscriminatedUnionHelper.GetAllCaseTypes(switchType);
            var handledCaseTypes = DiscriminatedUnionHelper.GetHandledCaseTypes(switchOperation);

            if (switchOperation.Syntax is not SwitchStatementSyntax switchStatementSyntax)
            {
                return document;
            }

            var sections = switchStatementSyntax.Sections;
            var missingCaseTypes = caseTypes.Except<ITypeSymbol>(handledCaseTypes, SymbolEqualityComparer.Default);
            var missingCases = missingCaseTypes.Select(missingCaseType => SyntaxFactory.SwitchSection(
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
                SyntaxFactory.List(new[] { ThrowNotImplementExceptionStatement(generator), }))).ToList();
            var unionTypeInfo = semanticModel.GetTypeInfo(switchOperation.Value.Syntax);
            if (unionTypeInfo.ConvertedNullability.FlowState != NullableFlowState.NotNull &&
                !DiscriminatedUnionHelper.HasNullCase(switchOperation))
            {
                missingCases.Add(SyntaxFactory.SwitchSection(
                    SyntaxFactory.List(
                        new SwitchLabelSyntax[]
                        {
                            SyntaxFactory.CaseSwitchLabel((ExpressionSyntax)generator.NullLiteralExpression()),
                        }),
                    SyntaxFactory.List(new[] { ThrowNotImplementExceptionStatement(generator), })));
            }

            var index = sections.IndexOf(x => x.Labels.Any(x => x is DefaultSwitchLabelSyntax));
            if (index == -1)
            {
                index = sections.Count - 1;
            }

            sections = sections.InsertRange(index, missingCases);

            return document.WithSyntaxRoot(
                root.ReplaceNode(
                node,
                switchStatementSyntax.WithSections(SyntaxFactory.List(sections))
                    .WithAdditionalAnnotations(Formatter.Annotation)));
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