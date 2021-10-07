// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DiscriminatedUnionSwitchAnalyzer.cs" company="Hukano">
// Copyright (c) Hukano. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Sundew.DiscriminatedUnions.Analyzer
{
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.Linq;
    using System.Text;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.Diagnostics;
    using Microsoft.CodeAnalysis.Operations;

    internal class DiscriminatedUnionSwitchAnalyzer
    {
        private const string Null = "null";
        private const string Is = "is";
        private const string Are = "are";

        [SuppressMessage("MicrosoftCodeAnalysisCorrectness", "RS1024:Compare symbols correctly", Justification = "False positive... using SymbolEqualityComparer")]
        private readonly ConcurrentDictionary<ISymbol, HashSet<ITypeSymbol>> discriminatedUnions = new (SymbolEqualityComparer.Default);

        private enum SwitchNullabilityError
        {
            None,
            IsMissingNullCase,
            HasUnreachableNullCase,
        }

        public void AnalyzeSwitchExpression(OperationAnalysisContext context)
        {
            bool HasNullCase(ISwitchExpressionOperation switchExpressionOperation)
            {
                return switchExpressionOperation.Arms.Any(x =>
                    x.Pattern is IConstantPatternOperation constantPatternOperation &&
                    ((constantPatternOperation.Value is IConversionOperation conversionOperation &&
                      conversionOperation.Operand is ILiteralOperation conversionLiteralOperation &&
                      IsNullLiteral(conversionLiteralOperation)) ||
                     (constantPatternOperation.Value is ILiteralOperation literalOperation &&
                      IsNullLiteral(literalOperation))));
            }

            if (!(context.Operation is ISwitchExpressionOperation switchExpressionOperation && switchExpressionOperation.SemanticModel != null))
            {
                return;
            }

            var unionTypeSymbol = switchExpressionOperation.Value.Type;
            var unionType = unionTypeSymbol as INamedTypeSymbol;
            if (!DiscriminatedUnionHelper.IsDiscriminatedUnionSwitch(unionType))
            {
                return;
            }

            var unionTypeWithoutNull = unionType.WithNullableAnnotation(NullableAnnotation.NotAnnotated);
            var actualCaseTypes = switchExpressionOperation.Arms.Select(switchExpressionArmOperation =>
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

                if (switchExpressionArmOperation.Pattern is IDiscardPatternOperation discardPatternOperation)
                {
                    context.ReportDiagnostic(Diagnostic.Create(
                        SundewDiscriminatedUnionsAnalyzer.SwitchShouldNotHaveDefaultCaseRule,
                        switchExpressionArmOperation.Syntax.GetLocation(),
                        unionType));
                }

                /*if (switchExpressionArmOperation.Pattern is IDiscardPatternOperation discardPatternOperation)
                {
                    if (!(switchExpressionArmOperation.Value is IConversionOperation { Operand: IThrowOperation { Exception: IConversionOperation exceptionConversionOperation } } &&
                          exceptionConversionOperation.Operand.Type!.Name.EndsWith(nameof(UnreachableCaseException)) &&
                          exceptionConversionOperation.Operand is IObjectCreationOperation objectCreationOperation &&
                          objectCreationOperation.Arguments.SingleOrDefault(
                              x =>
                                  x.Value is ITypeOfOperation typeOfOperation &&
                                  SymbolEqualityComparer.Default.Equals(typeOfOperation.TypeOperand, unionTypeWithoutNull)) != null))
                    {
                        context.ReportDiagnostic(Diagnostic.Create(SwitchShouldThrowInDefaultCaseRule, switchExpressionArmOperation.Syntax.GetLocation(), unionTypeWithoutNull));
                    }
                }*/

                return default;
            }).Where(x => x != null).Select(x => x!).ToList();

            /*if (!switchExpressionOperation.Arms.Any(x => x.Pattern is IDiscardPatternOperation))
            {
                context.ReportDiagnostic(Diagnostic.Create(SwitchShouldThrowInDefaultCaseRule, switchExpressionOperation.Syntax.GetLocation(), unionType));
            }*/

            var switchNullabilityError = EvaluateSwitchNullability(
                switchExpressionOperation.Value,
                switchExpressionOperation.SemanticModel,
                HasNullCase(switchExpressionOperation));

            var caseTypes = this.GetCaseTypes(unionTypeWithoutNull);

#pragma warning disable RS1024 // Compare symbols correctly
            ReportDiagnostics(
                new HashSet<ITypeSymbol>(caseTypes, SymbolEqualityComparer.Default),
                actualCaseTypes,
                switchNullabilityError,
                unionTypeWithoutNull,
                switchExpressionOperation,
                context);
#pragma warning restore RS1024 // Compare symbols correctly
        }

        public void AnalyzeSwitchStatement(OperationAnalysisContext context)
        {
            bool HasNullCase(ISwitchOperation switchOperation)
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

            if (!(context.Operation is ISwitchOperation switchOperation && switchOperation.SemanticModel != null))
            {
                return;
            }

            var unionTypeSymbol = switchOperation.Value.Type;
            var unionType = unionTypeSymbol as INamedTypeSymbol;

            if (!DiscriminatedUnionHelper.IsDiscriminatedUnionSwitch(unionType))
            {
                return;
            }

            var unionTypeWithoutNull = unionType.WithNullableAnnotation(NullableAnnotation.NotAnnotated);
            var actualCaseTypes = switchOperation.Cases.SelectMany(switchCaseOperation =>
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

                    if (caseClauseOperation is IDefaultCaseClauseOperation)
                    {
                        if (!(switchCaseOperation.Body.SingleOrDefault(x =>
                            x is IThrowOperation { Exception: IConversionOperation exceptionConversionOperation } &&
                            exceptionConversionOperation.Operand.Type!.Name.EndsWith(
                                nameof(UnreachableCaseException)) &&
                            exceptionConversionOperation.Operand is IObjectCreationOperation
                                objectCreationOperation &&
                            objectCreationOperation.Arguments.SingleOrDefault(
                                x =>
                                    x.Value is ITypeOfOperation typeOfOperation && SymbolEqualityComparer.Default.Equals(typeOfOperation.TypeOperand, unionTypeWithoutNull)) != null) != null))
                        {
                            context.ReportDiagnostic(Diagnostic.Create(
                                SundewDiscriminatedUnionsAnalyzer.SwitchShouldThrowInDefaultCaseRule,
                                switchCaseOperation.Syntax.GetLocation(),
                                unionTypeWithoutNull));
                        }
                    }

                    return null;
                })).Where(x => x != null).Select(x => x!).ToList();

            var switchNullabilityError = EvaluateSwitchNullability(switchOperation.Value, switchOperation.SemanticModel, HasNullCase(switchOperation));

            var caseTypes = this.GetCaseTypes(unionTypeWithoutNull);

