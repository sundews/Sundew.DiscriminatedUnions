// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TextModelProvider.cs" company="Sundews">
// Copyright (c) Sundews. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Sundew.DiscriminatedUnions.Api.Generator;

using System.Collections.Generic;
using System.Threading.Tasks;
using Sundew.Base.Collections;
using Sundew.Generator;
using Sundew.Generator.Input;
using Sundew.IO;

public class TextModelProvider : IModelProvider<ISetup, FolderModelSetup, string>
{
    public async Task<IReadOnlyList<IModelInfo<string>>> GetModelsAsync(ISetup setup, FolderModelSetup? modelSetup)
    {
        if (modelSetup == null)
        {
            return [];
        }

        var files = await Directory.EnumerateFilesAsync(modelSetup.Folder, modelSetup.FilesSearchPattern).ConfigureAwait(false);
        return await files.SelectAsync(async x => new ModelInfo<string>(await File.ReadAllTextAsync(x), x)).ConfigureAwait(false);
    }
}