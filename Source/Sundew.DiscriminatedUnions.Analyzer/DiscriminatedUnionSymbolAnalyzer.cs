// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DiscriminatedUnionSymbolAnalyzer.cs" company="Hukano">
// Copyright (c) Hukano. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Sundew.DiscriminatedUnions.Analyzer
{
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.Collections.Immutable;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.Diagnostics;
    using Microsoft.CodeAnalysis.Operations;

    internal class DiscriminatedUnionSymbolAnalyzer
    {
        public IEnumerable<DiscriminatedUnionPart> AnalyzeSymbol(ISymbol symbol)
        {
            if (symbol is not INamedTypeSymbol namedTypeSymbol)
            {
                yield return new DiscriminatedUnionPart.None();
                yield break;
            }

            var baseType = namedTypeSymbol.BaseType;
            if (!DiscriminatedUnionHelper.IsDiscriminatedUnion(baseType))
            {
                yield return new DiscriminatedUnionPart.None();
                yield break;
            }

            if (namedTypeSymbol.IsSealed)
            {
                var discriminatedUnionType = namedTypeSymbol;
                while (discriminatedUnionType.BaseType != null)
                {
                    discriminatedUnionType = discriminatedUnionType.BaseType;
                    if (DiscriminatedUnionHelper.IsDiscriminatedUnion(discriminatedUnionType))
                    {
                        if (discriminatedUnionType.IsAbstract && discriminatedUnionType.TypeKind == TypeKind.Class)
                        {
                            yield return new DiscriminatedUnionPart.Valid(discriminatedUnionType, namedTypeSymbol);
                        }

                        yield return new DiscriminatedUnionPart.InvalidDeclaration(discriminatedUnionType);
                    }
                }
            }

            yield return new DiscriminatedUnionPart.None();
        }
    }
}