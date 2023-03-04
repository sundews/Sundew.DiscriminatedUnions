// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DeclarationNotFound.cs" company="Hukano">
// Copyright (c) Hukano. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Sundew.DiscriminatedUnions.Generator.ModelStage;

using Sundew.DiscriminatedUnions.Generator.DeclarationStage;
using Sundew.DiscriminatedUnions.Generator.Model;

internal readonly record struct DeclarationNotFound(Type Owner, DiscriminatedUnionCaseDeclaration DiscriminatedUnionCaseDeclaration);
