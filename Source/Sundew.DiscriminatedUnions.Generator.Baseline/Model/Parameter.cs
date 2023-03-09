﻿// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Parameter.cs" company="Hukano">
// Copyright (c) Hukano. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Sundew.DiscriminatedUnions.Generator.Model;

internal readonly record struct Parameter(string TypeName, string Name, string? DefaultValue);