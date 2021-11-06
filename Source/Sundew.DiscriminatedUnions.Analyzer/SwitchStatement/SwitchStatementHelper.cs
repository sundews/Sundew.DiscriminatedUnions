// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SwitchStatementHelper.cs" company="Hukano">
// Copyright (c) Hukano. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Sundew.DiscriminatedUnions.Analyzer.SwitchStatement
{
    using System.Collections.Generic;
    using System.Linq;
    using Microsoft.CodeAnalysis.Operations;

    /// <summary>
    /// Helpers for analyzing discriminated unions.
    /// </summary>
    public static class SwitchStatementHelper
    {
        /// <summary>
        /// Gets the handled case types.
        /// </summary>
        /// <param name="switchOperation">The switch operation.</param>
        /// <returns>The handled case types.</returns>
        public static IEnumerable<CaseInfo> GetHandledCaseTypes(ISwitchOperation switchOperation)
        {
            return switchOperation.Cases.SelectMany(switchCaseOperation =>
                switchCaseOperation.Clauses.Select(caseClauseOperation =>
                    {
                        if (caseClauseOperation is IPatternCaseClauseOperation patternCaseClauseOperation)
                        {
                            if (patternCaseClauseOperation.Pattern is IDeclarationPatternOperation
                                declarationPatternOperation)
                            {
                                return (Type: declarationPatternOperation.MatchedType, HandlesCase: true);
                            }

                            if (patternCaseClauseOperation.Pattern is ITypePatternOperation typePatternOperation)
                            {
                                return (Type: typePatternOperation.MatchedType, HandlesCase: true);
                            }

                            return (Type: patternCaseClauseOperation.Pattern.NarrowedType, HandlesCase: false);
                        }

                        return (Type: null, HandlesCase: false);
                    }).Where(x => x.Type != null)
                    .Select(x => new CaseInfo { Type = x.Type!, HandlesCase = x.HandlesCase }));
        }

        /// <summary>
        /// Determines whether [has null case] [the specified switch operation].
        /// </summary>
        /// <param name="switchOperation">The switch operation.</param>
        /// <returns>
        ///   <c>true</c> if [has null case] [the specified switch operation]; otherwise, <c>false</c>.
        /// </returns>
        public static ISwitchCaseOperation? GetNullCase(ISwitchOperation switchOperation)
        {
            return switchOperation.Cases.FirstOrDefault(x => x.Clauses.Any(
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