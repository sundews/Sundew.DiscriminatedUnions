// --------------------------------------------------------------------------------------------------------------------
// <copyright file="StructUnionProblem.cs" company="Sundews">
// Copyright (c) Sundews. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Sundew.DiscriminatedUnions.Generator.StructUnions;

/// <summary>
/// The problems that prevent a struct union from being generated.
/// </summary>
internal enum StructUnionProblem
{
    /// <summary>
    /// The union is not declared as a partial struct.
    /// </summary>
    NotPartialStruct,

    /// <summary>
    /// Every case is a reference type, for which a union declaration is the better choice.
    /// </summary>
    AllCasesAreReferenceTypes,

    /// <summary>
    /// More than one case is declared as the default case.
    /// </summary>
    MultipleDefaultCases,

    /// <summary>
    /// The default case is not a struct without instance fields.
    /// </summary>
    DefaultCaseHasPayload,

    /// <summary>
    /// A case is a type parameter.
    /// </summary>
    TypeParameterCase,
}
