// --------------------------------------------------------------------------------------------------------------------
// <copyright file="UnionHelper.cs" company="Sundews">
// Copyright (c) Sundews. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Sundew.DiscriminatedUnions.Analyzer;

using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Operations;
using Sundew.DiscriminatedUnions.Shared;

/// <summary>
/// Helpers for analyzing discriminated unions.
/// </summary>
public static class UnionHelper
{
    /// <summary>
    /// Gets all cases (Unions and Enums).
    /// </summary>
    /// <param name="unionType">Type of the union.</param>
    /// <returns>All case types within the discriminated unions.</returns>
    public static IEnumerable<ISymbol> GetKnownCases(ITypeSymbol unionType)
    {
        return PrivateGetKnownCaseTypes(unionType, true)
            .Concat(GetEnumMembers(unionType))
            .Distinct(SymbolEqualityComparer.Default);
    }

    /// <summary>
    /// Gets all case types.
    /// </summary>
    /// <param name="unionType">Type of the union.</param>
    /// <returns>All case types within the discriminated unions.</returns>
    public static IEnumerable<INamedTypeSymbol> GetKnownCaseTypes(ITypeSymbol unionType)
    {
        return PrivateGetKnownCaseTypes(unionType, true).Distinct<INamedTypeSymbol>(SymbolEqualityComparer.Default);
    }

    /// <summary>
    /// Gets all case types.
    /// </summary>
    /// <param name="unionType">Type of the union.</param>
    /// <returns>All case types within the discriminated unions.</returns>
    public static IEnumerable<INamedTypeSymbol> GetKnownExternalCaseTypes(ITypeSymbol unionType)
    {
        return PrivateGetKnownCaseTypes(unionType, false).Distinct<INamedTypeSymbol>(SymbolEqualityComparer.Default);
    }

