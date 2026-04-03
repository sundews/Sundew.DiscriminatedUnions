// --------------------------------------------------------------------------------------------------------------------
// <copyright file="FeatureSupportProvider.cs" company="Sundews">
// Copyright (c) Sundews. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Sundew.DiscriminatedUnions.Generator.DeclarationStage;

using System.Text.RegularExpressions;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;

internal static partial class FeatureSupportProvider
{
    private const string FrameworkName = "Framework";
    private const string MajorName = "Major";
    private const string Net = "net";
    private const string BuildPropertyTargetFrameworkName = "build_property.TargetFramework";
    private static readonly Regex TargetFrameworkRegex = new("(?<Framework>\\D+)(?<Major>\\d+)\\.(?<Minor>\\d)", RegexOptions.Compiled);

    public static IncrementalValueProvider<FeatureSupport> SetupFeatureSupportStage(this IncrementalValueProvider<AnalyzerConfigOptionsProvider> analyzerConfigOptionsProvider)
    {
        return analyzerConfigOptionsProvider.Select((x, _) =>
        {
            if (x.GlobalOptions.TryGetValue(BuildPropertyTargetFrameworkName, out var targetFramework))
            {
                var match = TargetFrameworkRegex.Match(targetFramework);
                if (match.Success)
                {
                    var framework = match.Groups[FrameworkName].Value;
                    var major = int.Parse(match.Groups[MajorName].Value);
                    var staticsMembersInInterfaceSupport = (framework, major) switch
                    {
                        (Net, >= 8) => StaticsMembersInInterfaceSupport.StaticAbstractInterfaceMember,
                        (Net, < 8) => StaticsMembersInInterfaceSupport.DefaultInterfaceMember,
                        _ => StaticsMembersInInterfaceSupport.None,
                    };
                    return new FeatureSupport(staticsMembersInInterfaceSupport);
                }
            }

            return new FeatureSupport(StaticsMembersInInterfaceSupport.None);
        });
    }
}