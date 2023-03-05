// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DiscriminatedUnionResult.cs" company="Hukano">
// Copyright (c) Hukano. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Sundew.DiscriminatedUnions.Generator.ModelStage;

using Sundew.Base;
using Sundew.DiscriminatedUnions.Generator.Model;

internal readonly record struct DiscriminatedUnionResult(DiscriminatedUnion DiscriminatedUnion, ValueArray<DeclarationNotFound> Errors)
{
    public bool IsSuccess => this.DiscriminatedUnion != default;

    public static DiscriminatedUnionResult Success(DiscriminatedUnion discriminatedUnion) =>
        new DiscriminatedUnionResult(discriminatedUnion, default);

    public static DiscriminatedUnionResult Error(ValueArray<DeclarationNotFound> errors) => new DiscriminatedUnionResult(default, errors);
}
