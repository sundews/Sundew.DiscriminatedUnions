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
/// <param name="IsDefaultInterfaceMembersSupported">true if default interface member implementations are supported; otherwise, false.</param>
public sealed record FeatureSupport(bool IsDefaultInterfaceMembersSupported);