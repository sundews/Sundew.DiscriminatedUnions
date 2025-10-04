// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Type.cs" company="Sundews">
// Copyright (c) Sundews. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Sundew.DiscriminatedUnions.Generator.Model;

internal readonly record struct Type(string Name, string Namespace, string AttributeName, string AssemblyAlias, int TypeParameterCount, bool IsArray);
