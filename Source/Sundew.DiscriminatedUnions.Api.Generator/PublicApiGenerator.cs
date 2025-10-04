// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PublicApiGenerator.cs" company="Sundews">
// Copyright (c) Sundews. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Sundew.DiscriminatedUnions.Api.Generator;

using System.Collections.Generic;
using System.IO;
using Sundew.Generator;
using Sundew.Generator.Code;
using Sundew.Generator.Core;

public class PublicApiGenerator : IGenerator<ISetup, IGeneratorSetup, IProject, string, IRun, ITextOutput>
{
    public IReadOnlyList<IRun> Prepare(ISetup setup, IGeneratorSetup generatorSetup, IProject target, string model, string modelOrigin)
    {
        return new IRun[] { new CodeRun(modelOrigin, Path.GetFileNameWithoutExtension(modelOrigin) + target.FileSuffix, null) };
    }

    public ITextOutput Generate(ISetup setup, IGeneratorSetup generatorSetup, IProject target, string model, IRun run, long index)
    {
        return new TextOutput(model.Replace("internal", "public"));
    }
}