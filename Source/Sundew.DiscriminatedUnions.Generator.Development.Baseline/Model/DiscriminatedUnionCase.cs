// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DiscriminatedUnionCase.cs" company="Sundews">
// Copyright (c) Sundews. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Sundew.DiscriminatedUnions.Generator.Model;

using Sundew.Base.Collections.Immutable;

internal sealed record DiscriminatedUnionCase(
    Accessibility Accessibility,
    UnderlyingType UnderlyingType,
    FullType Type,
    FullType ReturnType,
    ValueArray<Parameter> Parameters,
    bool HasConflictingName,
    bool IsPartial,
    bool HasImplementationTypeOwner);