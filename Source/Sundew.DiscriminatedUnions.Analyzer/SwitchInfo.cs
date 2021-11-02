// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SwitchInfo.cs" company="Hukano">
// Copyright (c) Hukano. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Sundew.DiscriminatedUnions.Analyzer
{
    using System.Collections.Generic;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.Operations;

    internal readonly struct SwitchInfo
    {
        public ISwitchOperation SwitchOperation { get; init; }

        public IEnumerable<CaseInfo> Cases { get; init; }

        public ISwitchCaseOperation? NullCase { get; init; }

        public SwitchNullability SwitchNullabilityError { get; init; }

        public SemanticModel SemanticModel { get; init; }
    }
}