// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SwitchNullability.cs" company="Hukano">
// Copyright (c) Hukano. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Sundew.DiscriminatedUnions.Analyzer
{
    internal enum SwitchNullability
    {
        None,
        IsMissingNullCase,
        HasUnreachableNullCase,
    }
}