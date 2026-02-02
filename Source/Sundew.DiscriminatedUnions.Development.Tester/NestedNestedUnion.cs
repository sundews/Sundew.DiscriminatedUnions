// --------------------------------------------------------------------------------------------------------------------
// <copyright file="NestedNestedUnion.cs" company="Sundews">
// Copyright (c) Sundews. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Sundew.DiscriminatedUnions.Development.Tester;

[DiscriminatedUnion]
public abstract partial record NestedNestedUnion
{
    [DiscriminatedUnion]
    public abstract partial record NestedUnionA : NestedNestedUnion
    {
        public sealed partial record CaseA1(int Value) : NestedUnionA;

        public sealed partial record CaseA2(string Text) : NestedUnionA;
    }
}
