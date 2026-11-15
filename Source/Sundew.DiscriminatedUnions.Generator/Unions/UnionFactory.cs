// --------------------------------------------------------------------------------------------------------------------
// <copyright file="UnionFactory.cs" company="Sundews">
// Copyright (c) Sundews. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Sundew.DiscriminatedUnions.Generator.Unions;

using Sundew.Base.Collections.Immutable;
using Sundew.DiscriminatedUnions.Generator.Model;

/// <summary>
/// Represents a member of a union that creates the union from a new instance of one of its cases.
/// What it returns is known as the union, where creating the case directly gives a value that is known as the case.
/// It is a method that takes the parameters of the constructor of the case, or a property when that constructor takes none.
/// </summary>
/// <param name="Name">The name of the member, which is the name of the case type.</param>
/// <param name="CaseTypeName">The fully qualified name of the case type, as it is written in generated code.</param>
/// <param name="CaseDisplayName">The name of the case type, as it is written in documentation.</param>
/// <param name="IsPublic">A value indicating whether the member is public. Otherwise it is internal, like the constructor it calls.</param>
/// <param name="Parameters">The parameters of the constructor of the case.</param>
/// <param name="HasParameterArray">A value indicating whether the last parameter takes a variable number of arguments.</param>
/// <param name="IsCreatedOnce">A value indicating whether a factory without parameters creates its union once and keeps it, which it does when creating the union allocates.</param>
internal sealed record UnionFactory(string Name, string CaseTypeName, string CaseDisplayName, bool IsPublic, ValueArray<Parameter> Parameters, bool HasParameterArray, bool IsCreatedOnce);
