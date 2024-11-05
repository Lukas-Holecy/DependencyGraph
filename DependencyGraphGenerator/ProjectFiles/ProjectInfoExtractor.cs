// <copyright file="ProjectInfoExtractor.cs" company="Lukas Holecy">
// Copyright (c) Lukas Holecy. All rights reserved.
// </copyright>

namespace Holecy.Console.Dependencies.ProjectFiles;

using System.Collections.Generic;
using System.Diagnostics;
using System.IO.Abstractions;
using System.Xml;
using Microsoft.Build.Construction;
using Microsoft.Build.Evaluation;
using Microsoft.Build.Locator;

/// <summary>
/// Extracts information from a .net project file.
/// </summary>
internal static class ProjectInfoExtractor
{
    /// <summary>
    /// Initializes static members of the <see cref="ProjectInfoExtractor"/> class.
    /// This method registers the MSBuild instance, and it will only do it once.
    /// </summary>
    static ProjectInfoExtractor()
    {
        if (!MSBuildLocator.IsRegistered)
        {
            MSBuildLocator.RegisterDefaults();
        }
    }

    /// <summary>
    /// Tries to extract information from the specified project file.
    /// </summary>
    /// <param name="projectFile">Project file to be interrogated.</param>
    /// <param name="projectInformation">Information extracted from the project file.</param>
    /// <param name="fileSystem">File system to be used for file operations.</param>
    /// <returns>True if extraction was successful, false otherwise.</returns>
    internal static bool TryExtractProjectInformation(
        IFileInfo projectFile, out ProjectInformation? projectInformation, IFileSystem fileSystem)
    {
        ArgumentNullException.ThrowIfNull(projectFile, nameof(projectFile));

        projectInformation = null;

        if (!fileSystem.File.Exists(projectFile.FullName))
        {
            return false;
        }

        try
        {
            // Read remarks to see why it's done like this.
            projectInformation = ExtractProjectInformation(projectFile, fileSystem);
            return true;
        }
        catch (Exception ex)
        {
            ProcessException(ex);
            return false;
        }
    }

    /// <summary>
    /// Tries to extract information from the specified project file.
    /// </summary>
    /// <param name="projectFile">Project file to be interrogated.</param>
    /// <param name="projectInformation">Information extracted from the project file.</param>
    /// <returns>True if extraction was successful, false otherwise.</returns>
    /// <remarks>
    /// This method is needed, because if a method with Microsoft.Build method is called before the static constructor
    /// is called, it will fail.
    /// This is because the availability of used assemblies is done before the static constructor is called.
    /// </remarks>
    internal static bool TryExtractProjectInformation(IFileInfo projectFile, out ProjectInformation? projectInformation)
    {
        return TryExtractProjectInformation(projectFile, out projectInformation, new FileSystem());
    }

    private static void ProcessException(Exception ex)
    {
        // TODO Implement logging.
        // exception that can happen when reading the project file.
        // Microsoft.Build.Exceptions.InvalidProjectFileException)
        // PathTooLongException
        // UnauthorizedAccessException
        // NotSupportedException
        // System.Security.SecurityException
    }

    /// <summary>
    /// Tries to extract information from the specified project file.
    /// </summary>
    /// <param name="projectFile">Project file to be interrogated.</param>
    /// <returns>Information extracted from the project file.</returns>
    /// <remarks>
    /// This method can't be public, because if it's called before the static constructor is called, it will fail.
    /// This is because the availability of used assemblies is done before the static constructor is called.
    /// </remarks>
    private static ProjectInformation ExtractProjectInformation(IFileInfo projectFile, IFileSystem fileSystem)
    {
        string xmlContent = fileSystem.File.ReadAllText(projectFile.FullName);
        using StringReader stringReader = new StringReader(xmlContent);
        using XmlReader xmlReader = XmlReader.Create(stringReader);

        var projectRootElement = ProjectRootElement.Create(xmlReader);

        // Not using Project(string) constructor, because it isn't testable.
        Project project = new Project(projectRootElement);
        var projectInformation = new ProjectInformation
        {
            Name = project.GetPropertyValue("AssemblyName"),
            Path = projectFile.FullName,
            PackageId = project.GetPropertyValue("PackageId"),
            References = ExtractReferences(project, projectFile.FullName, fileSystem),
        };

        return projectInformation;
    }

    private static HashSet<IReference> ExtractReferences(Project project, string projectPath, IFileSystem fileSystem)
    {
        HashSet<IReference> references = ExtractPackageReferences(project);
        references.UnionWith(ExtractProjectReferences(project, projectPath, fileSystem));
        references.UnionWith(ExtractAssemblyReferences(project));

        return references;
    }

    private static HashSet<IReference> ExtractAssemblyReferences(Project project)
    {
        var references = new HashSet<IReference>();

        foreach (var item in project.GetItems("Reference"))
        {
            string assemblyName = item.EvaluatedInclude;
            references.Add(new AssemblyReference { Name = assemblyName });
        }

        return references;
    }

    private static HashSet<IReference> ExtractProjectReferences(
        Project project, string projectPath, IFileSystem fileSystem)
    {
        var references = new HashSet<IReference>();

        foreach (var item in project.GetItems("ProjectReference"))
        {
            var referencePath = item.EvaluatedInclude;
            if (!fileSystem.Path.IsPathFullyQualified(referencePath))
            {
                var projectDir = fileSystem.Path.GetDirectoryName(projectPath);
                referencePath = fileSystem.Path.Combine(projectDir, referencePath);
                referencePath = fileSystem.Path.GetFullPath(referencePath);
            }

            references.Add(new ProjectReference { Name = referencePath });
        }

        return references;
    }

    private static HashSet<IReference> ExtractPackageReferences(Project project)
    {
        var references = new HashSet<IReference>();

        foreach (var item in project.GetItems("PackageReference"))
        {
            string packageName = item.EvaluatedInclude;
            string version = item.GetMetadataValue("Version");
            references.Add(new PackageReference { Name = packageName, Version = version });
        }

        return references;
    }
}
