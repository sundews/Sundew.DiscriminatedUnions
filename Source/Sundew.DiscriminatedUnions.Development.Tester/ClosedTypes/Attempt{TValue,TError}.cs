// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Attempt{TValue,TError}.cs" company="Sundews">
// Copyright (c) Sundews. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Sundew.DiscriminatedUnions.Development.Tester.ClosedTypes;

/// <summary>
/// A generic partial closed record. Its factories are declared in the closed record, so they use its type parameters.
/// The types that segregate it are declared next to it, so they repeat its type parameters and their constraints.
/// </summary>
/// <typeparam name="TValue">The type of the value.</typeparam>
/// <typeparam name="TError">The type of the error.</typeparam>
[DiscriminatedUnion(GeneratorFeatures.Segregate)]
public closed partial record Attempt<TValue, TError>
    where TValue : notnull
{
    public sealed record Succeeded(TValue Value) : Attempt<TValue, TError>;
}

public sealed record Faulted<TValue, TError>(TError Error) : Attempt<TValue, TError>
    where TValue : notnull;
