// --------------------------------------------------------------------------------------------------------------------
// <copyright file="LanguageUnionSwitchHelper.cs" company="Sundews">
// Copyright (c) Sundews. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Sundew.DiscriminatedUnions.Analyzer.SwitchStatement;

using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Threading;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Operations;
using Sundew.DiscriminatedUnions.Shared;

/// <summary>
/// Finds the cases that a switch statement over a closed type or a union type does not handle.
/// The compiler checks that for switch expressions, but it has no such check for switch statements.
/// </summary>
public static class LanguageUnionSwitchHelper
{
    private const string Null = "null";

    // A case is named the way it is written in a pattern, without what qualifies it: Circle, int, List<int>.
    private static readonly SymbolDisplayFormat CaseNameFormat = new(
        typeQualificationStyle: SymbolDisplayTypeQualificationStyle.NameOnly,
        genericsOptions: SymbolDisplayGenericsOptions.IncludeTypeParameters,
        miscellaneousOptions: SymbolDisplayMiscellaneousOptions.UseSpecialTypes);

    /// <summary>
    /// Tries to get the cases that the specified switch statement does not handle.
    /// </summary>
    /// <param name="switchOperation">The switch statement.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <param name="unhandledCases">The cases that are not handled, if the switch statement is over a closed type or a union type.</param>
    /// <returns><c>true</c>, if the switch statement is over a closed type or a union type, otherwise <c>false</c>.</returns>
    public static bool TryGetUnhandledCases(ISwitchOperation switchOperation, CancellationToken cancellationToken, out UnhandledCases unhandledCases)
    {
        unhandledCases = default;
        var semanticModel = switchOperation.SemanticModel;
        if (semanticModel == null || GetUnionType(switchOperation.Value.Type) is not { } unionType)
        {
            return false;
        }

        var caseTypes = unionType.GetLanguageUnionCases(cancellationToken);
        var matches = new Matches();
        foreach (var caseClause in switchOperation.Cases.SelectMany(x => x.Clauses))
        {
            AddMatches(caseClause, matches);
        }

        var compilation = semanticModel.Compilation;
        var unhandledCaseTypes = matches.Everything
            ? ImmutableArray<ITypeSymbol>.Empty
            : caseTypes.Where(caseType => !matches.Types.Any(type => IsMatchedBy(caseType, type, compilation))).ToImmutableArray();
        var isNullUnhandled = !matches.Null && CanBeNull(switchOperation, unionType, caseTypes, semanticModel, cancellationToken);
        unhandledCases = new UnhandledCases(unionType, unhandledCaseTypes, isNullUnhandled);
        return true;
    }

    /// <summary>
    /// Gets the names of what the specified case clause handles: the types it matches and null.
    /// </summary>
    /// <param name="caseClauseOperation">The case clause.</param>
    /// <returns>The names.</returns>
    public static IEnumerable<string> GetHandledNames(ICaseClauseOperation caseClauseOperation)
    {
        var matches = new Matches();
        AddMatches(caseClauseOperation, matches);
        return matches.Types.Select(GetCaseName).Concat(matches.Null && !matches.Everything ? [Null] : []);
    }

    /// <summary>
    /// Gets the name of a case as it is reported.
    /// </summary>
    /// <param name="caseType">The case type.</param>
    /// <returns>The name.</returns>
    public static string GetCaseName(ITypeSymbol caseType)
    {
        return GetPatternType(caseType).ToDisplayString(CaseNameFormat);
    }

    /// <summary>
    /// Gets the type that matches the specified case in a pattern. A nullable value type cannot be written in a pattern: the type it wraps matches the values it has.
    /// </summary>
    /// <param name="caseType">The case type.</param>
    /// <returns>The type.</returns>
    public static ITypeSymbol GetPatternType(ITypeSymbol caseType)
    {
        return caseType is INamedTypeSymbol { OriginalDefinition.SpecialType: SpecialType.System_Nullable_T } nullableType ? nullableType.TypeArguments[0] : caseType;
    }

    private static INamedTypeSymbol? GetUnionType(ITypeSymbol? typeSymbol)
    {
        // A union type is a struct, so a union that can be null is a nullable value type.
        var unionType = typeSymbol is INamedTypeSymbol { OriginalDefinition.SpecialType: SpecialType.System_Nullable_T } nullableType ? nullableType.TypeArguments[0] : typeSymbol;
        return unionType.IsLanguageUnion() ? unionType.WithNullableAnnotation(NullableAnnotation.NotAnnotated) as INamedTypeSymbol : null;
    }

    private static bool IsMatchedBy(ITypeSymbol caseType, ITypeSymbol matchedType, Compilation compilation)
    {
        // A type pattern matches every value of its type, so it also handles the cases that derive from that type or implement it.
        var conversion = compilation.ClassifyConversion(GetPatternType(caseType), matchedType);
        return conversion.IsIdentity || (conversion.IsImplicit && (conversion.IsReference || conversion.IsBoxing));
    }

