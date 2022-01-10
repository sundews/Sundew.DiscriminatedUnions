// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PopulateFactoryMethodsCodeFixer.cs" company="Hukano">
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

internal class PopulateFactoryMethodsCodeFixer : ICodeFixer
{
    public string DiagnosticId => PopulateUnionFactoryMethodsMarkerAnalyzer.PopulateFactoryMethodsDiagnosticId;

    public CodeFixStatus GetCodeFixState(
        SyntaxNode syntaxNode,
        SemanticModel semanticModel,
        Diagnostic diagnostic,
        CancellationToken cancellationToken)
    {
        var declaredSymbol = semanticModel.GetDeclaredSymbol(syntaxNode, cancellationToken);
        if (declaredSymbol == null)
        {
            return new CodeFixStatus.CannotFix();
        }

        var name = string.Format(CodeFixResources.CreateFactoryMethods, declaredSymbol.Name);
        return new CodeFixStatus.CanFix(
            name,
            nameof(PopulateFactoryMethodsCodeFixer));
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
        var index = generator.GetMembers(node).Count;
        if (semanticModel.GetDeclaredSymbol(node) is not INamedTypeSymbol unionType)
        {
            return document;
        }

        var factoryMethods = GetCaseTypesWithMissingFactoryMethods(unionType, semanticModel.Compilation)
            .Select((caseType, index) =>
                {
                    var constructor = caseType.Constructors.OrderByDescending(x => x.Parameters.Length).FirstOrDefault();
                    if (constructor == null)
                    {
                        return null;
                    }

                    var parameters = constructor.Parameters.Select(x =>
                    {
                        var name = x.Name.Uncapitalize();
                        return (Parameter: generator.WithName(generator.ParameterDeclaration(x), name), name);
                    }).ToList();

                    return GetFactoryMethod(
                            semanticModel,
                            unionType,
                            caseType.Name,
                            constructor,
                            caseType,
                            constructor.Parameters.Select(x => x.Type).ToArray(),
                            parameters,
                            generator,
                            index != 0)
                        .WithAdditionalAnnotations(Formatter.Annotation);
                }).Where(x => x != null).ToArray();

        if (!factoryMethods.Any())
        {
            return document;
        }

        var newNode = generator.InsertMembers(
            node,
            index,
            factoryMethods);
        return document.WithSyntaxRoot(root.ReplaceNode(node, newNode));
    }

    private static IEnumerable<INamedTypeSymbol> GetCaseTypesWithMissingFactoryMethods(
        INamedTypeSymbol unionType,
        Compilation compilation)
    {
        var knownCaseTypes = UnionHelper.GetKnownCaseTypes(unionType, compilation);
        var allCaseTypes = GetDerivedTypes(compilation, unionType);
        return allCaseTypes.Except(knownCaseTypes);
    }

    private static SyntaxNode GetFactoryMethod(
        SemanticModel semanticModel,
        INamedTypeSymbol unionType,
        string name,
        IMethodSymbol constructor,
        INamedTypeSymbol caseType,
        IReadOnlyCollection<ITypeSymbol> parameterTypes,
        List<(SyntaxNode Parameter, string Name)> parameters,
        SyntaxGenerator generator,
        bool addAdditionalNewLine)
    {
        if (semanticModel.Language == LanguageNames.CSharp)
        {
            var tokenList = SyntaxFactory.TokenList(
                SyntaxFactory.Token(SyntaxKind.PublicKeyword));
            if (HasInheritedSameFactoryMethodSignature(unionType, caseType, parameterTypes))
            {
                tokenList = tokenList.Add(SyntaxFactory.Token(SyntaxKind.NewKeyword));
            }

            tokenList = tokenList.Add(SyntaxFactory.Token(SyntaxKind.StaticKeyword));
            var syntaxNode = SyntaxFactory.MethodDeclaration(
                SyntaxFactory.List<AttributeListSyntax>(),
                tokenList,
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
                                    .WithType(SyntaxFactory.ParseTypeName(
                                        x.Type.ToDisplayString(SymbolDisplayFormat.MinimallyQualifiedFormat)));
                            }))),
                SyntaxFactory.List<TypeParameterConstraintClauseSyntax>(),
                null,
                SyntaxFactory.ArrowExpressionClause(
                    SyntaxFactory.ObjectCreationExpression(
                        SyntaxFactory.ParseTypeName(
                            caseType.ToDisplayString(SymbolDisplayFormat.MinimallyQualifiedFormat)),
                        SyntaxFactory.ArgumentList(
                            SyntaxFactory.SeparatedList(
                                parameters.Select(x =>
                                    SyntaxFactory.Argument(SyntaxFactory.IdentifierName(x.Name))))),
                        null)),
                SyntaxFactory.Token(SyntaxKind.SemicolonToken));

            if (addAdditionalNewLine)
            {
                return syntaxNode.WithLeadingTrivia(SyntaxFactory.CarriageReturnLineFeed, SyntaxFactory.CarriageReturnLineFeed);
            }

            return syntaxNode.WithLeadingTrivia(SyntaxFactory.CarriageReturnLineFeed);
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
                        parameterTypes.Select(x => generator.IdentifierName(x.Name)))),
            });
    }

    private static bool HasInheritedSameFactoryMethodSignature(INamedTypeSymbol unionType, INamedTypeSymbol caseType, IReadOnlyCollection<ITypeSymbol> parameterTypes)
    {
        return unionType.EnumerateBaseTypes().Concat(unionType.AllInterfaces)
            .SelectMany(x => x.GetMembers(caseType.Name)
                .Where(x => x.IsStatic && x.Kind == SymbolKind.Method)
                .OfType<IMethodSymbol>()
                .Where(x => x.Parameters.Select(x => x.Type).SequenceEqual(parameterTypes, SymbolEqualityComparer.Default))).Any();
    }

    private static IEnumerable<INamedTypeSymbol> GetDerivedTypes(Compilation compilation, ITypeSymbol baseClassTypeSymbol) =>
        GetAllTypes(compilation.GlobalNamespace, baseClassTypeSymbol);

    private static IEnumerable<INamedTypeSymbol> GetAllTypes(INamespaceSymbol @namespace, ITypeSymbol baseClassTypeSymbol)
    {
        foreach (var type in @namespace.GetTypeMembers())
        {
            foreach (var nestedType in GetNestedTypes(type))
            {
                if (!nestedType.IsAbstract && nestedType.TypeKind != TypeKind.Interface && CanBeAssignedTo(nestedType, baseClassTypeSymbol))
                {
                    yield return nestedType;
                }
            }
        }

        foreach (var nestedNamespace in @namespace.GetNamespaceMembers())
        {
            foreach (var type in GetAllTypes(nestedNamespace, baseClassTypeSymbol))
            {
                yield return type;
            }
        }
    }

    private static IEnumerable<INamedTypeSymbol> GetNestedTypes(INamedTypeSymbol type)
    {
        yield return type;
        foreach (var memberType in type.GetTypeMembers())
        {
            foreach (var nestedType in GetNestedTypes(memberType))
            {
                yield return nestedType;
            }
        }
    }

    private static bool CanBeAssignedTo(ITypeSymbol? typeSymbol, ITypeSymbol assignmentTypeSymbol)
    {
        while (typeSymbol != null)
        {
            if (SymbolEqualityComparer.Default.Equals(typeSymbol, assignmentTypeSymbol))
            {
                return true;
            }

            var any = typeSymbol.Interfaces.Any(x => CanBeAssignedTo(x, assignmentTypeSymbol));
            if (any)
            {
                return true;
            }

            typeSymbol = typeSymbol.BaseType;
        }

        return false;
    }
}