// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IGenericOutParameter.cs" company="Sundews">
// Copyright (c) Sundews. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Sundew.DiscriminatedUnions.Development.Tester;

[DiscriminatedUnion(generatorFeatures: GeneratorFeatures.Segregate)]
public partial interface IGenericOutParameter<out T>
    where T : notnull
{
}

public sealed record GenericOutParameter<T>(T Value) : IGenericOutParameter<T>
    where T : notnull;

public sealed record SpecialGenericOutParameter<T>(T Value) : IGenericOutParameter<T>
    where T : notnull;