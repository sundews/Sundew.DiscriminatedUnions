// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Container{TItem}.cs" company="Sundews">
// Copyright (c) Sundews. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Sundew.DiscriminatedUnions.Development.Tester.StructUnions;

/// <summary>
/// Contains a struct union that is nested in a generic type. A type nested in a generic type cannot have explicit layout,
/// so the two cases that do not depend on the item type share a struct that is declared next to the containing type. The wrapper depends on it and has a field of its own.
/// The types that segregate it are not nested, so they get the type parameter of the containing type, and they are internal as the union is.
/// </summary>
/// <typeparam name="TItem">The item type.</typeparam>
public partial class Container<TItem>
{
    [DiscriminatedUnion(GeneratorFeatures.Segregate)]
    internal readonly partial struct Nested
        : ICase<int>,
          ICase<long>,
          ICase<Wrapper>;

    internal readonly record struct Wrapper(TItem Item);
}
