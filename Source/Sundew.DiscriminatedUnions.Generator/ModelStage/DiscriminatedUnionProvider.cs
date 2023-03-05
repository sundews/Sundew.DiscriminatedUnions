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
            var discriminatedUnions = new ConcurrentDictionary<Type, DiscriminatedUnionResult>();
            foreach (var discriminatedUnionCase in tuple.Cases)
            {
                foreach (var owner in discriminatedUnionCase.Owners)
                {
                    token.ThrowIfCancellationRequested();
                    var discriminatedUnionDeclaration = tuple.Declarations.FirstOrDefault(x => x.Type.Equals(owner));
                    var discriminatedUnionResult = discriminatedUnionDeclaration != null
                        ? DiscriminatedUnionResult.Success(new DiscriminatedUnion(
                            new FullType(owner, discriminatedUnionDeclaration.Type.TypeMetadata),
                            discriminatedUnionDeclaration.UnderlyingType,
                            discriminatedUnionDeclaration.Accessibility,
                            discriminatedUnionDeclaration.IsPartial,
                            discriminatedUnionDeclaration.IsConstrainingUnion,
                            discriminatedUnionDeclaration.GeneratorFeatures,
                            ImmutableArray.Create((Type: discriminatedUnionCase.CaseType,
                                discriminatedUnionCase.Parameters))))
                        : DiscriminatedUnionResult.Error(
                            ImmutableArray.Create(new DeclarationNotFound(owner, discriminatedUnionCase)));

                    discriminatedUnions.AddOrUpdate(
                        owner,
                        discriminatedUnionResult,
                        (type, result) =>
                        {
                            if (result.IsSuccess)
                            {
                                var discriminatedUnion = result.DiscriminatedUnion with
                                {
                                    Cases = result.DiscriminatedUnion.Cases.Add(
                                        (Type: discriminatedUnionCase.CaseType, discriminatedUnionCase.Parameters)),
                                };
                                return DiscriminatedUnionResult.Success(discriminatedUnion);
                            }

                            return DiscriminatedUnionResult.Error(discriminatedUnionResult.Errors.Add(
                                new DeclarationNotFound(type, discriminatedUnionCase)));
                        });
                }
            }

            return discriminatedUnions.Values.ToImmutableArray();
        });
    }
}