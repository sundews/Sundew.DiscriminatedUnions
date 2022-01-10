// --------------------------------------------------------------------------------------------------------------------
// <copyright file="UnionCaseAnalyzer.cs" company="Hukano">
// Copyright (c) Hukano. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Sundew.DiscriminatedUnions.Analyzer.Analyzers;

using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using Microsoft.CodeAnalysis;

internal class UnionCaseAnalyzer : IUnionSymbolAnalyzer
{
    public void AnalyzeSymbol(INamedTypeSymbol namedTypeSymbol, Action<Diagnostic> reportDiagnostic)
    {
        var isCase = false;
        if (!namedTypeSymbol.IsAbstract && namedTypeSymbol.TypeKind != TypeKind.Interface)
        {
            foreach (var baseType in EnumerateBaseTypes(namedTypeSymbol).Concat(namedTypeSymbol.AllInterfaces))
            {
                if (UnionHelper.IsDiscriminatedUnion(baseType))
                {
                    isCase = true;
                    if (namedTypeSymbol.ContainingType == null)
                    {
                        var factoryMethod = baseType.GetMembers()
                            .OfType<IMethodSymbol>()
                            .Where(x => x.IsStatic)
                            .FirstOrDefault(x =>
                                x.Name == namedTypeSymbol.Name &&
                                SymbolEqualityComparer.Default.Equals(x.ReturnType, baseType));
                        if (factoryMethod == null)
                        {
                            var propertyBuilder = ImmutableDictionary.CreateBuilder<string, string?>();
                            propertyBuilder.Add(DiagnosticPropertyNames.QualifiedCaseName, namedTypeSymbol.ToDisplayString());
                            propertyBuilder.Add(DiagnosticPropertyNames.Name, namedTypeSymbol.Name);
                            foreach (var syntaxReference in baseType.DeclaringSyntaxReferences)
                            {
                                reportDiagnostic(Diagnostic.Create(
                                    DiscriminatedUnionsAnalyzer.UnnestedCasesShouldHaveFactoryMethodRule,
                                    syntaxReference.GetSyntax().GetLocation(),
                                    DiagnosticSeverity.Error,
                                    namedTypeSymbol.DeclaringSyntaxReferences.Select(x => x.GetSyntax().GetLocation()),
                                    propertyBuilder.ToImmutable(),
                                    namedTypeSymbol,
                                    baseType));
                            }
                        }
                    }

                    if (!SymbolEqualityComparer.Default.Equals(namedTypeSymbol.ContainingAssembly, baseType.ContainingAssembly))
                    {
                        foreach (var declaringSyntaxReference in namedTypeSymbol.DeclaringSyntaxReferences)
                        {
                            reportDiagnostic(Diagnostic.Create(
                                DiscriminatedUnionsAnalyzer.CasesMustBeDeclaredInUnionAssemblyRule,
                                declaringSyntaxReference.GetSyntax().GetLocation(),
                                namedTypeSymbol,
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

    private static IEnumerable<INamedTypeSymbol> EnumerateBaseTypes(INamedTypeSymbol? discriminatedUnionType)
    {
        if (discriminatedUnionType == null)
        {
            yield break;
        }

        discriminatedUnionType = discriminatedUnionType.BaseType;
        while (discriminatedUnionType != null)
        {
            yield return discriminatedUnionType;
            discriminatedUnionType = discriminatedUnionType.BaseType;
        }
    }
}