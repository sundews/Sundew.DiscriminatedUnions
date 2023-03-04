// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DiscriminatedUnion.cs" company="Hukano">
// Copyright (c) Hukano. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Sundew.DiscriminatedUnions.Generator.Model;

using Sundew.Base;
using Sundew.DiscriminatedUnions.Generator.DeclarationStage;

internal readonly record struct DiscriminatedUnion(
    Type Type,
    UnderlyingType UnderlyingType,
    Accessibility Accessibility,
    bool IsPartial,
    bool IsConstrainingUnion,
    GeneratorFeatures GeneratorFeatures,
    ValueArray<(Type Type, ValueArray<Parameter> Parameters)> Cases);