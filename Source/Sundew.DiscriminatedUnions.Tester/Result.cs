// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Result.cs" company="Hukano">
// Copyright (c) Hukano. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Sundew.DiscriminatedUnions.Tester;

[Sundew.DiscriminatedUnions.DiscriminatedUnion(GeneratorFeatures.Segregate)]
public abstract partial record Result<T>;

public sealed record Success<T>(T Value) : Result<T>;

public sealed record Warning<T>(string Message) : Result<T>;

[Sundew.DiscriminatedUnions.DiscriminatedUnion]
public abstract partial record ErrorResult<T> : Result<T>;

public sealed record Error<T>(int Code) : ErrorResult<T>;
public sealed record FatalError<T>(int Code) : ErrorResult<T>;