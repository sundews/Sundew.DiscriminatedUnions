﻿// --------------------------------------------------------------------------------------------------------------------
// <copyright file="BaselineValidationFixture.cs" company="Hukano">
// Copyright (c) Hukano. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Sundew.DiscriminatedUnions.Generator.IntegrationTests;

using System.IO;
using System.Linq;
using FluentAssertions;
using NUnit.Framework;
using Sundew.Testing.CodeAnalysis;
using Sundew.Testing.IO;

[TestFixture(Category = "MainBranchBuilds")]
public class BaselineValidationFixture
{
    [Test]
    public void VerifyWorkingCopyAndBaselineProjectsAreTheSame()
    {
        var workingCopyProject = new CSharpProject(Paths.FindPathUpwards("Sundew.DiscriminatedUnions.Generator"), new Paths(), new Paths("bin", "obj"), null);

        var baselineProject = new CSharpProject(Paths.FindPathUpwards("Sundew.DiscriminatedUnions.Generator.Baseline"), new Paths(), new Paths("bin", "obj"), null);

        var workingCopyFiles = workingCopyProject.GetFiles().ToList();
        var baselineFiles = baselineProject.GetFiles().ToList();

        workingCopyFiles.Select(x => x.Substring(workingCopyProject.Directory.Length + 1)).Should().Equal(baselineFiles.Select(x => x.Substring(baselineProject.Directory.Length + 1)));
        workingCopyFiles.Select(File.ReadAllText).Should().Equal(baselineFiles.Select(File.ReadAllText));
    }
}