    private static bool CanBeNull(ISwitchOperation switchOperation, INamedTypeSymbol unionType, ImmutableArray<ITypeSymbol> caseTypes, SemanticModel semanticModel, CancellationToken cancellationToken)
    {
        // Null has to be handled where the compiler asks a switch expression to handle it: where nullable analysis says that the value can be null.
        if (!semanticModel.GetNullableContext(switchOperation.Syntax.SpanStart).WarningsEnabled())
        {
            return false;
        }

        if (semanticModel.GetTypeInfo(switchOperation.Value.Syntax, cancellationToken).ConvertedNullability.FlowState == NullableFlowState.MaybeNull)
        {
            return true;
        }

        // A union contains null when one of its case types allows it, which shows on the constructor that creates the union from that case.
        return unionType.IsUnion
               && (caseTypes.Any(x => x.NullableAnnotation == NullableAnnotation.Annotated || x.OriginalDefinition.SpecialType == SpecialType.System_Nullable_T)
                   || unionType.InstanceConstructors.Any(x => x.DeclaredAccessibility == Accessibility.Public && x.Parameters.Length == 1 && x.Parameters[0].NullableAnnotation == NullableAnnotation.Annotated));
    }

    private static void AddMatches(ICaseClauseOperation caseClauseOperation, Matches matches)
    {
        switch (caseClauseOperation)
        {
            case IDefaultCaseClauseOperation:
                matches.Everything = true;
                matches.Null = true;
                break;

            // A case with a when clause only handles the values that the clause lets through, so it does not count.
            case IPatternCaseClauseOperation { Guard: null } patternCaseClauseOperation:
                AddMatches(patternCaseClauseOperation.Pattern, matches);
                break;
            case ISingleValueCaseClauseOperation singleValueCaseClauseOperation when IsNull(singleValueCaseClauseOperation.Value):
                matches.Null = true;
                break;
        }
    }

    private static void AddMatches(IPatternOperation patternOperation, Matches matches)
    {
        switch (patternOperation)
        {
            // A discard and var match every value, null included.
            case IDiscardPatternOperation:
            case IDeclarationPatternOperation { MatchedType: null }:
                matches.Everything = true;
                matches.Null = true;
                break;
            case IDeclarationPatternOperation { MatchedType: { } matchedType }:
                matches.Types.Add(matchedType);
                break;
            case ITypePatternOperation typePatternOperation:
                matches.Types.Add(typePatternOperation.MatchedType);
                break;
            case IConstantPatternOperation constantPatternOperation when IsNull(constantPatternOperation.Value):
                matches.Null = true;
                break;

            // "not null" matches every value there is.
            case INegatedPatternOperation { Pattern: IConstantPatternOperation negatedPatternOperation } when IsNull(negatedPatternOperation.Value):
                matches.Everything = true;
                break;

            // A pattern that looks into a value only handles a case when what it looks at can be anything, as in "Circle { }" and "Circle(var radius)".
            case IRecursivePatternOperation recursivePatternOperation when MatchesEveryValueOfItsType(recursivePatternOperation):
                if (recursivePatternOperation.Syntax is RecursivePatternSyntax { Type: not null })
                {
                    matches.Types.Add(recursivePatternOperation.MatchedType);
                }
                else if (recursivePatternOperation.DeconstructionSubpatterns.IsEmpty)
                {
                    matches.Everything = true;
                }

                break;
            case IBinaryPatternOperation { OperatorKind: BinaryOperatorKind.Or } orPatternOperation:
                AddMatches(orPatternOperation.LeftPattern, matches);
                AddMatches(orPatternOperation.RightPattern, matches);
                break;
            case IBinaryPatternOperation { OperatorKind: BinaryOperatorKind.And } andPatternOperation:
                var left = new Matches();
                var right = new Matches();
                AddMatches(andPatternOperation.LeftPattern, left);
                AddMatches(andPatternOperation.RightPattern, right);

                // Both sides have to match, so what one side matches is only handled when the other side matches everything.
                var narrowing = right.Everything ? left : left.Everything ? right : null;
                if (narrowing != null)
                {
                    matches.Everything |= narrowing.Everything;
                    matches.Types.AddRange(narrowing.Types);
                    matches.Null |= left.Null && right.Null;
                }

                break;
        }
    }

    private static bool MatchesEveryValueOfItsType(IRecursivePatternOperation recursivePatternOperation)
    {
        return recursivePatternOperation.DeconstructionSubpatterns.All(MatchesEverything)
               && recursivePatternOperation.PropertySubpatterns.All(x => MatchesEverything(x.Pattern));
    }

    private static bool MatchesEverything(IPatternOperation patternOperation)
    {
        return patternOperation is IDiscardPatternOperation or IDeclarationPatternOperation { MatchedType: null };
    }

    private static bool IsNull(IOperation operation)
    {
        return operation.ConstantValue is { HasValue: true, Value: null };
    }

    /// <summary>
    /// What the cases of a switch statement match.
    /// </summary>
    private sealed class Matches
    {
        /// <summary>
        /// Gets the types of which every value is matched.
        /// </summary>
        public List<ITypeSymbol> Types { get; } = new();

        /// <summary>
        /// Gets or sets a value indicating whether every value that is not null is matched.
        /// </summary>
        public bool Everything { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether null is matched.
        /// </summary>
        public bool Null { get; set; }
    }
}
