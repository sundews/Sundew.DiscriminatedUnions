// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SwitchStatementHelper.cs" company="Sundews">
// Copyright (c) Sundews. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Sundew.DiscriminatedUnions.Analyzer.SwitchStatement;

using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;
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
            switchCaseOperation.Clauses.SelectMany(caseClauseOperation =>
                {
                    var throwingNotImplementedException = switchCaseOperation.Body.Select(UnionHelper.GetThrowingNotImplementedException).FirstOrDefault();
                    if (caseClauseOperation is IPatternCaseClauseOperation patternCaseClauseOperation)
                    {
                        if (patternCaseClauseOperation.Pattern is IDeclarationPatternOperation
                            declarationPatternOperation)
                        {
                            return UnionHelper.GetHandledSymbols(declarationPatternOperation.MatchedType as INamedTypeSymbol, true, throwingNotImplementedException);
                        }

                        if (patternCaseClauseOperation.Pattern is ITypePatternOperation typePatternOperation)
                        {
                            return UnionHelper.GetHandledSymbols(typePatternOperation.MatchedType as INamedTypeSymbol, true, throwingNotImplementedException);
                        }

                        if (patternCaseClauseOperation.Pattern is IConstantPatternOperation { Value: IFieldReferenceOperation fieldReferenceOperation })
                        {
                            return UnionHelper.GetHandledFieldTypeSymbols(fieldReferenceOperation.Field, throwingNotImplementedException);
                        }

                        if (IsNullLiteralCaseClauseOperation(caseClauseOperation))
                        {
                            return Enumerable.Empty<(ISymbol? Symbol, bool HandlesCase, IOperation? ThrowingNotImplementedException)>();
                        }

                        return UnionHelper.GetHandledSymbols(patternCaseClauseOperation.Pattern.NarrowedType as INamedTypeSymbol, false, throwingNotImplementedException);
                    }

                    if (caseClauseOperation is ISingleValueCaseClauseOperation { Value: IFieldReferenceOperation fieldReferenceOperation2 })
                    {
                        return UnionHelper.GetHandledFieldTypeSymbols(fieldReferenceOperation2.Field, throwingNotImplementedException);
                    }

                    return Enumerable.Empty<(ISymbol? Symbol, bool HandlesCase, IOperation? ThrowingNotImplementedException)>();
                }).Select(x => new CaseInfo { Symbol = x.Symbol, HandlesCase = x.HandlesCase, ThrowingNotImplementedException = x.ThrowingNotImplementedException }));
    }

    /// <summary>
    /// Determines whether [has null case] [the specified switch operation].
    /// </summary>
    /// <param name="switchOperation">The switch operation.</param>
    /// <returns>
    ///   <c>true</c> if [has null case] [the specified switch operation]; otherwise, <c>false</c>.
    /// </returns>
    public static NullCase? GetNullCase(ISwitchOperation switchOperation)
    {
        var (nullCase, throwingNotImplementedException) = switchOperation.Cases.Select(caseOperation =>
        {
            var hasNullClause = caseOperation.Clauses.Any(IsNullLiteralCaseClauseOperation);
            return hasNullClause ? (Case: caseOperation, Clause: caseOperation.Body.Select(UnionHelper.GetThrowingNotImplementedException).FirstOrDefault()) : default;
        }).FirstOrDefault(x => x.Case != default);
        if (nullCase == default)
        {
            return null;
        }

        return new NullCase(nullCase, throwingNotImplementedException);
    }

    private static bool IsNullLiteralCaseClauseOperation(ICaseClauseOperation x)
    {
        return (x is IPatternCaseClauseOperation patternCaseClauseOperation &&
                patternCaseClauseOperation.Pattern is IConstantPatternOperation constantPatternOperation &&
                ((constantPatternOperation.Value is IConversionOperation conversionOperation &&
                  conversionOperation.Operand is ILiteralOperation conversionLiteralOperation &&
                  IsNullLiteral(conversionLiteralOperation)) ||
                 (constantPatternOperation.Value is ILiteralOperation literalOperation &&
                  IsNullLiteral(literalOperation)))) ||
               (x is ISingleValueCaseClauseOperation singleValueCaseClauseOperation &&
                singleValueCaseClauseOperation.Value is IConversionOperation conversionOperation2 &&
                conversionOperation2.Operand is ILiteralOperation literalOperation2 &&
                IsNullLiteral(literalOperation2));
    }

    private static bool IsNullLiteral(ILiteralOperation literalOperation)
    {
        return literalOperation.ConstantValue.HasValue &&
               literalOperation.ConstantValue.Value == null;
    }
}