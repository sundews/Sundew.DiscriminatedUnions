// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DiscriminatedUnionCaseDeclaration.cs" company="Hukano">
// Copyright (c) Hukano. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Sundew.DiscriminatedUnions.Generator.DeclarationStage;

using Sundew.Base;
using Sundew.DiscriminatedUnions.Generator.Model;

internal readonly record struct DiscriminatedUnionCaseDeclaration(FullType CaseType, ValueArray<(Type Type, string? GenerateFactoryMethodWithName)> Owners, ValueArray<Parameter> Parameters);