#pragma warning disable RS1024 // Compare symbols correctly
            ReportDiagnostics(
                new HashSet<ITypeSymbol>(caseTypes, SymbolEqualityComparer.Default),
                actualCaseTypes,
                switchNullabilityError,
                unionTypeWithoutNull,
                switchOperation,
                context);
#pragma warning restore RS1024 // Compare symbols correctly
        }

        private static void ReportDiagnostics(
            ISet<ITypeSymbol> caseTypes,
            List<ITypeSymbol> actualCaseTypes,
            SwitchNullabilityError switchNullabilityError,
            ITypeSymbol unionType,
            IOperation operation,
            OperationAnalysisContext context)
        {
            foreach (var actualCaseType in actualCaseTypes)
            {
                if (!caseTypes.Remove(actualCaseType))
                {
                    // Should not be possible
                }
            }

            if (switchNullabilityError == SwitchNullabilityError.HasUnreachableNullCase)
            {
                context.ReportDiagnostic(Diagnostic.Create(
                    SundewDiscriminatedUnionsAnalyzer.HasUnreachableNullCaseRule,
                    operation.Syntax.GetLocation(),
                    DiagnosticSeverity.Error));
            }

            var isNullCaseMissing = switchNullabilityError == SwitchNullabilityError.IsMissingNullCase;
            var hasMultiple = caseTypes.Count + (isNullCaseMissing ? 1 : 0) > 1;
            if (caseTypes.Any() || isNullCaseMissing)
            {
                context.ReportDiagnostic(
                    Diagnostic.Create(
                        SundewDiscriminatedUnionsAnalyzer.AllCasesNotHandledRule,
                        operation.Syntax.GetLocation(),
                        caseTypes.OrderBy(x => x.Name).Aggregate(
                            new StringBuilder(),
                            (stringBuilder, typeSymbol) =>
                                stringBuilder.Append('\'').Append(typeSymbol.Name).Append('\'').Append(',')
                                    .Append(' '),
                            builder => isNullCaseMissing
                                ? builder.Append('\'').Append(Null).Append('\'').ToString()
                                : builder.ToString(0, builder.Length - 2)),
                        hasMultiple ? 's' : string.Empty,
                        unionType,
                        hasMultiple ? Are : Is));
            }
        }

        private static bool IsNullableEnabled(SemanticModel semanticModel, IOperation operation)
        {
            return (semanticModel.GetNullableContext(operation.Syntax.GetLocation()
                       .SourceSpan.Start) & NullableContext.Enabled) == NullableContext.Enabled ||
                   semanticModel.Compilation.Options.NullableContextOptions != NullableContextOptions.Disable;
        }

        private static bool IsNullLiteral(ILiteralOperation literalOperation)
        {
            return literalOperation.ConstantValue.HasValue &&
                   literalOperation.ConstantValue.Value == null;
        }

        private static SwitchNullabilityError EvaluateSwitchNullability(IOperation operation, SemanticModel semanticModel, bool hasNullCase)
        {
            var unionTypeInfo = semanticModel.GetTypeInfo(operation.Syntax);
            var isNullableEnabled = IsNullableEnabled(semanticModel, operation);
            var isNullable = unionTypeInfo.Nullability.Annotation != NullableAnnotation.NotAnnotated;
            var maybeNull = unionTypeInfo.Nullability.FlowState != NullableFlowState.NotNull;
            var nullableStates = (isNullableEnabled, isNullable, maybeNull, hasNullCase);
            var switchNullabilityError = nullableStates switch
            {
                (true, false, _, true) => SwitchNullabilityError.HasUnreachableNullCase,
                (_, _, true, false) => SwitchNullabilityError.IsMissingNullCase,
                (_, _, false, true) => SwitchNullabilityError.HasUnreachableNullCase,
                (_, _, true, true) => SwitchNullabilityError.None,
                (_, _, false, false) => SwitchNullabilityError.None,
            };
            return switchNullabilityError;
        }

        private HashSet<ITypeSymbol> GetCaseTypes(ITypeSymbol unionType)
        {
            return this.discriminatedUnions.GetOrAdd(unionType, x =>
            {
#pragma warning disable RS1024 // Compare symbols correctly
                var caseTypes = new HashSet<ITypeSymbol>(SymbolEqualityComparer.Default);
#pragma warning restore RS1024 // Compare symbols correctly
                foreach (var caseType in unionType.GetTypeMembers()
                    .Where(x => SymbolEqualityComparer.Default.Equals(x.BaseType, unionType)))
                {
                    caseTypes.Add(caseType);
                }

                return caseTypes;
            });
        }
    }
}