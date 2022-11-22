// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ProvideApiGenerator.cs" company="Hukano">
// Copyright (c) Hukano. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Sundew.DiscriminatedUnions.Analyzer.Api;

using System.IO;
using System.Linq;
using Microsoft.CodeAnalysis;
using System.Reflection;

/// <summary>
/// Provides the required types to create discriminated unions.
/// </summary>
[Generator]
public class ProvideApiGenerator : IIncrementalGenerator
{
    private const string File = "file=\"";

    /// <summary>
    /// Initializes the target project with the required types.
    /// </summary>
    /// <param name="context">The context.</param>
    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        context.RegisterPostInitializationOutput(context =>
        {
            var interfaceResources = Assembly.GetExecutingAssembly().GetManifestResourceNames().Where(x =>
                x.StartsWith($"{typeof(ProvideApiGenerator).Namespace}"));
            foreach (var interfaceResource in interfaceResources)
            {
                var stream = Assembly.GetExecutingAssembly().GetManifestResourceStream(interfaceResource);
                using StreamReader reader = new StreamReader(stream);
                var fileContent = reader.ReadToEnd();
                var filePath = $"{typeof(DiscriminatedUnion).Namespace}.";
                context.AddSource(filePath + interfaceResource.Substring(typeof(ProvideApiGenerator).Namespace.Length + 1), fileContent.Replace(File, File + filePath));
            }
        });
    }
}