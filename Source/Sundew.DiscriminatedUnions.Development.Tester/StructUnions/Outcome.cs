// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Outcome.cs" company="Sundews">
// Copyright (c) Sundews. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Sundew.DiscriminatedUnions.Development.Tester.StructUnions;

using System;

/// <summary>
/// A struct union that mixes a struct case with two reference cases, which share a single reference field.
/// It asks to be segregated, which gives each case a list, including the cases that are not declared here.
/// </summary>
[DiscriminatedUnion(GeneratorFeatures.Segregate)]
public readonly partial struct Outcome
    : ICase<Success>,
      ICase<string>,
      ICase<Exception>;

public readonly record struct Success(int Code);
