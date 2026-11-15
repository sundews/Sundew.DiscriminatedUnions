// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DiagnosticLocation.cs" company="Sundews">
// Copyright (c) Sundews. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Sundew.DiscriminatedUnions.Generator.Unions;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Text;

/// <summary>
/// A location that is comparable by value, so that it can be kept in the incremental generator pipeline.
/// </summary>
/// <param name="FilePath">The file path.</param>
/// <param name="TextSpan">The text span.</param>
/// <param name="LineSpan">The line span.</param>
internal readonly record struct DiagnosticLocation(string FilePath, TextSpan TextSpan, LinePositionSpan LineSpan)
{
    /// <summary>
    /// Creates a <see cref="DiagnosticLocation"/> from the specified location.
    /// </summary>
    /// <param name="location">The location.</param>
    /// <returns>The diagnostic location.</returns>
    public static DiagnosticLocation From(Location location)
    {
        var lineSpan = location.GetLineSpan();
        return new DiagnosticLocation(lineSpan.Path, location.SourceSpan, lineSpan.Span);
    }

    /// <summary>
    /// Converts this instance to a <see cref="Location"/>.
    /// </summary>
    /// <returns>The location.</returns>
    public Location ToLocation()
    {
        return Location.Create(this.FilePath, this.TextSpan, this.LineSpan);
    }
}
