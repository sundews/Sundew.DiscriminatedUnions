// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Figure.cs" company="Sundews">
// Copyright (c) Sundews. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Sundew.DiscriminatedUnions.Development.Tester.ClosedTypes;

/// <summary>
/// A partial closed record, for which the factories that return a case as the closed record are generated.
/// The factory of a case that is nested in the closed record has a prefix, because the name of the case is taken by the case itself.
/// The list of its cases is generated as well, which a closed record does not have at runtime. The circle is partial, so it gets a list of its own.
/// </summary>
public closed partial record Figure
{
    public sealed partial record Circle(double Radius) : Figure;

    public sealed record Dot : Figure;
}

public sealed record Box(double Width, double Height) : Figure;
