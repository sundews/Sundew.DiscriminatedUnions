// --------------------------------------------------------------------------------------------------------------------
// <copyright file="FeatureSupport.cs" company="Sundews">
// Copyright (c) Sundews. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Sundew.DiscriminatedUnions.Generator.DeclarationStage;

/// <summary>
/// Represents the set of feature flags indicating which language or runtime features are supported by the current
/// environment.
/// </summary>
/// <param name="StaticsMembersInInterfaceSupport">Describes the supported static members by the runtime.</param>
public sealed record FeatureSupport(StaticsMembersInInterfaceSupport StaticsMembersInInterfaceSupport);