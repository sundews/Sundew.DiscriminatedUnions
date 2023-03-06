// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DiagnosticDescriptorHelper.cs" company="Hukano">
// Copyright (c) Hukano. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Sundew.DiscriminatedUnions.Generator;

using Microsoft.CodeAnalysis;

/// <summary>
/// Helps creating diagnostic descriptors.
/// </summary>
public static class DiagnosticDescriptorHelper
{
    /// <summary>
    /// Creates the specified diagnostic identifier.
    /// </summary>
    /// <param name="diagnosticId">The diagnostic identifier.</param>
    /// <param name="titleResourceId">The title resource identifier.</param>
    /// <param name="messageFormatResourceId">The message format resource identifier.</param>
    /// <param name="category">The category.</param>
    /// <param name="diagnosticSeverity">The diagnostic severity.</param>
    /// <param name="isEnabledByDefault">if set to <c>true</c> [is enabled by default].</param>
    /// <param name="descriptionResourceId">The description resource identifier.</param>
    /// <returns>A Diagnostic descriptor.</returns>
    public static DiagnosticDescriptor Create(
        string diagnosticId,
        string titleResourceId,
        string messageFormatResourceId,
        string category,
        DiagnosticSeverity diagnosticSeverity,
        bool isEnabledByDefault,
        string descriptionResourceId)
    {
        return new DiagnosticDescriptor(
            diagnosticId,
            new LocalizableResourceString(titleResourceId, Resources.ResourceManager, typeof(Resources)),
            new LocalizableResourceString(messageFormatResourceId, Resources.ResourceManager, typeof(Resources)),
            category,
            diagnosticSeverity,
            isEnabledByDefault,
            new LocalizableResourceString(descriptionResourceId, Resources.ResourceManager, typeof(Resources)));
    }
}