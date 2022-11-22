// --------------------------------------------------------------------------------------------------------------------
// <copyright file="FactoryMethodAnalyzer.cs" company="Hukano">
// Copyright (c) Hukano. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Sundew.DiscriminatedUnions.Analyzer.FactoryMethod;

using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;

internal class FactoryMethodAnalyzer
{
    public void Analyze(SymbolAnalysisContext symbolAnalysisContext)
    {
        if (symbolAnalysisContext.Symbol is IMethodSymbol methodSymbol)
        {
            var unionType = methodSymbol.ContainingType;
            if (UnionHelper.IsDiscriminatedUnion(unionType) &&
                SymbolEqualityComparer.Default.Equals(methodSymbol.ReturnType, unionType))
            {
                var factoryMethod = methodSymbol;
                var caseTypeAttribute = factoryMethod.GetAttributes().FirstOrDefault(x =>
                    x.AttributeClass?.ToDisplayString() == typeof(CaseTypeAttribute).FullName);
                var createdCaseTypeSymbol =
                    UnionHelper.GetInstantiatedCaseTypeSymbol(factoryMethod, symbolAnalysisContext.Compilation);
                if (caseTypeAttribute != null)
                {
                    var caseTypeSymbol =
                        (INamedTypeSymbol?)caseTypeAttribute.ConstructorArguments.FirstOrDefault().Value ??
                        (INamedTypeSymbol?)caseTypeAttribute.NamedArguments.FirstOrDefault().Value.Value;
                    if (caseTypeAttribute.ApplicationSyntaxReference != null)
                    {
                        if (caseTypeSymbol is { IsGenericType: true })
                        {
                            caseTypeSymbol = caseTypeSymbol.OriginalDefinition;
                        }

                        if (createdCaseTypeSymbol is { IsGenericType: true })
                        {
                            createdCaseTypeSymbol = createdCaseTypeSymbol.OriginalDefinition;
                        }

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