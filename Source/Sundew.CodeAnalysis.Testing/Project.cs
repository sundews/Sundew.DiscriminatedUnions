// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Project.cs" company="Hukano">
// Copyright (c) Hukano. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Sundew.CodeAnalysis.Testing;

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Text;

public class Project
{
    private readonly string projectName;
    private readonly Paths additionalPaths;
    private readonly string[] excludePaths;

    public Project(string basePath, Paths additionalPaths, params string[] excludePaths)
    {
        this.ProjectDirectory = Path.GetFullPath(basePath);
        this.additionalPaths = additionalPaths;
        this.projectName = Path.GetFileName(basePath);
        this.excludePaths = Array.ConvertAll(excludePaths, input => Path.GetFullPath(Path.Combine(basePath, input))).Concat(additionalPaths.FileSystemPaths.SelectMany(s => excludePaths, (s, s1) => Path.GetFullPath(Path.Combine(s, s1)))).ToArray();
    }

    public string ProjectDirectory { get; }

    public CSharpCompilation Compile()
    {
        return CSharpCompilation.Create(
            this.projectName,
            this.GetFiles().Select(x => CSharpSyntaxTree.ParseText(SourceText.From(File.ReadAllText(x)), null, x)),
            AppDomain.CurrentDomain.GetAssemblies().Where(x => !x.IsDynamic).Select(assembly => MetadataReference.CreateFromFile(assembly.Location)),
            new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary));
    }

    public IEnumerable<string> GetFiles()
    {
        return Directory.EnumerateFiles(this.ProjectDirectory, "*.cs", SearchOption.AllDirectories).Concat(this.additionalPaths.FileSystemPaths.SelectMany(x => Directory.EnumerateFiles(x, "*.cs", SearchOption.AllDirectories))).Where(this.IsNotExcluded).OrderBy(x => x);
    }

    public IEnumerable<string> GetFilesRelativeToProject()
    {
        return this.GetFiles().Select(x => x.Substring(this.ProjectDirectory.Length + 1));
    }

    private bool IsNotExcluded(string path)
    {
        return !this.excludePaths.Any(path.StartsWith);
    }
}