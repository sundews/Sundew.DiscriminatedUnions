// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Shape.cs" company="Sundews">
// Copyright (c) Sundews. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Sundew.DiscriminatedUnions.Development.Tester.StructUnions;

/// <summary>
/// A struct union without a default case. The circle and the rectangle are unmanaged and share their storage, the point needs none.
/// </summary>
public readonly partial struct Shape
    : ICase<Circle>,
      ICase<Rectangle>,
      ICase<Point>;

public readonly record struct Circle(double Radius);

public readonly record struct Rectangle(double Width, double Height);

public readonly record struct Point;
