// <copyright file="ProjectInfoExtractor.cs" company="Lukas Holecy">
// Copyright (c) Lukas Holecy. All rights reserved.
// </copyright>

namespace Holecy.Console.Dependencies.ProjectFiles;

using System.IO.Abstractions;
using Microsoft.Build.Evaluation;
using Microsoft.Build.Locator;

internal class ProjectInfoExtractor(IFileSystem fileSystem)
{
    private IFileSystem fileSystem = fileSystem;

    /// <summary>
    /// Stastic constructor that registers MSBuild.
    /// </summary>
    static ProjectInfoExtractor()
    {
        // Check if MSBuild is already registered
        if (!MSBuildLocator.IsRegistered)
        {
            MSBuildLocator.RegisterDefaults();
        }
    }

    internal bool TryExtractProjectInformation(IFileInfo projectFile, out ProjectInformation? projectInformation)
    {
        projectInformation = null;

        try
        {
            Project project = new Project(projectFile.FullName);
            projectInformation = new ProjectInformation
            {
                Name = project.GetPropertyValue("AssemblyName"),
                Path = projectFile.FullName,
                PackageId = project.GetPropertyValue("PackageId"),
            };

            return true;
        }
        catch (Exception)
        {
            return false;
        }
    }
}