    /// <summary>
    /// Tries to get the case for the specified method symbol.
    /// </summary>
    /// <param name="unionType">The union type.</param>
    /// <param name="attributes">The attributes.</param>
    /// <returns>The case type.</returns>
    public static INamedTypeSymbol? TryGetCaseType(ITypeSymbol unionType, ImmutableArray<AttributeData> attributes)
    {
        var caseTypeAttribute = attributes.FirstOrDefault(x =>
            x.AttributeClass?.ToDisplayString() == typeof(CaseTypeAttribute).FullName);
        if (caseTypeAttribute != null)
        {
            return GetEquatableCaseTypeForUnionType(caseTypeAttribute, unionType);
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

    internal static IEnumerable<(ISymbol? Symbol, bool HandlesCase, IOperation? ThrowingNotImplementedException)> GetHandledSymbols(INamedTypeSymbol? namedTypeSymbol, bool handlesCase, IOperation? throwingNotImplementedException)
    {
        if (namedTypeSymbol != null && namedTypeSymbol.IsDiscriminatedUnion())
        {
            foreach (var knownCase in UnionHelper.GetKnownCaseTypes(namedTypeSymbol))
            {
                yield return (knownCase, handlesCase, throwingNotImplementedException);
            }
        }

        yield return (namedTypeSymbol, handlesCase, throwingNotImplementedException);
    }

    internal static IEnumerable<(ISymbol? Symbol, bool HandlesCase, IOperation? ThrowingNotImplementedException)> GetHandledFieldTypeSymbols(IFieldSymbol fieldSymbol, IOperation? throwingNotImplementedException)
    {
        yield return (fieldSymbol, true, throwingNotImplementedException);
    }

    internal static ITypeSymbol GetNonNullableUnionType(INamedTypeSymbol unionType)
    {
        return unionType.OriginalDefinition.SpecialType == SpecialType.System_Nullable_T ? unionType.TypeArguments.Single() : unionType.WithNullableAnnotation(NullableAnnotation.NotAnnotated);
    }

    internal static IOperation? GetThrowingNotImplementedException(IOperation? implementationOperation)
    {
        if (implementationOperation == default)
        {
            return default;
        }

        if (implementationOperation is IConversionOperation conversionOperation)
        {
            implementationOperation = conversionOperation.Operand;
        }

        if (implementationOperation is IThrowOperation { Exception: IConversionOperation exceptionConversionOperation } &&
            exceptionConversionOperation.Operand.Type!.Name.EndsWith(
                nameof(NotImplementedException)) &&
            exceptionConversionOperation.Operand is IObjectCreationOperation)
        {
            return implementationOperation;
        }

        return default;
    }

    internal static SwitchNullability EvaluateSwitchNullability(
        IOperation operation,
        SemanticModel semanticModel,
        bool hasNullCase)
    {
        var unionTypeInfo = semanticModel.GetTypeInfo(operation.Syntax);
        var isNullableEnabled = IsNullableEnabled(semanticModel, operation);
        var isValueType = (unionTypeInfo.ConvertedType?.IsValueType ?? false) && unionTypeInfo.ConvertedType?.OriginalDefinition.SpecialType != SpecialType.System_Nullable_T;
        var isNullable = unionTypeInfo.ConvertedNullability.Annotation != NullableAnnotation.NotAnnotated;
        var maybeNull = unionTypeInfo.ConvertedNullability.FlowState != NullableFlowState.NotNull;
        var nullableStates = (isNullableEnabled, isNullable, maybeNull, isValueType, hasNullCase);
        var switchNullability = nullableStates switch
        {
            (_, _, _, true, true) => SwitchNullability.HasUnreachableNullCase,
            (_, _, _, true, false) => SwitchNullability.None,
            (true, false, _, _, true) => SwitchNullability.HasUnreachableNullCase,
            (_, _, true, _, false) => SwitchNullability.IsMissingNullCase,
            (_, _, false, _, true) => SwitchNullability.HasUnreachableNullCase,
            (_, _, true, _, true) => SwitchNullability.None,
            (_, _, false, _, false) => SwitchNullability.None,
        };
        return switchNullability;
    }

    internal static INamedTypeSymbol? GetEquatableCaseTypeForUnionType(AttributeData caseTypeAttribute, ITypeSymbol? unionType)
    {
        var caseTypeSymbol = (INamedTypeSymbol?)caseTypeAttribute.ConstructorArguments.FirstOrDefault().Value ??
                             (INamedTypeSymbol?)caseTypeAttribute.NamedArguments.FirstOrDefault().Value.Value;

        if (caseTypeSymbol != null && caseTypeSymbol.IsTypeGeneric() && unionType is INamedTypeSymbol namedUnionType && namedUnionType.TypeArguments.Length > 0)
        {
            if (SymbolEqualityComparer.Default.Equals(caseTypeSymbol.OriginalDefinition, unionType))
            {
                return caseTypeSymbol.OriginalDefinition.Construct(namedUnionType.TypeArguments, namedUnionType.TypeArgumentNullableAnnotations);
            }

            if (caseTypeSymbol.OriginalDefinition.TypeParameters.Length != namedUnionType.TypeArguments.Length)
            {
                return default;
            }

            caseTypeSymbol = caseTypeSymbol.OriginalDefinition.Construct(namedUnionType.TypeArguments, namedUnionType.TypeArgumentNullableAnnotations);
        }

        return caseTypeSymbol;
    }

    private static bool IsNullableEnabled(SemanticModel semanticModel, IOperation operation)
    {
        return (semanticModel.GetNullableContext(operation.Syntax.GetLocation()
                   .SourceSpan.Start) & NullableContext.Enabled) == NullableContext.Enabled ||
               semanticModel.Compilation.Options.NullableContextOptions != NullableContextOptions.Disable;
    }

    private static IEnumerable<INamedTypeSymbol> PrivateGetKnownCaseTypes(ITypeSymbol unionType, bool includeTypeMembers)
    {
        return GetFactoryMethodSymbols(unionType)
            .Select(x => TryGetCaseType(unionType, x.Attributes))
            .Where(x => x != null).Select(x => x!)
            .Concat(
                includeTypeMembers
                    ? unionType.GetTypeMembers()
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
                        })
                    : ImmutableArray<INamedTypeSymbol>.Empty);
    }

    private static IEnumerable<ISymbol> GetEnumMembers(ITypeSymbol unionType)
    {
        if (unionType is INamedTypeSymbol namedTypeSymbol && namedTypeSymbol.TypeKind == TypeKind.Enum)
        {
            return namedTypeSymbol.GetMembers().Where(x => x.Kind == SymbolKind.Field && x.IsStatic);
        }

        return Enumerable.Empty<ISymbol>();
    }
}