﻿// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TestData.cs" company="Sundews">
// Copyright (c) Sundews. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Sundew.DiscriminatedUnions.Tests;

public static class TestData
{
    public const string UnionsResult = "Unions.Result";
    public const string UnionsState = "Unions.State";
    public const string UnionsOptionInt = "Unions.Option<int>";
    public const string UnionsOptionString = "Unions.Option<string>";
    public const string UnionsOptionT = "Unions.Option<T>";
    public const string ListCardinalityString = "Unions.ListCardinality<string>";

    public const string Usings = @"
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using Sundew.DiscriminatedUnions;
using Sundew.DiscriminatedUnions.TestData;";

    public const string ValidResultUnion = @"
[Sundew.DiscriminatedUnions.DiscriminatedUnion]
public abstract record Result
{ 
    private Result()
    { }

    public sealed record Success : Result;

    public sealed record Warning(string Message) : Result;

    public sealed record Error(int Code) : Result;
}";

    public const string ValidGenericOptionUnion = @"
[Sundew.DiscriminatedUnions.DiscriminatedUnion]
public abstract record Option<T>
    where T : notnull
{
    private Option()
    { }

    public sealed record Some(T Value) : Option<T>;

    public sealed record None : Option<T>;
}";

    public const string ValidMultiUnion = @"
[Sundew.DiscriminatedUnions.DiscriminatedUnion]
public abstract record Expression
{
    private protected Expression()
    { }

    [Sundew.DiscriminatedUnions.CaseTypeAttribute(typeof(AdditionExpression))]
    public static Expression AdditionExpression(Expression lhs, Expression rhs) => new AdditionExpression(lhs, rhs);

    [Sundew.DiscriminatedUnions.CaseTypeAttribute(typeof(SubtractionExpression))]
    public static Expression SubtractionExpression(Expression lhs, Expression rhs) => new SubtractionExpression(lhs, rhs);
 
    [Sundew.DiscriminatedUnions.CaseTypeAttribute(typeof(ValueExpression))]
    public static Expression ValueExpression(int value) => new ValueExpression(value);
}

[Sundew.DiscriminatedUnions.DiscriminatedUnion]
public abstract record ArithmeticExpression : Expression
{
    private protected ArithmeticExpression()
    { }

    [Sundew.DiscriminatedUnions.CaseTypeAttribute(typeof(AdditionExpression))]
    public static ArithmeticExpression AdditionExpression(Expression lhs, Expression rhs) => new AdditionExpression(lhs, rhs);

    [Sundew.DiscriminatedUnions.CaseTypeAttribute(typeof(SubtractionExpression))]
    public static ArithmeticExpression SubtractionExpression(Expression lhs, Expression rhs)
    {
        return new SubtractionExpression(lhs, rhs);
    }
}

public sealed record AdditionExpression(Expression Lhs, Expression Rhs) : ArithmeticExpression;

public sealed record SubtractionExpression(Expression Lhs, Expression Rhs) : ArithmeticExpression;

public sealed record ValueExpression(int Value) : Expression;";

    public const string ValidEnumUnion = @"
[Sundew.DiscriminatedUnions.DiscriminatedUnion]
public enum State
{ 
    None,
    
    On,

    Off
}";

    public const string ValidGenericListCardinalityUnion = @"
[Sundew.DiscriminatedUnions.DiscriminatedUnion]
public abstract partial class ListCardinality<TItem>
{
    [Sundew.DiscriminatedUnions.CaseType(typeof(Empty<>))]
    public static ListCardinality<TItem> Empty { get; }
        = new Empty<TItem>();

    [Sundew.DiscriminatedUnions.CaseType(typeof(Multiple<>))]
    public static ListCardinality<TItem> Multiple(global::System.Collections.Generic.IEnumerable<TItem> items)
        => new Multiple<TItem>(items);

    [Sundew.DiscriminatedUnions.CaseType(typeof(Single<>))]
    public static ListCardinality<TItem> Single(TItem item)
        => new Single<TItem>(item);
}

public sealed class Empty<TItem> : ListCardinality<TItem>
{
}

public sealed class Single<TItem> : ListCardinality<TItem>
{
    internal Single(TItem item)
    {
        this.Item = item;
    }

    public TItem Item { get; }
}

public sealed class Multiple<TItem> : ListCardinality<TItem>
{
    public Multiple(IEnumerable<TItem> items)
    {
        this.Items = items;
    }

    public IEnumerable<TItem> Items { get; }
}";
}