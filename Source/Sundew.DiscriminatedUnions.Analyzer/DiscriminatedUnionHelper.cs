// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DiscriminatedUnionHelper.cs" company="Hukano">
// Copyright (c) Hukano. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Sundew.DiscriminatedUnions.Analyzer
{
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.Linq;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.Diagnostics;
    using Microsoft.CodeAnalysis.Operations;

    /// <summary>
    /// Helpers for analyzing discriminated unions.
    /// </summary>
    public static class DiscriminatedUnionHelper
    {
        /// <summary>
        /// Determines whether [is discriminated union] [the specified union type].
        /// </summary>
        /// <param name="unionType">Type of the union.</param>
        /// <returns>
        ///   <c>true</c> if [is discriminated union] [the specified union type]; otherwise, <c>false</c>.
        /// </returns>
        public static bool IsDiscriminatedUnion([NotNullWhen(true)] ITypeSymbol? unionType)
        {
            if (unionType == null)
            {
                return false;
            }

            return unionType.GetAttributes().Any(attribute =>
            {
                var containingClass = attribute.AttributeClass?.ToDisplayString();
                return containingClass == typeof(Sundew.DiscriminatedUnions.DiscriminatedUnion).FullName;
            });
        }

        /// <summary>
        /// Gets all case types.
        /// </summary>
        /// <param name="unionType">Type of the union.</param>
        /// <returns>All case types within the discriminated unions.</returns>
        public static IEnumerable<INamedTypeSymbol> GetAllCaseTypes(ITypeSymbol unionType)
        {
            return unionType.GetTypeMembers().Where(x => SymbolEqualityComparer.Default.Equals(x.BaseType, unionType));
        }

        /// <summary>
        /// Gets the handled case types.
        /// </summary>
        /// <param name="switchExpressionOperation">The switch expression operation.</param>
        /// <returns>The handled case types.</returns>
        public static IReadOnlyList<ITypeSymbol> GetHandledCaseTypes(ISwitchExpressionOperation switchExpressionOperation)
        {
            return switchExpressionOperation.Arms.Select(switchExpressionArmOperation =>
            {
                if (switchExpressionArmOperation.Pattern is IDeclarationPatternOperation
                    declarationPatternSyntax)
                {
                    return declarationPatternSyntax.MatchedType;
                }

                if (switchExpressionArmOperation.Pattern is ITypePatternOperation typePatternOperation)
                {
                    return typePatternOperation.MatchedType;
                }

                return default;
            }).Where(x => x != null).Select(x => x!).ToList();
        }

        /// <summary>
        /// Determines whether [has null case] [the specified switch expression operation].
        /// </summary>
        /// <param name="switchExpressionOperation">The switch expression operation.</param>
        /// <returns>
        ///   <c>true</c> if [has null case] [the specified switch expression operation]; otherwise, <c>false</c>.
        /// </returns>
        public static bool HasNullCase(ISwitchExpressionOperation switchExpressionOperation)
        {
            return switchExpressionOperation.Arms.Any(x =>
                x.Pattern is IConstantPatternOperation constantPatternOperation &&
                ((constantPatternOperation.Value is IConversionOperation conversionOperation &&
                  conversionOperation.Operand is ILiteralOperation conversionLiteralOperation &&
                  IsNullLiteral(conversionLiteralOperation)) ||
                 (constantPatternOperation.Value is ILiteralOperation literalOperation &&
                  IsNullLiteral(literalOperation))));
        }

        /// <summary>
        /// Gets the handled case types.
        /// </summary>
        /// <param name="switchOperation">The switch operation.</param>
        /// <returns>The handled case types.</returns>
        public static IReadOnlyList<ITypeSymbol> GetHandledCaseTypes(ISwitchOperation switchOperation)
        {
            return switchOperation.Cases.SelectMany(switchCaseOperation =>
                switchCaseOperation.Clauses.Select(caseClauseOperation =>
                {
                    if (caseClauseOperation is IPatternCaseClauseOperation patternCaseClauseOperation)
                    {
                        if (patternCaseClauseOperation.Pattern is IDeclarationPatternOperation
                            declarationPatternOperation)
                        {
                            return declarationPatternOperation.MatchedType;
                        }

                        if (patternCaseClauseOperation.Pattern is ITypePatternOperation typePatternOperation)
                        {
                            return typePatternOperation.MatchedType;
                        }
                    }

                    /*if (caseClauseOperation is IDefaultCaseClauseOperation defaultCaseClauseOperation)
                    {
                        context.ReportDiagnostic(Diagnostic.Create(SwitchShouldNotHaveDefaultCaseRule, defaultCaseClauseOperation.Syntax.GetLocation(), unionType));
                    }*/

                    return null;
                })).Where(x => x != null).Select(x => x!).ToList();
        }

        /// <summary>
        /// Determines whether [has null case] [the specified switch operation].
        /// </summary>
        /// <param name="switchOperation">The switch operation.</param>
        /// <returns>
        ///   <c>true</c> if [has null case] [the specified switch operation]; otherwise, <c>false</c>.
        /// </returns>
        public static bool HasNullCase(ISwitchOperation switchOperation)
        {
            return switchOperation.Cases.Any(x => x.Clauses.Any(
                x => x is IPatternCaseClauseOperation patternCaseClauseOperation &&
                     patternCaseClauseOperation.Pattern is IConstantPatternOperation constantPatternOperation &&
                     ((constantPatternOperation.Value is IConversionOperation conversionOperation &&
                       conversionOperation.Operand is ILiteralOperation conversionLiteralOperation &&
                       IsNullLiteral(conversionLiteralOperation)) ||
                      (constantPatternOperation.Value is ILiteralOperation literalOperation &&
                       IsNullLiteral(literalOperation)))));
        }

        private static bool IsNullLiteral(ILiteralOperation literalOperation)
        {
            return literalOperation.ConstantValue.HasValue &&
                   literalOperation.ConstantValue.Value == null;
        }
    }
}