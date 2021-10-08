// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DiscriminatedUnionTypeAnalyzer.cs" company="Hukano">
// Copyright (c) Hukano. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Sundew.DiscriminatedUnions.Analyzer
{
    using System.Linq;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.Diagnostics;

    internal class DiscriminatedUnionTypeAnalyzer
    {
        public void AnalyzeDiscriminatedUnionsTypes(SymbolAnalysisContext context)
        {
            if (context.Symbol is not INamedTypeSymbol namedTypeSymbol)
            {
                return;
            }

            if (DiscriminatedUnionHelper.IsDiscriminatedUnion(namedTypeSymbol))
            {
                var invalidConstructors = namedTypeSymbol.Constructors
                    .Where(x => !SymbolEqualityComparer.Default.Equals(x.Parameters.SingleOrDefault()?.Type, namedTypeSymbol) &&
                                x.DeclaredAccessibility != Accessibility.Private);
                foreach (var invalidConstructor in invalidConstructors)
                {
                    if (invalidConstructor.DeclaringSyntaxReferences.IsEmpty)
                    {
                        foreach (var declaringSyntaxReference in namedTypeSymbol.DeclaringSyntaxReferences)
                        {
                            context.ReportDiagnostic(Diagnostic.Create(
                                SundewDiscriminatedUnionsAnalyzer.MustHavePrivateConstructorRule,
                                declaringSyntaxReference.GetSyntax().GetLocation(),
                                namedTypeSymbol,
                                invalidConstructor));
                        }
                    }

                    foreach (var declaringSyntaxReference in invalidConstructor.DeclaringSyntaxReferences)
                    {
                        context.ReportDiagnostic(Diagnostic.Create(
                            SundewDiscriminatedUnionsAnalyzer.DiscriminatedUnionCanOnlyHavePrivateConstructorsRule,
                            declaringSyntaxReference.GetSyntax().GetLocation(),
                            namedTypeSymbol));
                    }
                }

                return;
            }

            if (namedTypeSymbol.BaseType == null || !DiscriminatedUnionHelper.IsDiscriminatedUnion(namedTypeSymbol.BaseType))
            {
                return;
            }

            if (!namedTypeSymbol.IsSealed)
            {
                foreach (var declaringSyntaxReference in namedTypeSymbol.DeclaringSyntaxReferences)
                {
                    context.ReportDiagnostic(Diagnostic.Create(SundewDiscriminatedUnionsAnalyzer.CasesShouldBeSealedRule, declaringSyntaxReference.GetSyntax().GetLocation(), namedTypeSymbol));
                }
            }

            if (!SymbolEqualityComparer.Default.Equals(namedTypeSymbol.ContainingType, namedTypeSymbol.BaseType))
            {
                foreach (var declaringSyntaxReference in namedTypeSymbol.DeclaringSyntaxReferences)
                {
                    context.ReportDiagnostic(Diagnostic.Create(SundewDiscriminatedUnionsAnalyzer.CasesShouldNestedRule, declaringSyntaxReference.GetSyntax().GetLocation(), namedTypeSymbol, namedTypeSymbol.BaseType));
                }
            }
        }
    }
}