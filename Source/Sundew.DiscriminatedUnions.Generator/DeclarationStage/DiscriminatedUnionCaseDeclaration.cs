// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DiscriminatedUnionCaseDeclaration.cs" company="Sundews">
// Copyright (c) Sundews. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Sundew.DiscriminatedUnions.Generator.DeclarationStage;

using Sundew.Base.Collections.Immutable;
using Sundew.DiscriminatedUnions.Generator.Model;

internal readonly record struct DiscriminatedUnionCaseDeclaration(FullType CaseType, ValueArray<(Type Type, FullType ReturnType, bool HasConflictingName)> Owners, ValueArray<Parameter> Parameters);