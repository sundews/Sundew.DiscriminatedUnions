// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DiagnosticPropertyNames.cs" company="Sundews">
// Copyright (c) Sundews. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Sundew.DiscriminatedUnions.Analyzer;

/// <summary>
/// Property names to share data between analyzers and code fixes.
/// </summary>
public class DiagnosticPropertyNames
{
    /// <summary>
    /// Used to retrieve the case.
    /// </summary>
    public const string QualifiedCaseName = nameof(QualifiedCaseName);

    /// <summary>
    /// Used to retrieve the name.
    /// </summary>
    public const string Name = nameof(Name);
}