// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DiscriminatedUnionDeclaration.cs" company="Sundews">
// Copyright (c) Sundews. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Sundew.DiscriminatedUnions.Generator.DeclarationStage;

using Sundew.DiscriminatedUnions.Generator.Model;

internal readonly record struct DiscriminatedUnionDeclaration(FullType Type, UnderlyingType UnderlyingType, Accessibility Accessibility, bool IsPartial, bool IsConstrainingUnion, GeneratorFeatures GeneratorFeatures);