// --------------------------------------------------------------------------------------------------------------------
// <copyright file="UnhandledCases.cs" company="Sundews">
// Copyright (c) Sundews. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Sundew.DiscriminatedUnions.Analyzer.SwitchStatement;

using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using Microsoft.CodeAnalysis;

/// <summary>
/// The cases that a switch statement over a closed type or a union type does not handle.
/// </summary>
public readonly struct UnhandledCases
{
    private const string Null = "null";

    /// <summary>
    /// Initializes a new instance of the <see cref="UnhandledCases"/> struct.
    /// </summary>
    /// <param name="unionType">The closed type or union type that is switched on.</param>
    /// <param name="caseTypes">The case types that are not handled, in the order the union declares them.</param>
    /// <param name="isNullUnhandled">A value indicating whether the value can be null and null is not handled.</param>
    public UnhandledCases(INamedTypeSymbol unionType, ImmutableArray<ITypeSymbol> caseTypes, bool isNullUnhandled)
    {
        this.UnionType = unionType;
        this.CaseTypes = caseTypes;
        this.IsNullUnhandled = isNullUnhandled;
    }

    /// <summary>
    /// Gets the closed type or union type that is switched on.
    /// </summary>
    public INamedTypeSymbol UnionType { get; }

    /// <summary>
    /// Gets the case types that are not handled, in the order the union declares them.
    /// </summary>
    public ImmutableArray<ITypeSymbol> CaseTypes { get; }

    /// <summary>
    /// Gets a value indicating whether the value can be null and null is not handled.
    /// </summary>
    public bool IsNullUnhandled { get; }

    /// <summary>
    /// Gets a value indicating whether anything is not handled.
    /// </summary>
    public bool HasAny => !this.CaseTypes.IsDefaultOrEmpty || this.IsNullUnhandled;

    /// <summary>
    /// Gets the names of what is not handled, as they are reported.
    /// </summary>
    /// <returns>The names.</returns>
    public IReadOnlyList<string> GetNames()
    {
        var names = this.CaseTypes.IsDefault ? new List<string>() : this.CaseTypes.Select(LanguageUnionSwitchHelper.GetCaseName).ToList();
        if (this.IsNullUnhandled)
        {
            names.Add(Null);
        }

        return names;
    }
}
