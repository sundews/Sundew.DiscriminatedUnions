﻿// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Result.cs" company="Sundews">
// Copyright (c) Sundews. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Sundew.DiscriminatedUnions.Tester;

[DiscriminatedUnion(GeneratorFeatures.Segregate)]
public abstract partial record Result<T>;

public sealed record Success<T>(T Value, T? Optional) : Result<T>;

public sealed record Warning<T>(string Message) : Result<T>;

[Sundew.DiscriminatedUnions.DiscriminatedUnion]
public abstract partial record ErrorResult<T> : Result<T>;

public sealed record Error<T>(int Code) : ErrorResult<T>;
public sealed record FatalError<T>(int Code) : ErrorResult<T>;