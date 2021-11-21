// --------------------------------------------------------------------------------------------------------------------
// <copyright file="UnnestedCasesShouldHaveFactoryMethodCodeFixer.cs" company="Hukano">
// Copyright (c) Hukano. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Sundew.DiscriminatedUnions.CodeFixes;

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
using Sundew.DiscriminatedUnions.Analyzer;

internal class UnnestedCasesShouldHaveFactoryMethodCodeFixer : ICodeFixer
{
    public string DiagnosticId => DimensionalUnionsAnalyzer.UnnestedCasesShouldHaveFactoryMethodDiagnosticId;

    public CodeFixStatus GetCodeFixState(
        SyntaxNode syntaxNode,
        SemanticModel semanticModel,
        Diagnostic diagnostic,
        CancellationToken cancellationToken)
    {
        if (diagnostic.Properties.TryGetKey(DiagnosticPropertyNames.Name, out var name) && name != null)
        {
            return new CodeFixStatus.CanFix(
                string.Format(CodeFixResources.CreateFactoryMethod, name),
                nameof(UnnestedCasesShouldHaveFactoryMethodCodeFixer));
        }

        return new CodeFixStatus.CannotFix();
    }

    public async Task<Document> Fix(
        Document document,
        SyntaxNode root,
        SyntaxNode node,
        ImmutableDictionary<string, string?> diagnosticProperties,
        SemanticModel semanticModel,
        CancellationToken cancellationToken)
    {
        if (!diagnosticProperties.TryGetValue(DiagnosticPropertyNames.QualifiedCaseName, out var discriminatedUnionCase) ||
            discriminatedUnionCase == null)
        {
            return document;
        }

        if (!diagnosticProperties.TryGetValue(DiagnosticPropertyNames.Name, out var name) ||
            name == null)
        {
            return document;
        }

        var editor = await DocumentEditor.CreateAsync(document, cancellationToken).ConfigureAwait(false);
        var generator = editor.Generator;
        var index = generator.GetMembers(node).Count;
        if (semanticModel.GetDeclaredSymbol(node) is not INamedTypeSymbol unionType)
        {
            return document;
        }

        var caseType = semanticModel.Compilation.GetTypeByMetadataName(discriminatedUnionCase);
        if (caseType == null)
        {
            return document;
        }

        var constructor = caseType.Constructors.OrderByDescending(x => x.Parameters.Length).FirstOrDefault();
        if (constructor == null)
        {
            return document;
        }

        var parameters = constructor.Parameters.Select(x =>
        {
            var name = x.Name.Uncapitalize();
            return (Parameter: generator.WithName(generator.ParameterDeclaration(x), name), name);
        }).ToList();

        var factoryMethod = GetFactoryMethod(semanticModel, unionType, name, constructor, caseType, parameters, generator);
        var newNode = generator.InsertMembers(
            node,
            index,
            factoryMethod.WithAdditionalAnnotations(Formatter.Annotation));
        return document.WithSyntaxRoot(root.ReplaceNode(node, newNode));
    }

    private static SyntaxNode GetFactoryMethod(
        SemanticModel semanticModel,
        INamedTypeSymbol unionType,
        string name,
        IMethodSymbol constructor,
        INamedTypeSymbol caseType,
        List<(SyntaxNode Parameter, string Name)>? parameters,
        SyntaxGenerator generator)
    {
        if (semanticModel.Language == LanguageNames.CSharp)
        {
            return SyntaxFactory.MethodDeclaration(
                    SyntaxFactory.List<AttributeListSyntax>(),
                    SyntaxFactory.TokenList(
                        SyntaxFactory.Token(SyntaxKind.PublicKeyword),
                        SyntaxFactory.Token(SyntaxKind.StaticKeyword)),
                    SyntaxFactory.ParseTypeName(unionType.ToDisplayString(SymbolDisplayFormat.MinimallyQualifiedFormat)),
                    null,
                    SyntaxFactory.Identifier(name),
                    null,
                    SyntaxFactory.ParameterList(
                        SyntaxFactory.SeparatedList<ParameterSyntax>()
                            .AddRange(
                                constructor.Parameters.Select(x =>
                                {
                                    name = x.Name.Uncapitalize();
                                    return SyntaxFactory.Parameter(SyntaxFactory.Identifier(name))
                                        .WithType(SyntaxFactory.ParseTypeName(x.Type.ToDisplayString(SymbolDisplayFormat.MinimallyQualifiedFormat)));
                                }))),
                    SyntaxFactory.List<TypeParameterConstraintClauseSyntax>(),
                    null,
                    SyntaxFactory.ArrowExpressionClause(
                        SyntaxFactory.ObjectCreationExpression(
                            SyntaxFactory.ParseTypeName(caseType.ToDisplayString(SymbolDisplayFormat.MinimallyQualifiedFormat)),
                            SyntaxFactory.ArgumentList(
                                SyntaxFactory.SeparatedList(
                                    parameters.Select(x =>
                                        SyntaxFactory.Argument(SyntaxFactory.IdentifierName(x.Name))))),
                            null)),
                    SyntaxFactory.Token(SyntaxKind.SemicolonToken))
                .WithLeadingTrivia(SyntaxFactory.CarriageReturnLineFeed);
        }

        return generator.MethodDeclaration(
            name,
            parameters.Select(x => x.Parameter),
            null,
            generator.TypeExpression(unionType),
            Accessibility.Public,
            DeclarationModifiers.Static,
            new[]
            {
                generator.ReturnStatement(
                    generator.ObjectCreationExpression(
                        caseType,
                        parameters.Select(x => generator.IdentifierName(x.Name)))),
            });
    }
}