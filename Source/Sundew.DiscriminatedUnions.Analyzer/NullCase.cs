// --------------------------------------------------------------------------------------------------------------------
// <copyright file="NullCase.cs" company="Sundews">
// Copyright (c) Sundews. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Sundew.DiscriminatedUnions.Analyzer;

using Microsoft.CodeAnalysis;

/// <summary>
/// Contains information about a case in a switch statement or expression.
/// </summary>
public readonly record struct NullCase(IOperation NullOperation, IOperation? ThrowingNotImplementedException);