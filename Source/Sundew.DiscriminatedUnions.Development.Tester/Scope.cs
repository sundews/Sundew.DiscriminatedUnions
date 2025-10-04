// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Scope.cs" company="Sundews">
// Copyright (c) Sundews. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Sundew.DiscriminatedUnions.Development.Tester;

[Sundew.DiscriminatedUnions.DiscriminatedUnion]
public abstract partial record Scope
{
    [CaseType(typeof(AutoScope))]
    public static Scope Auto { get; } = new AutoScope();

    [Sundew.DiscriminatedUnions.CaseType(typeof(SingleInstancePerFuncResultScope))]
    public static Scope SingleInstancePerFuncResult(string method) => new SingleInstancePerFuncResultScope(method);

    internal sealed record AutoScope : Scope;

    internal sealed record SingleInstancePerFuncResultScope(string Method) : Scope;
}