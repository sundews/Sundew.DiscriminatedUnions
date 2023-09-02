// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SwitchAllCasesNotHandledCodeFixer.cs" company="Sundews">
// Copyright (c) Sundews. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Sundew.DiscriminatedUnions.CodeFixes;

using System;
using System.Collections.Generic;
using System.Collections.Immutable;
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
using Sundew.DiscriminatedUnions.Analyzer.SwitchExpression;
using Sundew.DiscriminatedUnions.Analyzer.SwitchStatement;
using Sundew.DiscriminatedUnions.CodeFixes.Collections;
using Sundew.DiscriminatedUnions.Shared;
using Sundew.DiscriminatedUnions.Text;

internal class SwitchAllCasesNotHandledCodeFixer : ICodeFixer
{
    public string DiagnosticId => DiscriminatedUnionsAnalyzer.SwitchAllCasesNotHandledDiagnosticId;

    public CodeFixStatus GetCodeFixState(
        SyntaxNode syntaxNode,
        SemanticModel semanticModel,
        Diagnostic diagnostic,
        CancellationToken cancellationToken)
    {
        return new CodeFixStatus.CanFix(CodeFixResources.PopulateMissingCases, nameof(SwitchAllCasesNotHandledCodeFixer));
    }

    public async Task<Document> Fix(
        Document document,
        SyntaxNode root,
        SyntaxNode node,
        IReadOnlyList<Location> additionalLocations,
        ImmutableDictionary<string, string?> diagnosticProperties,
        SemanticModel semanticModel,
        CancellationToken cancellationToken)
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
        if (!switchType.IsDiscriminatedUnion())
        {
            return document;
        }

        var cases = UnionHelper.GetKnownCaseTypes(switchType).Pair().ToList();
        var handledCaseTypes = SwitchExpressionHelper.GetHandledCaseTypes(switchExpressionOperation).ToList();

        if (switchExpressionOperation.Syntax is not SwitchExpressionSyntax switchExpressionSyntax)
        {
            return document;
        }

        var arms = switchExpressionSyntax.Arms;
        foreach (var (previousCaseType, missingCaseType) in cases)
        {
            var (index, wasHandled) = FindIndex(handledCaseTypes, missingCaseType, previousCaseType);
            if (wasHandled)
            {
                continue;
            }

            handledCaseTypes.Insert(index, new CaseInfo { HandlesCase = true, Type = missingCaseType });
            arms = arms.Insert(
                index,
                SyntaxFactory.SwitchExpressionArm(
                        SyntaxFactory.DeclarationPattern(
                            (TypeSyntax)generator.TypeExpression(missingCaseType),
                            SyntaxFactory.SingleVariableDesignation(
                                SyntaxFactory.ParseToken(missingCaseType.Name.Uncapitalize()))),
                        ThrowNotImplementExceptionExpression(generator))
                    .WithAdditionalAnnotations(Formatter.Annotation));
        }

        var unionTypeInfo = semanticModel.GetTypeInfo(switchExpressionOperation.Value.Syntax);
        if (unionTypeInfo.ConvertedNullability.FlowState != NullableFlowState.NotNull &&
            SwitchExpressionHelper.GetNullCase(switchExpressionOperation) == null)
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
        if (!switchType.IsDiscriminatedUnion())
        {
            return document;
        }

        var cases = UnionHelper.GetKnownCaseTypes(switchType).Pair().ToList();
        var handledCaseTypes = SwitchStatementHelper.GetHandledCaseTypes(switchOperation).ToList();

        if (switchOperation.Syntax is not SwitchStatementSyntax switchStatementSyntax)
        {
            return document;
        }

        var sections = switchStatementSyntax.Sections;
        foreach (var (previousCaseType, missingCaseType) in cases)
        {
            var (index, wasHandled) = FindIndex(handledCaseTypes, missingCaseType, previousCaseType);
            if (wasHandled)
            {
                continue;
            }

            handledCaseTypes.Insert(index, new CaseInfo { HandlesCase = true, Type = missingCaseType });
            sections = sections.Insert(
                index,
                SyntaxFactory.SwitchSection(
                    SyntaxFactory.List(
                        new SwitchLabelSyntax[]
                        {
                            SyntaxFactory.CasePatternSwitchLabel(
                                SyntaxFactory.DeclarationPattern(
                                    (TypeSyntax)generator.TypeExpression(missingCaseType),
                                    SyntaxFactory.SingleVariableDesignation(
                                        SyntaxFactory.ParseToken(missingCaseType.Name.Uncapitalize()))),
                                SyntaxFactory.Token(SyntaxKind.ColonToken)),
                        }),
                    SyntaxFactory.List(new[] { ThrowNotImplementExceptionStatement(generator), })));
        }

        var unionTypeInfo = semanticModel.GetTypeInfo(switchOperation.Value.Syntax);
        if (unionTypeInfo.ConvertedNullability.FlowState != NullableFlowState.NotNull &&
            SwitchStatementHelper.GetNullCase(switchOperation) == null)
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

    private static (int Index, bool WasHandled) FindIndex(
        List<CaseInfo> handledCases,
        ITypeSymbol caseType,
        ITypeSymbol? previousCaseType)
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
                insertionIndex = index;
            }

            if (isMatch)
            {
                hadMatch = true;
            }

            index++;
        }

        if (!hadMatch)
        {
            return previousCaseType == null ?
                (0, false) :
                (handledCases.FindIndex((x) => SymbolEqualityComparer.Default.Equals(x.Type, previousCaseType)) + 1, false);
        }

        return (insertionIndex, false);
    }

    private static ExpressionSyntax ThrowNotImplementExceptionExpression(SyntaxGenerator generator)
    {
        return (ExpressionSyntax)generator.ThrowExpression(generator.ObjectCreationExpression(SyntaxFactory.ParseTypeName(typeof(NotImplementedException).FullName)));
    }

    private static StatementSyntax ThrowNotImplementExceptionStatement(SyntaxGenerator generator)
    {
        return (StatementSyntax)generator.ThrowStatement(generator.ObjectCreationExpression(SyntaxFactory.ParseTypeName(typeof(NotImplementedException).FullName)));
    }
}