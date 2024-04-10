// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SwitchExpressionHelper.cs" company="Sundews">
// Copyright (c) Sundews. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Sundew.DiscriminatedUnions.Analyzer.SwitchExpression;

using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Operations;

/// <summary>
/// Helpers for analyzing discriminated unions.
/// </summary>
public static class SwitchExpressionHelper
{
    /// <summary>
    /// Gets the handled case types.
    /// </summary>
    /// <param name="switchExpressionOperation">The switch expression operation.</param>
    /// <returns>The handled case types.</returns>
    public static IEnumerable<CaseInfo> GetHandledCaseTypes(ISwitchExpressionOperation switchExpressionOperation)
    {
        return switchExpressionOperation.Arms.SelectMany((switchExpressionArmOperation) =>
            {
                var throwingNotImplementedException = UnionHelper.GetThrowingNotImplementedException(switchExpressionArmOperation.Value);
                if (switchExpressionArmOperation.Pattern is IDeclarationPatternOperation declarationPatternSyntax)
                {
                    return UnionHelper.GetHandledSymbols(declarationPatternSyntax.MatchedType as INamedTypeSymbol, true, throwingNotImplementedException);
                }

                if (switchExpressionArmOperation.Pattern is ITypePatternOperation typePatternOperation)
                {
                    return UnionHelper.GetHandledSymbols(typePatternOperation.MatchedType as INamedTypeSymbol, true, throwingNotImplementedException);
                }

                if (switchExpressionArmOperation.Pattern is IConstantPatternOperation { Value: IFieldReferenceOperation fieldReferenceOperation })
                {
                    return UnionHelper.GetHandledFieldTypeSymbols(fieldReferenceOperation.Field, throwingNotImplementedException);
                }

                if (IsNullLiteralArmOperation(switchExpressionArmOperation))
                {
                    return Enumerable.Empty<(ISymbol? Symbol, bool HandlesCase, IOperation? ThrowingNotImplementedException)>();
                }

                return UnionHelper.GetHandledSymbols(switchExpressionArmOperation.Pattern.NarrowedType as INamedTypeSymbol, false, throwingNotImplementedException);
            })
            .Select(x => new CaseInfo { Symbol = x.Symbol, HandlesCase = x.HandlesCase, ThrowingNotImplementedException = x.ThrowingNotImplementedException });
    }

    /// <summary>
    /// Determines whether [has null case] [the specified switch expression operation].
    /// </summary>
    /// <param name="switchExpressionOperation">The switch expression operation.</param>
    /// <returns>
    ///   <c>true</c> if [has null case] [the specified switch expression operation]; otherwise, <c>false</c>.
    /// </returns>
    public static NullCase? GetNullCase(ISwitchExpressionOperation switchExpressionOperation)
    {
        var nullCase = switchExpressionOperation.Arms.FirstOrDefault(IsNullLiteralArmOperation);
        return nullCase == default ? default(NullCase?) : new NullCase(nullCase, UnionHelper.GetThrowingNotImplementedException(nullCase.Value));
    }

    private static bool IsNullLiteralArmOperation(ISwitchExpressionArmOperation x)
    {
        return x.Pattern is IConstantPatternOperation constantPatternOperation &&
               ((constantPatternOperation.Value is IConversionOperation conversionOperation &&
                 conversionOperation.Operand is ILiteralOperation conversionLiteralOperation &&
                 IsNullLiteral(conversionLiteralOperation)) ||
                (constantPatternOperation.Value is ILiteralOperation literalOperation &&
                 IsNullLiteral(literalOperation)));
    }

    private static bool IsNullLiteral(ILiteralOperation literalOperation)
    {
        return literalOperation.ConstantValue.HasValue &&
               literalOperation.ConstantValue.Value == null;
    }
}