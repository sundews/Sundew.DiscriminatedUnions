// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TestData.cs" company="Hukano">
// Copyright (c) Hukano. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Sundew.DiscriminatedUnions.Test;

public static class TestData
{
    public const string UnionsResult = "Unions.Result";
    public const string UnionsOptionInt = "Unions.Option<int>";

    public const string Usings = @"
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using Sundew.DiscriminatedUnions;
using Sundew.DiscriminatedUnions.TestData;";

    public const string ValidResultDiscriminatedUnion = @"
[Sundew.DiscriminatedUnions.DiscriminatedUnion]
public abstract record Result
{ 
    private Result()
    { }

    public sealed record Success : Result;

    public sealed record Warning(string Message) : Result;

    public sealed record Error(int Code) : Result;
}";

    public const string ValidGenericOptionDiscriminatedUnion = @"
[Sundew.DiscriminatedUnions.DiscriminatedUnion]
public abstract record Option<T>
    where T : notnull
{
    private Option()
    { }

    public sealed record Some(T Value) : Option<T>;

    public sealed record None : Option<T>;
}";

    public const string ValidDiscriminatedUnionWithSubUnions = @"
[Sundew.DiscriminatedUnions.DiscriminatedUnion]
public abstract record Expression
{
    private protected Expression()
    { }

    public static Expression AdditionExpression(Expression lhs, Expression rhs) => new AdditionExpression(lhs, rhs);

    public static Expression SubtractionExpression(Expression lhs, Expression rhs) => new SubtractionExpression(lhs, rhs);
 
    public static Expression ValueExpression(int value) => new ValueExpression(value);
}

[Sundew.DiscriminatedUnions.DiscriminatedUnion]
public abstract record ArithmeticExpression : Expression
{
    private protected ArithmeticExpression()
    { }

    public static ArithmeticExpression AdditionExpression(Expression lhs, Expression rhs) => new AdditionExpression(lhs, rhs);

    public static ArithmeticExpression SubtractionExpression(Expression lhs, Expression rhs)
    {
        return new SubtractionExpression(lhs, rhs);
    }
}

public sealed record AdditionExpression(Expression Lhs, Expression Rhs) : ArithmeticExpression;

public sealed record SubtractionExpression(Expression Lhs, Expression Rhs) : ArithmeticExpression;

public sealed record ValueExpression(int Value) : Expression;";
}