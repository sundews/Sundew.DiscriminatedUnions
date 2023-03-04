// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DemoProjectInfo.cs" company="Hukano">
// Copyright (c) Hukano. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Sundew.DiscriminatedUnions.Generator.IntegrationTests.Testing;

using System.IO;

public static class DemoProjectInfo
{
    public static string GetPath(string path)
    {
        var demoPath = Path.Combine(string.Format(@"..{0}..{0}..{0}..{0}..{0}..{0}..{0}..{0}", Path.DirectorySeparatorChar), path);
        if (!Directory.Exists(demoPath))
        {
            demoPath = Path.Combine(string.Format(@"..{0}..{0}..{0}..{0}", Path.DirectorySeparatorChar), path);
        }

        return demoPath;
    }
}