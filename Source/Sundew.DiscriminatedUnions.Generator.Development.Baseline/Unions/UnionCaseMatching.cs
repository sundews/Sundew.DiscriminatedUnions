// --------------------------------------------------------------------------------------------------------------------
// <copyright file="UnionCaseMatching.cs" company="Sundews">
// Copyright (c) Sundews. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Sundew.DiscriminatedUnions.Generator.Unions;

/// <summary>
/// The ways to find out which case a union is, which depends on the kind of union.
/// </summary>
internal enum UnionCaseMatching
{
    /// <summary>
    /// The union is the case, as it is for a closed type: the type of the instance tells the case.
    /// </summary>
    Type,

    /// <summary>
    /// The union contains the case as its value, as it does for a union declaration: the type of the value tells the case.
    /// </summary>
    Value,

    /// <summary>
    /// The union has a <c>TryGetValue</c> method per case, as a struct union does. It tells the case without boxing it.
    /// </summary>
    TryGetValue,
}
