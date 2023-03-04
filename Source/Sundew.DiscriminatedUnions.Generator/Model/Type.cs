// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Type.cs" company="Hukano">
// Copyright (c) Hukano. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Sundew.DiscriminatedUnions.Generator.Model;

using Sundew.Base;
using Sundew.DiscriminatedUnions.Generator.DeclarationStage;

internal readonly record struct Type(string Name, string Namespace, ValueArray<TypeParameter> TypeParameters, bool IsArray);