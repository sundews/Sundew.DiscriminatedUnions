// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DiscriminatedUnionPartAnalyzer.cs" company="Hukano">
// Copyright (c) Hukano. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Sundew.DiscriminatedUnions.Analyzer
{
    using System;
    using System.Collections.Generic;

    internal class DiscriminatedUnionPartAnalyzer
    {
        private readonly DiscriminatedUnionRegistry discriminatedUnionRegistry;

        public DiscriminatedUnionPartAnalyzer(DiscriminatedUnionRegistry discriminatedUnionRegistry)
        {
            this.discriminatedUnionRegistry = discriminatedUnionRegistry;
        }

        public void AnalyzePart(IEnumerable<DiscriminatedUnionPart> discriminatedUnionParts)
        {
            foreach (var discriminatedUnionPart in discriminatedUnionParts)
            {
                switch (discriminatedUnionPart)
                {
                    case DiscriminatedUnionPart.InvalidDeclaration invalidDeclaration:
                        // Report error
                        break;
                    case DiscriminatedUnionPart.None none:
                        break;
                    case DiscriminatedUnionPart.Valid valid:
                        this.discriminatedUnionRegistry.Register(valid.DiscriminatedUnionType, valid.CaseType);
                        break;
                    default:
                        throw new ArgumentOutOfRangeException(nameof(discriminatedUnionPart));
                }
            }
        }
    }
}