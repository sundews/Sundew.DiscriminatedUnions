// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Animal.cs" company="Sundews">
// Copyright (c) Sundews. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Sundew.DiscriminatedUnions.Development.Tester.ClosedTypes;

/// <summary>
/// A partial closed record with a case that is a closed record itself. The cases of that one are cases of both,
/// so both get a factory for them, each returning its own type. The same goes for the list of cases and for what segregates them.
/// A case that is partial gets a list of cases of its own, which lists the case itself: the dog is partial and the cat is not.
/// </summary>
[DiscriminatedUnion(GeneratorFeatures.Segregate)]
public closed partial record Animal;

[DiscriminatedUnion(GeneratorFeatures.Segregate)]
public closed partial record Mammal : Animal;

public sealed partial record Dog(string Name) : Mammal;

public sealed record Cat(string Name) : Mammal;

public sealed record Fish : Animal;
