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
        ISymbol? GetActualTypeSymbol(INamedTypeSymbol? namedTypeSymbol)
        {
            return namedTypeSymbol?.IsGenericType ?? false ? namedTypeSymbol.OriginalDefinition : namedTypeSymbol;
        }

        return switchOperation.Cases.SelectMany(switchCaseOperation =>
            switchCaseOperation.Clauses.Select(caseClauseOperation =>
                {
                    if (caseClauseOperation is IPatternCaseClauseOperation patternCaseClauseOperation)
                    {
                        if (patternCaseClauseOperation.Pattern is IDeclarationPatternOperation
                            declarationPatternOperation)
                        {
                            return (Type: GetActualTypeSymbol(declarationPatternOperation.MatchedType as INamedTypeSymbol), HandlesCase: true);
                        }

                        if (patternCaseClauseOperation.Pattern is ITypePatternOperation typePatternOperation)
                        {
                            return (Type: GetActualTypeSymbol(typePatternOperation.MatchedType as INamedTypeSymbol), HandlesCase: true);
                        }

                        if (patternCaseClauseOperation.Pattern is IConstantPatternOperation { Value: IFieldReferenceOperation fieldReferenceOperation })
                        {
                            return (Type: fieldReferenceOperation.Field, HandlesCase: true);
                        }

                        return (Type: GetActualTypeSymbol(patternCaseClauseOperation.Pattern.NarrowedType as INamedTypeSymbol), HandlesCase: false);
                    }

                    if (caseClauseOperation is ISingleValueCaseClauseOperation { Value: IFieldReferenceOperation fieldReferenceOperation2 })
                    {
                        return (Type: fieldReferenceOperation2.Field, HandlesCase: true);
                    }

                    return (Type: null, HandlesCase: false);
                }).Where(x => x.Type != null)
                .Select(x => new CaseInfo { Symbol = x.Type!, HandlesCase = x.HandlesCase }));
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
            x => (x is IPatternCaseClauseOperation patternCaseClauseOperation &&
                  patternCaseClauseOperation.Pattern is IConstantPatternOperation constantPatternOperation &&
                  ((constantPatternOperation.Value is IConversionOperation conversionOperation &&
                    conversionOperation.Operand is ILiteralOperation conversionLiteralOperation &&
                    IsNullLiteral(conversionLiteralOperation)) ||
                   (constantPatternOperation.Value is ILiteralOperation literalOperation &&
                    IsNullLiteral(literalOperation)))) ||
                 (x is ISingleValueCaseClauseOperation singleValueCaseClauseOperation &&
                  singleValueCaseClauseOperation.Value is IConversionOperation conversionOperation2 &&
                  conversionOperation2.Operand is ILiteralOperation literalOperation2 &&
                  IsNullLiteral(literalOperation2))));
    }

    private static bool IsNullLiteral(ILiteralOperation literalOperation)
    {
        return literalOperation.ConstantValue.HasValue &&
               literalOperation.ConstantValue.Value == null;
    }
}