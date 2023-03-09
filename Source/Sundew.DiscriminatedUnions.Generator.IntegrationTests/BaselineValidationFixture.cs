// --------------------------------------------------------------------------------------------------------------------
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
using Sundew.CodeAnalysis.Testing;

[TestFixture(Category = "MainBranchBuilds")]
public class BaselineValidationFixture
{
    [Test]
    public void VerifyWorkingCopyAndBaselineProjectsAreTheSame()
    {
        var workingCopyProject = new Project(DemoProjectInfo.GetPath("Sundew.DiscriminatedUnions.Generator"), new Paths(), "bin", "obj");

        var baselineProject = new Project(DemoProjectInfo.GetPath("Sundew.DiscriminatedUnions.Generator.Baseline"), new Paths(), "bin", "obj");

        var workingCopyFiles = workingCopyProject.GetFiles().ToList();
        var baselineFiles = baselineProject.GetFiles().ToList();

        workingCopyFiles.Select(x => x.Substring(workingCopyProject.ProjectDirectory.Length + 1)).Should().Equal(baselineFiles.Select(x => x.Substring(baselineProject.ProjectDirectory.Length + 1)));
        workingCopyFiles.Select(File.ReadAllText).Should().Equal(baselineFiles.Select(File.ReadAllText));
    }
}