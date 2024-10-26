// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TypeMetadata.cs" company="Sundews">
// Copyright (c) Sundews. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Sundew.DiscriminatedUnions.Generator.Model;

using Sundew.Base.Collections.Immutable;
using Sundew.DiscriminatedUnions.Generator.DeclarationStage;

internal readonly record struct TypeMetadata(string FullName, ValueArray<TypeParameter> TypeParameters);