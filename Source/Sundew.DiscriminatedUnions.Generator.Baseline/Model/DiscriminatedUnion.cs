// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DiscriminatedUnion.cs" company="Sundews">
// Copyright (c) Sundews. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Sundew.DiscriminatedUnions.Generator.Model;

using Sundew.Base;

internal readonly record struct DiscriminatedUnion(
    FullType Type,
    UnderlyingType UnderlyingType,
    Accessibility Accessibility,
    bool IsPartial,
    bool IsConstrainingUnion,
    GeneratorFeatures GeneratorFeatures,
    ValueArray<(FullType Type, ValueArray<Parameter> Parameters, bool HasConflictingName)> Cases);