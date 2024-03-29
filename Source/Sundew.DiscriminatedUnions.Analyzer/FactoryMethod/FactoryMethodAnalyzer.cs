﻿// --------------------------------------------------------------------------------------------------------------------
// <copyright file="FactoryMethodAnalyzer.cs" company="Sundews">
// Copyright (c) Sundews. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Sundew.DiscriminatedUnions.Analyzer.FactoryMethod;

using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using Sundew.DiscriminatedUnions.Shared;

internal class FactoryMethodAnalyzer
{
    public void Analyze(SymbolAnalysisContext symbolAnalysisContext)
    {
        if (symbolAnalysisContext.Symbol is IMethodSymbol methodSymbol)
        {
            var unionType = methodSymbol.ContainingType;
            if (unionType.IsDiscriminatedUnion() &&
                SymbolEqualityComparer.Default.Equals(methodSymbol.ReturnType, unionType))
            {
                var factoryMethod = methodSymbol;
                var caseTypeAttribute = factoryMethod.GetAttributes().FirstOrDefault(x =>
                    x.AttributeClass?.ToDisplayString() == typeof(CaseTypeAttribute).FullName);
                var createdCaseTypeSymbol =
                    UnionHelper.GetInstantiatedCaseTypeSymbol(factoryMethod, symbolAnalysisContext.Compilation);
                if (caseTypeAttribute != null)
                {
                    var caseTypeSymbol = UnionHelper.GetEquatableCaseTypeForUnionType(caseTypeAttribute, createdCaseTypeSymbol);
                    if (caseTypeAttribute.ApplicationSyntaxReference != null)
                    {
                        if (!SymbolEqualityComparer.Default.Equals(caseTypeSymbol, createdCaseTypeSymbol))
                        {
                            var incorrectFactoryMethodSyntaxReference =
                                factoryMethod.DeclaringSyntaxReferences.FirstOrDefault(x =>
                                    x.SyntaxTree == caseTypeAttribute.ApplicationSyntaxReference
                                        .SyntaxTree);
                            if (incorrectFactoryMethodSyntaxReference != null)
                            {
                                symbolAnalysisContext.ReportDiagnostic(Diagnostic.Create(
                                    DiscriminatedUnionsAnalyzer
                                        .FactoryMethodShouldHaveMatchingCaseTypeAttributeRule,
                                    incorrectFactoryMethodSyntaxReference.GetSyntax().GetLocation(),
                                    new[] { caseTypeAttribute.ApplicationSyntaxReference.GetSyntax().GetLocation() },
                                    factoryMethod,
                                    createdCaseTypeSymbol));
                            }
                        }
                    }
                }
                else
                {
                    if (factoryMethod.DeclaringSyntaxReferences.Any(x =>
                        {
                            var syntaxNode = x.GetSyntax();
                            if (!(syntaxNode is MethodDeclarationSyntax methodDeclarationSyntax))
                            {
                                return false;
                            }

                            return UnionHelper.GetInstantiatedCaseTypeSymbol(factoryMethod, symbolAnalysisContext.Compilation) != null;
                        }))
                    {
                        foreach (var declaringSyntaxReference in factoryMethod.DeclaringSyntaxReferences)
                        {
                            symbolAnalysisContext.ReportDiagnostic(Diagnostic.Create(
                                DiscriminatedUnionsAnalyzer
                                    .FactoryMethodShouldHaveMatchingCaseTypeAttributeRule,
                                declaringSyntaxReference.GetSyntax().GetLocation(),
                                factoryMethod,
                                createdCaseTypeSymbol));
                        }
                    }
                }
            }
        }
    }
}