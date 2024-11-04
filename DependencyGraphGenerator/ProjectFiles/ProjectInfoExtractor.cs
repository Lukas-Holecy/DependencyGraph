// <copyright file="ProjectInfoExtractor.cs" company="Lukas Holecy">
// Copyright (c) Lukas Holecy. All rights reserved.
// </copyright>

namespace Holecy.Console.Dependencies.ProjectFiles;

using System.IO.Abstractions;

internal class ProjectInfoExtractor(IFileSystem fileSystem)
{
    private IFileSystem fileSystem = fileSystem;

    internal ProjectInformation ExtractProjectInformation(IFileInfo projectFile)
    {
        throw new NotImplementedException();
    }
}
