// --------------------------------------------------------------------------------------------------------------------
// <copyright file="UnionHelper.cs" company="Hukano">
// Copyright (c) Hukano. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Sundew.DiscriminatedUnions.Analyzer;

using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Operations;

/// <summary>
/// Helpers for analyzing discriminated unions.
/// </summary>
public static class UnionHelper
{
    /// <summary>
    /// Gets all case types.
    /// </summary>
    /// <param name="unionType">Type of the union.</param>
    /// <returns>All case types within the discriminated unions.</returns>
    public static IEnumerable<INamedTypeSymbol> GetKnownCaseTypes(ITypeSymbol unionType)
    {
        return GetFactoryMethodSymbols(unionType)
            .Select(x => TryGetCaseType(x.Attributes))
            .Where(x => x != null).Select(x => x!)
            .Concat(
                unionType.GetTypeMembers()
                    .Where(x =>
                    {
                        var baseType = x.BaseType;
                        while (baseType != null)
                        {
                            if (SymbolEqualityComparer.Default.Equals(baseType, unionType))
                            {
                                return true;
                            }

                            baseType = baseType.BaseType;
                        }

                        return false;
                    })).Distinct(SymbolEqualityComparer.Default)
            .Cast<INamedTypeSymbol>();
    }

    /// <summary>
    /// Tries to get the case for the specified method symbol.
    /// </summary>
    /// <param name="attributes">The attributes.</param>
    /// <returns>The case type.</returns>
    public static INamedTypeSymbol? TryGetCaseType(ImmutableArray<AttributeData> attributes)
    {
        var caseTypeAttribute = attributes.FirstOrDefault(x =>
            x.AttributeClass?.ToDisplayString() == typeof(CaseTypeAttribute).FullName);
        if (caseTypeAttribute != null)
        {
            var caseTypeSymbol = (INamedTypeSymbol?)caseTypeAttribute.ConstructorArguments.FirstOrDefault().Value ??
                                 (INamedTypeSymbol?)caseTypeAttribute.NamedArguments.FirstOrDefault().Value.Value;

            return caseTypeSymbol;
        }

        return null;
    }

    /// <summary>
    /// Gets factory method symbols for the union type.
    /// </summary>
    /// <param name="unionType">The union type.</param>
    /// <returns>The method symbols.</returns>
    public static IEnumerable<(string Name, ImmutableArray<AttributeData> Attributes, ITypeSymbol ResultType)> GetFactoryMethodSymbols(ITypeSymbol unionType)
    {
        return unionType.GetMembers()
            .Where(x => x.IsStatic)
            .Select(x =>
                {
                    if (x is IPropertySymbol propertySymbol)
                    {
                        return (Name: propertySymbol.Name, Attributes: propertySymbol.GetAttributes(), ResultType: propertySymbol.Type);
                    }

                    if (x is IMethodSymbol methodSymbol)
                    {
                        return (Name: methodSymbol.Name, Attributes: methodSymbol.GetAttributes(), ResultType: methodSymbol.ReturnType);
                    }

                    return default;
                })
               .Where(x => x != default && SymbolEqualityComparer.Default.Equals(x.ResultType, unionType));
    }

    /// <summary>
    /// Gets the instantiated case type symbol.
    /// </summary>
    /// <param name="factoryMethod">The factory method.</param>
    /// <param name="compilation">The compilation.</param>
    /// <returns>The type symbol for the instantiated type.</returns>
    public static INamedTypeSymbol? GetInstantiatedCaseTypeSymbol(IMethodSymbol factoryMethod, Compilation compilation)
    {
        static INamedTypeSymbol? TryGetActualReturnType(
            IBlockOperation? body,
            string name)
        {
            var operations = body?.Operations;
            if (operations != null && operations.Value.Length == 1 && operations.Value.LastOrDefault() is IReturnOperation
                {
                    ReturnedValue: IConversionOperation
                    {
                        Operand: IObjectCreationOperation
                        {
                            Type: INamedTypeSymbol namedTypeSymbol
                        }
                    }
                })
            {
                return namedTypeSymbol;
            }

            return null;
        }

        var name = factoryMethod.Name;
        return factoryMethod.DeclaringSyntaxReferences
            .Select(x => (
                methodBodyOperation: compilation.GetSemanticModel(x.SyntaxTree)
                    .GetOperation(x.GetSyntax()) as IMethodBodyOperation, name))
            .Where(x => x.methodBodyOperation != null)
            .Select(x => (methodBodyOperation: x.methodBodyOperation!, name))
            .Select(x =>
            {
                var blockBody = x.methodBodyOperation.BlockBody;
                if (blockBody != null)
                {
                    return TryGetActualReturnType(blockBody, name);
                }

                var expressionBody = x.methodBodyOperation.ExpressionBody;
                if (expressionBody != null)
                {
                    return TryGetActualReturnType(expressionBody, name);
                }

                return null;
            }).FirstOrDefault();
    }

    internal static SwitchNullability EvaluateSwitchNullability(
        IOperation operation,
        SemanticModel semanticModel,
        bool hasNullCase)
    {
        var unionTypeInfo = semanticModel.GetTypeInfo(operation.Syntax);
        var isNullableEnabled = IsNullableEnabled(semanticModel, operation);
        var isNullable = unionTypeInfo.ConvertedNullability.Annotation != NullableAnnotation.NotAnnotated;
        var maybeNull = unionTypeInfo.ConvertedNullability.FlowState != NullableFlowState.NotNull;
        var nullableStates = (isNullableEnabled, isNullable, maybeNull, hasNullCase);
        var switchNullability = nullableStates switch
        {
            (true, false, _, true) => SwitchNullability.HasUnreachableNullCase,
            (_, _, true, false) => SwitchNullability.IsMissingNullCase,
            (_, _, false, true) => SwitchNullability.HasUnreachableNullCase,
            (_, _, true, true) => SwitchNullability.None,
            (_, _, false, false) => SwitchNullability.None,
        };
        return switchNullability;
    }

    private static bool IsNullableEnabled(SemanticModel semanticModel, IOperation operation)
    {
        return (semanticModel.GetNullableContext(operation.Syntax.GetLocation()
                   .SourceSpan.Start) & NullableContext.Enabled) == NullableContext.Enabled ||
               semanticModel.Compilation.Options.NullableContextOptions != NullableContextOptions.Disable;
    }
}