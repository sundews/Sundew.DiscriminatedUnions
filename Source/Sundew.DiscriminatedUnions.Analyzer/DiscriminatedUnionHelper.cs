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
        public static IEnumerable<(ITypeSymbol Type, int Index, bool HandlesCase)> GetHandledCaseTypes(ISwitchExpressionOperation switchExpressionOperation)
        {
            return switchExpressionOperation.Arms.Select((switchExpressionArmOperation, index) =>
            {
                if (switchExpressionArmOperation.Pattern is IDeclarationPatternOperation
                    declarationPatternSyntax)
                {
                    return (Type: declarationPatternSyntax.MatchedType, index, HandlesCase: true);
                }

                if (switchExpressionArmOperation.Pattern is ITypePatternOperation typePatternOperation)
                {
                    return (Type: typePatternOperation.MatchedType, index, HandlesCase: true);
                }

                return (Type: switchExpressionArmOperation.Pattern.NarrowedType, index, HandlesCase: false);
            }).Where(x => x.Type != null).Select(x => (x.Type!, x.index, x.HandlesCase));
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
        public static IEnumerable<(ITypeSymbol Type, int Index, bool HandlesCase)> GetHandledCaseTypes(ISwitchOperation switchOperation)
        {
            var index = 0;
            return switchOperation.Cases.SelectMany(switchCaseOperation =>
                switchCaseOperation.Clauses.Select(caseClauseOperation =>
                {
                    if (caseClauseOperation is IPatternCaseClauseOperation patternCaseClauseOperation)
                    {
                        if (patternCaseClauseOperation.Pattern is IDeclarationPatternOperation
                            declarationPatternOperation)
                        {
                            return (Type: declarationPatternOperation.MatchedType, Index: index++, HandlesCase: true);
                        }

                        if (patternCaseClauseOperation.Pattern is ITypePatternOperation typePatternOperation)
                        {
                            return (Type: typePatternOperation.MatchedType, Index: index++, HandlesCase: true);
                        }

                        return (Type: patternCaseClauseOperation.Pattern.NarrowedType, Index: index++, HandlesCase: false);
                    }

                    /*if (caseClauseOperation is IDefaultCaseClauseOperation defaultCaseClauseOperation)
                    {
                        context.ReportDiagnostic(Diagnostic.Create(SwitchShouldNotHaveDefaultCaseRule, defaultCaseClauseOperation.Syntax.GetLocation(), unionType));
                    }*/

                    return (Type: null, Index: -1, HandlesCase: false);
                }).Where(x => x.Type != null).Select(x => (x.Type!, x.Index, x.HandlesCase)));
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