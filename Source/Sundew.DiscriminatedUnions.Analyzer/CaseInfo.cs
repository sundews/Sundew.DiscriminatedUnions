﻿// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CaseInfo.cs" company="Sundews">
// Copyright (c) Sundews. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Sundew.DiscriminatedUnions.Analyzer;

using Microsoft.CodeAnalysis;

/// <summary>
/// Contains information about a case in a switch statement or expression.
/// </summary>
public readonly struct CaseInfo
{
    /// <summary>
    /// Gets the type.
    /// </summary>
    public ISymbol? Symbol { get; init; }

    /// <summary>
    /// Gets a value indicating whether the case is handled.
    /// </summary>
    public bool HandlesCase { get; init; }

    /// <summary>
    /// Gets the syntax node throwing NotImplementedException if present.
    /// </summary>
    public IOperation? ThrowingNotImplementedException { get; init; }
}