// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DiscriminatedUnionCaseAnalyzer.cs" company="Hukano">
// Copyright (c) Hukano. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Sundew.DiscriminatedUnions.Analyzer
{
    using System;
    using System.Collections.Immutable;
    using System.Linq;
    using Microsoft.CodeAnalysis;

    internal class DiscriminatedUnionCaseAnalyzer
    {
        public void AnalyzeSymbol(INamedTypeSymbol namedTypeSymbol, Action<Diagnostic> reportDiagnostic)
        {
            var isCase = false;
            if (!namedTypeSymbol.IsAbstract && namedTypeSymbol.TypeKind != TypeKind.Interface)
            {
                var discriminatedUnionType = namedTypeSymbol;
                while (discriminatedUnionType.BaseType != null)
                {
                    discriminatedUnionType = discriminatedUnionType.BaseType;
                    if (DiscriminatedUnionHelper.IsDiscriminatedUnion(discriminatedUnionType))
                    {
                        isCase = true;
                        if (namedTypeSymbol.ContainingType == null)
                        {
                            var factoryMethod = discriminatedUnionType.GetMembers()
                                .OfType<IMethodSymbol>()
                                .Where(x => x.IsStatic)
                                .FirstOrDefault(x =>
                                    x.Name == namedTypeSymbol.Name &&
                                    SymbolEqualityComparer.Default.Equals(x.ReturnType, discriminatedUnionType));
                            if (factoryMethod == null)
                            {
                                foreach (var syntaxReference in discriminatedUnionType.DeclaringSyntaxReferences)
                                {
                                    var propertyBuilder = ImmutableDictionary.CreateBuilder<string, string?>();
                                    propertyBuilder.Add(DiagnosticPropertyNames.QualifiedCaseName, namedTypeSymbol.ToDisplayString());
                                    propertyBuilder.Add(DiagnosticPropertyNames.Name, namedTypeSymbol.Name);
                                    reportDiagnostic(Diagnostic.Create(
                                        SundewDiscriminatedUnionsAnalyzer.UnnestedCasesShouldHaveFactoryMethodRule,
                                        syntaxReference.GetSyntax().GetLocation(),
                                        propertyBuilder.ToImmutable(),
                                        namedTypeSymbol,
                                        discriminatedUnionType));
                                }
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
                        SundewDiscriminatedUnionsAnalyzer.CasesShouldBeSealedRule,
                        declaringSyntaxReference.GetSyntax().GetLocation(),
                        namedTypeSymbol));
                }
            }
        }
    }
}