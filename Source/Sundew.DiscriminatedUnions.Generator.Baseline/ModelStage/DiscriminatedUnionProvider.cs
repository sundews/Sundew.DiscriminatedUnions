// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DiscriminatedUnionProvider.cs" company="Hukano">
// Copyright (c) Hukano. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Sundew.DiscriminatedUnions.Generator.ModelStage;

using System.Collections.Concurrent;
using System.Collections.Immutable;
using System.Linq;
using System.Threading;
using Microsoft.CodeAnalysis;
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
                        ImmutableArray.Create((Type: discriminatedUnionCase.CaseType,
                            discriminatedUnionCase.Parameters, GenerateFactoryMethodWithName: owner.GenerateFactoryMethodWithName))))
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
                                    (Type: discriminatedUnionCase.CaseType, discriminatedUnionCase.Parameters, GenerateFactoryMethodWithName: owner.GenerateFactoryMethodWithName)),
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
                    Cases = x.DiscriminatedUnion.Cases.Distinct().OrderBy(@case => @case.Type.Name).ToImmutableArray(),
                });
            }

            return x;
        }).ToImmutableArray();
    }
}