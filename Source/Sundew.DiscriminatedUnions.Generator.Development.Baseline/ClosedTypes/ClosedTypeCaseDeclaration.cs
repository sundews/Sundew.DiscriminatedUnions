// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ClosedTypeCaseDeclaration.cs" company="Sundews">
// Copyright (c) Sundews. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Sundew.DiscriminatedUnions.Generator.ClosedTypes;

using Sundew.DiscriminatedUnions.Generator.Unions;

/// <summary>
/// Represents a partial case of a partial closed type. Such a case gets a list of cases of its own, which lists the case itself,
/// because the list it inherits from the closed type is the one of the closed type.
/// </summary>
/// <param name="Type">The case type.</param>
/// <param name="IsRecord">A value indicating whether the case is a record.</param>
/// <param name="TypeName">The fully qualified name of the case type, as it is written in generated code.</param>
internal sealed record ClosedTypeCaseDeclaration(UnionType Type, bool IsRecord, string TypeName);
