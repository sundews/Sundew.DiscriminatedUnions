// --------------------------------------------------------------------------------------------------------------------
// <copyright file="InvocationExpression.cs" company="Sundews">
// Copyright (c) Sundews. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Sundew.DiscriminatedUnions.Development.Tester;

using System;
using System.Collections.Generic;
using System.Linq.Expressions;

internal sealed record InvocationExpression(Expression Expression, IReadOnlyList<Expression> Arguments) : InvocationExpressionBase(Arguments)
{
    public InvocationExpression(Expression expression)
    : this(expression, Array.Empty<Expression>())
    {
    }
}