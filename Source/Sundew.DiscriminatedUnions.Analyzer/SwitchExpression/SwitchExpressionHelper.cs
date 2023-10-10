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
        ISymbol? GetActualTypeSymbol(INamedTypeSymbol? namedTypeSymbol)
        {
            return namedTypeSymbol?.IsGenericType ?? false ? namedTypeSymbol.OriginalDefinition : namedTypeSymbol;
        }

        return switchExpressionOperation.Arms.Select((switchExpressionArmOperation) =>
            {
                if (switchExpressionArmOperation.Pattern is IDeclarationPatternOperation
                    declarationPatternSyntax)
                {
                    return (Type: GetActualTypeSymbol(declarationPatternSyntax.MatchedType as INamedTypeSymbol), HandlesCase: true);
                }

                if (switchExpressionArmOperation.Pattern is ITypePatternOperation typePatternOperation)
                {
                    return (Type: GetActualTypeSymbol(typePatternOperation.MatchedType as INamedTypeSymbol), HandlesCase: true);
                }

                if (switchExpressionArmOperation.Pattern is IConstantPatternOperation { Value: IFieldReferenceOperation fieldReferenceOperation })
                {
                    return (Type: fieldReferenceOperation.Field, HandlesCase: true);
                }

                return (Type: GetActualTypeSymbol(switchExpressionArmOperation.Pattern.NarrowedType as INamedTypeSymbol), HandlesCase: false);
            }).Where(x => x.Type != null)
            .Select(x => new CaseInfo { Symbol = x.Type!, HandlesCase = x.HandlesCase });
    }

    /// <summary>
    /// Determines whether [has null case] [the specified switch expression operation].
    /// </summary>
    /// <param name="switchExpressionOperation">The switch expression operation.</param>
    /// <returns>
    ///   <c>true</c> if [has null case] [the specified switch expression operation]; otherwise, <c>false</c>.
    /// </returns>
    public static ISwitchExpressionArmOperation? GetNullCase(ISwitchExpressionOperation switchExpressionOperation)
    {
        return switchExpressionOperation.Arms.FirstOrDefault(x => x.Pattern is IConstantPatternOperation constantPatternOperation &&
                                                                  ((constantPatternOperation.Value is IConversionOperation conversionOperation &&
                                                                    conversionOperation.Operand is ILiteralOperation conversionLiteralOperation &&
                                                                    IsNullLiteral(conversionLiteralOperation)) ||
                                                                   (constantPatternOperation.Value is ILiteralOperation literalOperation &&
                                                                    IsNullLiteral(literalOperation))));
    }

    private static bool IsNullLiteral(ILiteralOperation literalOperation)
    {
        return literalOperation.ConstantValue.HasValue &&
               literalOperation.ConstantValue.Value == null;
    }
}