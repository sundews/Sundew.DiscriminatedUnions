// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TypeParameter.cs" company="Sundews">
// Copyright (c) Sundews. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Sundew.DiscriminatedUnions.Generator.DeclarationStage;

using Microsoft.CodeAnalysis;
using Sundew.Base.Collections.Immutable;

internal readonly record struct TypeParameter(string Name, VarianceKind VarianceKind, ValueArray<string> Constraints);