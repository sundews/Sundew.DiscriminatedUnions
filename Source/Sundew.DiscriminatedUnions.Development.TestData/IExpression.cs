// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IExpression.cs" company="Sundews">
// Copyright (c) Sundews. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Sundew.DiscriminatedUnions.Development.TestData;

[Sundew.DiscriminatedUnions.DiscriminatedUnion]
public interface IExpression
{
    [Sundew.DiscriminatedUnions.CaseTypeAttribute(typeof(AdditionExpression))]
    public static IExpression AdditionExpression(IExpression lhs, IExpression rhs) => new AdditionExpression(lhs, rhs);

    [Sundew.DiscriminatedUnions.CaseTypeAttribute(typeof(SubtractionExpression))]
    public static IExpression SubtractionExpression(IExpression lhs, IExpression rhs) => new SubtractionExpression(lhs, rhs);

    [CaseType(typeof(MultiplicationExpression))]
    public static IExpression MultiplicationExpression(IExpression lhs, IExpression rhs) => new MultiplicationExpression(lhs, rhs);

    [Sundew.DiscriminatedUnions.CaseTypeAttribute(typeof(ValueExpression))]
    public static IExpression ValueExpression(int value) => new ValueExpression(value);
}

[DiscriminatedUnion]
public interface IArithmeticExpression : IExpression
{
    IExpression Lhs { get; init; }

    IExpression Rhs { get; init; }

    [Sundew.DiscriminatedUnions.CaseTypeAttribute(typeof(AdditionExpression))]
    public static new IArithmeticExpression AdditionExpression(IExpression lhs, IExpression rhs) => new AdditionExpression(lhs, rhs);

    [Sundew.DiscriminatedUnions.CaseTypeAttribute(typeof(SubtractionExpression))]
    public static new IArithmeticExpression SubtractionExpression(IExpression lhs, IExpression rhs) => new SubtractionExpression(lhs, rhs);

    [Sundew.DiscriminatedUnions.CaseTypeAttribute(typeof(MultiplicationExpression))]
    public static new IArithmeticExpression MultiplicationExpression(IExpression lhs, IExpression rhs) => new MultiplicationExpression(lhs, rhs);
}

public sealed record AdditionExpression(IExpression Lhs, IExpression Rhs) : IArithmeticExpression;

public sealed record SubtractionExpression(IExpression Lhs, IExpression Rhs) : IArithmeticExpression;

public sealed record MultiplicationExpression(IExpression Lhs, IExpression Rhs) : IArithmeticExpression;

public sealed record ValueExpression(int Value) : IExpression;