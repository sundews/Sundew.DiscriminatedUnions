﻿// --------------------------------------------------------------------------------------------------------------------
// <copyright file="UnionCaseAnalyzer.cs" company="Sundews">
// Copyright (c) Sundews. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Sundew.DiscriminatedUnions.Analyzer.Analyzers;

using System;
using System.Collections.Immutable;
using System.Linq;
using Microsoft.CodeAnalysis;
using Sundew.DiscriminatedUnions.Shared;

internal class UnionCaseAnalyzer : IUnionSymbolAnalyzer
{
    public void AnalyzeSymbol(INamedTypeSymbol namedTypeSymbol, Action<Diagnostic> reportDiagnostic)
    {
        var isCase = false;
        if (!namedTypeSymbol.IsAbstract && namedTypeSymbol.TypeKind != TypeKind.Interface)
        {
            foreach (var baseType in namedTypeSymbol.EnumerateBaseTypes().Concat(namedTypeSymbol.AllInterfaces))
            {
                if (baseType.IsDiscriminatedUnion())
                {
                    var caseNamedTypeSymbol = namedTypeSymbol;
                    isCase = true;
                    if (caseNamedTypeSymbol.ContainingType == null)
                    {
                        var hasFactoryMethod = UnionHelper.GetFactoryMethodSymbols(baseType)
                            .Any(x =>
                                (x.Name == caseNamedTypeSymbol.Name || SymbolEqualityComparer.Default.Equals(UnionHelper.TryGetCaseType(x.Attributes), caseNamedTypeSymbol)));
                        if (!hasFactoryMethod)
                        {
                            var propertyBuilder = ImmutableDictionary.CreateBuilder<string, string?>();
                            propertyBuilder.Add(DiagnosticPropertyNames.QualifiedCaseName, caseNamedTypeSymbol.ToDisplayString());
                            propertyBuilder.Add(DiagnosticPropertyNames.Name, caseNamedTypeSymbol.Name);
                            foreach (var syntaxReference in caseNamedTypeSymbol.DeclaringSyntaxReferences)
                            {
                                reportDiagnostic(Diagnostic.Create(
                                    DiscriminatedUnionsAnalyzer.UnnestedCasesShouldHaveFactoryMethodRule,
                                    syntaxReference.GetSyntax().GetLocation(),
                                    DiagnosticSeverity.Error,
                                    baseType.DeclaringSyntaxReferences.Select(x => x.GetSyntax().GetLocation()),
                                    propertyBuilder.ToImmutable(),
                                    caseNamedTypeSymbol,
                                    baseType));
                            }
                        }
                    }

                    if (!SymbolEqualityComparer.Default.Equals(caseNamedTypeSymbol.ContainingAssembly, baseType.ContainingAssembly))
                    {
                        foreach (var declaringSyntaxReference in caseNamedTypeSymbol.DeclaringSyntaxReferences)
                        {
                            reportDiagnostic(Diagnostic.Create(
                                DiscriminatedUnionsAnalyzer.CasesMustBeDeclaredInUnionAssemblyRule,
                                declaringSyntaxReference.GetSyntax().GetLocation(),
                                caseNamedTypeSymbol,
                                baseType.ContainingAssembly.Name));
                        }
                    }
                }
            }
        }

        if (isCase && !namedTypeSymbol.IsSealed)
        {
            foreach (var declaringSyntaxReference in namedTypeSymbol.DeclaringSyntaxReferences)
            {
                reportDiagnostic(Diagnostic.Create(
                    DiscriminatedUnionsAnalyzer.CasesShouldBeSealedRule,
                    declaringSyntaxReference.GetSyntax().GetLocation(),
                    namedTypeSymbol));
            }
        }
    }
}