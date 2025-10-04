// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Input.cs" company="Sundews">
// Copyright (c) Sundews. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Sundew.DiscriminatedUnions.Development.Tester;

[DiscriminatedUnion]
public abstract partial record Input<TError>()
{
}

public sealed record IntInput(int Value) : Input<IntError>;

public sealed record DoubleInput(double Value) : Input<DoubleError>;

[DiscriminatedUnion]
public abstract partial record DoubleError
{
    public sealed record OutOfRangeError() : DoubleError;

    public sealed record RoundingError() : DoubleError;
}

[DiscriminatedUnion]
public abstract partial record IntError
{
    public sealed record OutOfRangeError() : IntError;
}