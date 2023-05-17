﻿// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TestProjects.cs" company="Hukano">
// Copyright (c) Hukano. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Sundew.DiscriminatedUnions.Generator.IntegrationTests;

using Microsoft.CodeAnalysis;
using Sundew.Testing.CodeAnalysis;
using Sundew.Testing.IO;

public static class TestProjects
{
    public static ProjectData SundewDiscriminatedUnionsTester { get; } = GetSundewDiscriminatedUnionsTester();

    private static ProjectData GetSundewDiscriminatedUnionsTester()
    {
        var project = new CSharpProject(Paths.FindPathUpwards("Sundew.DiscriminatedUnions.Tester"), new Paths(Paths.FindPathUpwards("Sundew.DiscriminatedUnions")), new Paths("bin", "obj"));
        var compilation = project.Compile();
        return new ProjectData(project, compilation);
    }

    public record ProjectData(CSharpProject Project, Compilation Compilation);
}