// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DiscriminatedUnionProvider.cs" company="Sundews">
// Copyright (c) Sundews. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Sundew.DiscriminatedUnions.Generator.ModelStage;

using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Threading;
using Microsoft.CodeAnalysis;
using Sundew.Base.Collections.Immutable;
using Sundew.DiscriminatedUnions.Generator.DeclarationStage;
using Sundew.DiscriminatedUnions.Generator.Model;
using Type = Sundew.DiscriminatedUnions.Generator.Model.Type;

internal static class DiscriminatedUnionProvider
{
    public static IncrementalValueProvider<ImmutableArray<DiscriminatedUnionResult>> SetupDiscriminatedUnionStage(this IncrementalValueProvider<(ImmutableArray<DiscriminatedUnionDeclaration> Declarations, ImmutableArray<DiscriminatedUnionCaseDeclaration> Cases)> provider)
    {
        return provider.Select((tuple, token) =>
        {
            var cases = tuple.Cases;
            var declarations = tuple.Declarations;
            return GetDiscriminatedUnionResults(declarations, cases, token);
        });
    }

    internal static ImmutableArray<DiscriminatedUnionResult> GetDiscriminatedUnionResults(ImmutableArray<DiscriminatedUnionDeclaration> declarations, ImmutableArray<DiscriminatedUnionCaseDeclaration> cases, CancellationToken cancellationToken)
    {
        var discriminatedUnions = new ConcurrentDictionary<Type, DiscriminatedUnionResult>();
        foreach (var discriminatedUnionCase in cases)
        {
            var hasConflictingName = discriminatedUnionCase.Owners.Any(x => x.HasConflictingName);
            foreach (var owner in discriminatedUnionCase.Owners)
            {
                cancellationToken.ThrowIfCancellationRequested();
                var discriminatedUnionDeclaration = declarations.FirstOrDefault(x => x.Type.Equals(owner.Type));
                var discriminatedUnionResult = discriminatedUnionDeclaration != null
                    ? DiscriminatedUnionResult.Success(new DiscriminatedUnion(
                        new FullType(owner.Type, discriminatedUnionDeclaration.Type.TypeMetadata),
                        discriminatedUnionDeclaration.UnderlyingType,
                        discriminatedUnionDeclaration.Accessibility,
                        discriminatedUnionDeclaration.IsPartial,
                        discriminatedUnionDeclaration.IsConstrainingUnion,
                        discriminatedUnionDeclaration.GeneratorFeatures,
                        ImmutableArray.Create((Type: discriminatedUnionCase.CaseType, owner.ReturnType,
                            discriminatedUnionCase.Parameters, HasConflictingName: hasConflictingName))))
                    : DiscriminatedUnionResult.Error(
                        ImmutableArray.Create(new DeclarationNotFound(owner.Type, discriminatedUnionCase)));

                discriminatedUnions.AddOrUpdate(
                    owner.Type,
                    discriminatedUnionResult,
                    (type, result) =>
                    {
                        if (result.IsSuccess)
                        {
                            var discriminatedUnion = result.DiscriminatedUnion with
                            {
                                Cases = result.DiscriminatedUnion.Cases.Add(
                                    (Type: discriminatedUnionCase.CaseType, owner.ReturnType, discriminatedUnionCase.Parameters, HasConflictingName: hasConflictingName)),
                            };
                            return DiscriminatedUnionResult.Success(discriminatedUnion);
                        }

                        return DiscriminatedUnionResult.Error(discriminatedUnionResult.Errors.Add(
                            new DeclarationNotFound(type, discriminatedUnionCase)));
                    });
            }
        }

        return discriminatedUnions.Values.Select(x =>
        {
            if (x.IsSuccess)
            {
                return DiscriminatedUnionResult.Success(x.DiscriminatedUnion with
                {
                    Cases = GetDistinctOrdered(x.DiscriminatedUnion.Cases.OrderBy(x => x.Parameters.Any()).ThenBy(x => x.Type.Name)).ToImmutableArray(),
                });
            }

            return x;
        }).ToImmutableArray();
    }

    private static ImmutableArray<(FullType Type, FullType ReturnType, ValueArray<Parameter> Parameters, bool HasConflictingName)> GetDistinctOrdered(IEnumerable<(FullType Type, FullType ReturnType, ValueArray<Parameter> Parameters, bool HasConflictingName)> cases)
    {
        var hashSet = new HashSet<(FullType Type, FullType ReturnType, ValueArray<Parameter> Parameters, bool HasConflictingName)>();
        var immutableArrayBuilder = ImmutableArray.CreateBuilder<(FullType Type, FullType ReturnType, ValueArray<Parameter> Parameters, bool HasConflictingName)>();
        foreach (var valueTuple in cases)
        {
            if (hashSet.Add(valueTuple))
            {
                immutableArrayBuilder.Add(valueTuple);
            }
        }

        return immutableArrayBuilder.ToImmutable();
    }
}