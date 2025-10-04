// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ScopeForGenerator.cs" company="Sundews">
// Copyright (c) Sundews. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Sundew.DiscriminatedUnions.Development.Tester;

[Sundew.DiscriminatedUnions.DiscriminatedUnion]
public abstract partial record ScopeForGenerator
{
    internal sealed record Auto : ScopeForGenerator;

    internal sealed record SingleInstancePerFuncResult(string Method) : ScopeForGenerator;
}