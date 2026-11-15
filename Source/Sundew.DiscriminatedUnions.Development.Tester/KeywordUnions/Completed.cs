// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Completed.cs" company="Sundews">
// Copyright (c) Sundews. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Sundew.DiscriminatedUnions.Development.Tester.KeywordUnions;

using System;
using System.Collections.Generic;

public readonly record struct Completed(int Code);

public sealed record Failed(string Reason);

/// <summary>
/// A partial union declaration, for which the list of cases, equality and ToString are generated.
/// It asks to be segregated as well.
/// </summary>
[DiscriminatedUnion(GeneratorFeatures.Segregate)]
public partial union Completion(Completed, Failed);

/// <summary>
/// A generic partial union declaration, which asks to be segregated.
/// </summary>
/// <typeparam name="TItem">The item type.</typeparam>
[DiscriminatedUnion(GeneratorFeatures.Segregate)]
public partial union Batch<TItem>(Completed, List<TItem>);

/// <summary>
/// A partial union declaration that describes itself, so ToString is not generated for it.
/// </summary>
public partial union Described(Completed, Failed)
{
    public override string ToString()
    {
        return "described";
    }
}

/// <summary>
/// A union declaration that is not partial, for which nothing is generated.
/// </summary>
public union Plain(Completed, Failed);

/// <summary>
/// A union declaration that is not partial and asks to be segregated. The types that do so are types of their own, so they are generated all the same.
/// An invalid operation exception is also an exception, so the more specific case has to be looked for first.
/// </summary>
[DiscriminatedUnion(GeneratorFeatures.Segregate)]
public union Sorted(Exception, InvalidOperationException, int?);
