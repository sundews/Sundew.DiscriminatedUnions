// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CodeFixStatus.cs" company="Hukano">
// Copyright (c) Hukano. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Sundew.DiscriminatedUnions.CodeFixes
{
    [DiscriminatedUnion]
    internal record CodeFixStatus
    {
        private CodeFixStatus()
        {
        }

        internal record CanFix(string Title, string EquivalenceKey) : CodeFixStatus;

        internal record CannotFix : CodeFixStatus;
    }
}