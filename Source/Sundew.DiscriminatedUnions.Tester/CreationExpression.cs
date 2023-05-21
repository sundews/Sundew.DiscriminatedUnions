// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CreationExpression.cs" company="Sundews">
// Copyright (c) Sundews. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Sundew.Injection.Generator;

using System.Collections.Generic;
using System.Linq.Expressions;

[DiscriminatedUnions.DiscriminatedUnion]
internal abstract partial record CreationExpression
    (IReadOnlyList<Expression> Arguments) : InvocationExpressionBase(Arguments)
{
    public sealed record ArrayCreationExpression
        (string ArrayCreation, IReadOnlyList<Expression> Arguments) : CreationExpression(Arguments);
}