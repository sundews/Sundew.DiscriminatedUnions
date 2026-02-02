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

public sealed partial record IntInput(int Value) : Input<IntError>;

public sealed partial record DoubleInput(double Value) : Input<DoubleError>;

[DiscriminatedUnion]
public abstract partial record DoubleError
{
    public sealed partial record OutOfRangeError() : DoubleError;

    public sealed partial record RoundingError() : DoubleError;
}

[DiscriminatedUnion]
public abstract partial record IntError
{
    public sealed partial record OutOfRangeError() : IntError;
}