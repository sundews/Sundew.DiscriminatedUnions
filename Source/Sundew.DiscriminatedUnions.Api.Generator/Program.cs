// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Program.cs" company="Sundews">
// Copyright (c) Sundews. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Sundew.DiscriminatedUnions.Api.Generator;

using System.Threading.Tasks;
using Sundew.Generator;
using Sundew.Generator.Code;
using Sundew.Generator.Input;
using Sundew.Generator.Output;

public static class Program
{
    public static Task Main()
    {
        return GeneratorFacade.RunAsync(new Setup
        {
            ModelSetup = new FolderModelSetup
            {
                Folder = "../../../../Sundew.DiscriminatedUnions.Shared/Api",
                Provider = new TextModelProvider(),
                FilesSearchPattern = "*.cs",
            },
            GeneratorSetups = new[]
            {
                new GeneratorSetup
                {
                    Generator = new PublicApiGenerator(),
                },
            },
            WriterSetups = new[]
            {
                new FileWriterSetup("../../../../Sundew.DiscriminatedUnions/Sundew.DiscriminatedUnions.csproj")
                {
                    Writer = new ProjectTextFileWriter(),
                    FileExtension = ".cs",
                    FileNameSuffix = ".g",
                    Folder = ".generated",
                },
            },
        });
    }
}