// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DiscriminatedUnionBaseAnalyzer.cs" company="Hukano">
// Copyright (c) Hukano. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Sundew.DiscriminatedUnions.Analyzer;

using System;
using System.Linq;
using Microsoft.CodeAnalysis;

internal class DiscriminatedUnionBaseAnalyzer
{
    public void AnalyzeSymbol(INamedTypeSymbol namedTypeSymbol, Action<Diagnostic> reportDiagnostic)
    {
        if (!DiscriminatedUnionHelper.IsDiscriminatedUnion(namedTypeSymbol))
        {
            return;
        }

        if (namedTypeSymbol.TypeKind == TypeKind.Class && !namedTypeSymbol.IsAbstract)
        {
            foreach (var declaringSyntaxReference in namedTypeSymbol.DeclaringSyntaxReferences)
            {
                reportDiagnostic(
                    Diagnostic.Create(
                        SundewDiscriminatedUnionsAnalyzer.ClassDiscriminatedUnionsMustBeAbstractRule,
                        declaringSyntaxReference.GetSyntax().GetLocation(),
                        namedTypeSymbol));
            }
        }

        if (namedTypeSymbol.TypeKind == TypeKind.Interface &&
            (namedTypeSymbol.DeclaredAccessibility != Accessibility.Internal &&
             namedTypeSymbol.DeclaredAccessibility != Accessibility.Private))
        {
            foreach (var declaringSyntaxReference in namedTypeSymbol.DeclaringSyntaxReferences)
            {
                reportDiagnostic(
                    Diagnostic.Create(
                        SundewDiscriminatedUnionsAnalyzer.InterfaceDiscriminatedUnionsMustBeInternalRule,
                        declaringSyntaxReference.GetSyntax().GetLocation(),
                        namedTypeSymbol));
            }
        }

        var invalidConstructors = namedTypeSymbol.Constructors
            .Where(x => !SymbolEqualityComparer.Default.Equals(x.Parameters.SingleOrDefault()?.Type, namedTypeSymbol) &&
                        x.DeclaredAccessibility != Accessibility.Private &&
                        x.DeclaredAccessibility != Accessibility.ProtectedAndInternal);
        foreach (var invalidConstructor in invalidConstructors)
        {
            if (invalidConstructor.DeclaringSyntaxReferences.IsEmpty)
            {
                foreach (var declaringSyntaxReference in namedTypeSymbol.DeclaringSyntaxReferences)
                {
                    reportDiagnostic(Diagnostic.Create(
                        SundewDiscriminatedUnionsAnalyzer.DiscriminatedUnionsMustHavePrivateProtectedConstructorRule,
                        declaringSyntaxReference.GetSyntax().GetLocation(),
                        namedTypeSymbol,
                        invalidConstructor));
                }
            }

            foreach (var declaringSyntaxReference in invalidConstructor.DeclaringSyntaxReferences)
            {
                reportDiagnostic(Diagnostic.Create(
                    SundewDiscriminatedUnionsAnalyzer.DiscriminatedUnionsCanOnlyHavePrivateProtectedConstructorsRule,
                    declaringSyntaxReference.GetSyntax().GetLocation(),
                    namedTypeSymbol));
            }
        }
    }
}