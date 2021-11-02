// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DiscriminatedUnionRegistry.cs" company="Hukano">
// Copyright (c) Hukano. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Sundew.DiscriminatedUnions.Analyzer
{
    using System.Collections.Concurrent;
    using System.Collections.Immutable;
    using System.Diagnostics.CodeAnalysis;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.Diagnostics;
    using Microsoft.CodeAnalysis.Operations;

    internal class DiscriminatedUnionRegistry
    {
        [SuppressMessage("MicrosoftCodeAnalysisCorrectness", "RS1024:Compare symbols correctly", Justification = "False positive... using SymbolEqualityComparer")]
        private readonly ConcurrentDictionary<ITypeSymbol, ImmutableList<ITypeSymbol>> discriminatedUnionsAndCases = new(SymbolEqualityComparer.Default);

        public void Register(ITypeSymbol discriminatedUnionType, ITypeSymbol caseType)
        {
            this.discriminatedUnionsAndCases.AddOrUpdate(
                discriminatedUnionType,
                symbol => ImmutableList<ITypeSymbol>.Empty.Add(caseType),
                (symbol, list) => list.Add(caseType));
        }

        public ImmutableList<ITypeSymbol> GetCases(ITypeSymbol discriminatedUnionType)
        {
            if (this.discriminatedUnionsAndCases.TryGetValue(discriminatedUnionType, out var cases))
            {
                return cases;
            }

            return ImmutableList<ITypeSymbol>.Empty;
        }
    }
}