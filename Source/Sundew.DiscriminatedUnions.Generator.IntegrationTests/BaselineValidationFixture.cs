// --------------------------------------------------------------------------------------------------------------------
// <copyright file="BaselineValidationFixture.cs" company="Sundews">
// Copyright (c) Sundews. All rights reserved.
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
using Xunit;

[Trait("Category", "MainBranchBuilds")]
public class BaselineValidationFixture
{
    [Fact]
    public void VerifyWorkingCopyAndBaselineProjectsAreTheSame()
    {
        var workingCopyProject = new CSharpProject(Paths.FindPathUpwards("Sundew.DiscriminatedUnions.Generator"), new Paths(), new Paths("bin", "obj"));

        var baselineProject = new CSharpProject(Paths.FindPathUpwards("Sundew.DiscriminatedUnions.Generator.Baseline"), new Paths(), new Paths("bin", "obj"), null);

        var workingCopyFiles = workingCopyProject.GetFiles().ToList();
        var baselineFiles = baselineProject.GetFiles().ToList();

        workingCopyFiles.Select(x => x.Substring(workingCopyProject.BasePath.Length + 1)).Should().Equal(baselineFiles.Select(x => x.Substring(baselineProject.BasePath.Length + 1)));
        workingCopyFiles.Select(File.ReadAllText).Should().Equal(baselineFiles.Select(File.ReadAllText));
    }
}