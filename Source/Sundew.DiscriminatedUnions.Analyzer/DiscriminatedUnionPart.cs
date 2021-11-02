// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DiscriminatedUnionPart.cs" company="Hukano">
// Copyright (c) Hukano. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Sundew.DiscriminatedUnions.Analyzer
{
    using Microsoft.CodeAnalysis;

    internal abstract record DiscriminatedUnionPart
    {
        public sealed record Valid(ITypeSymbol DiscriminatedUnionType, ITypeSymbol CaseType) : DiscriminatedUnionPart;

        public sealed record InvalidDeclaration(ITypeSymbol DiscriminatedUnionType) : DiscriminatedUnionPart;

        public sealed record None() : DiscriminatedUnionPart;
    }
}