// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CodeFixStatus.cs" company="Sundews">
// Copyright (c) Sundews. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Sundew.DiscriminatedUnions.CodeFixes;

[DiscriminatedUnion]
internal abstract record CodeFixStatus
{
    private CodeFixStatus()
    {
    }

    internal sealed record CanFix(string Title, string EquivalenceKey) : CodeFixStatus;

    internal sealed record CannotFix : CodeFixStatus;
}