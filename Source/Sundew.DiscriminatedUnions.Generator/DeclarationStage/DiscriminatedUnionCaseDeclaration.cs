// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DiscriminatedUnionCaseDeclaration.cs" company="Sundews">
// Copyright (c) Sundews. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Sundew.DiscriminatedUnions.Generator.DeclarationStage;

using Sundew.Base;
using Sundew.DiscriminatedUnions.Generator.Model;

internal readonly record struct DiscriminatedUnionCaseDeclaration(FullType CaseType, ValueArray<(Type Type, bool HasConflictingName)> Owners, ValueArray<Parameter> Parameters);