// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ContainingType.cs" company="Sundews">
// Copyright (c) Sundews. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Sundew.DiscriminatedUnions.Generator.Model;

using Sundew.Base.Collections.Immutable;

internal readonly record struct ContainingType(string Name, Accessibility Accessibility, UnderlyingType UnderlyingType, ValueArray<string> TypeParameters);