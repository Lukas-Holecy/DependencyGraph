// <copyright file="ProjectFinder.cs" company="Lukas Holecy">
// Copyright (c) Lukas Holecy. All rights reserved.
// </copyright>

namespace Holecy.Console.Dependencies.IO;

using System.IO.Abstractions;

/// <summary>
/// Provides functionality to locate C# project files within specified file and directory paths.
/// </summary>
internal class ProjectFinder(IFileSystem fileSystem)
{
    /// <summary>
    /// The file system abstraction used for file and directory operations.
    /// </summary>
    private readonly IFileSystem fileSystem = fileSystem;

    /// <summary>
    /// Finds all C# project files (*.csproj) within the provided collection of files and directories.
    /// </summary>
    /// <param name="filesAndDirectories">
    /// A collection of <see cref="IFileOrDirectoryInfo"/> instances representing files or directories to search for project files.
    /// </param>
    /// <returns>
    /// An enumerable of <see cref="IFileInfo"/> representing the found C# project files.
    /// </returns>
    public IEnumerable<IFileInfo> FindProjects(IEnumerable<IFileOrDirectoryInfo> filesAndDirectories)
    {
        List<IFileInfo> projectFiles = [];
        foreach (var fileOrDirectory in filesAndDirectories)
        {
            projectFiles.AddRange(this.FindProjects(fileOrDirectory));
        }

        return projectFiles;
    }

    /// <summary>
    /// Finds all C# project files (*.csproj) within the specified file or directory.
    /// </summary>
    /// <param name="fileOrDirectory">
    /// An <see cref="IFileOrDirectoryInfo"/> instance representing a single file or directory to search for project files.
    /// </param>
    /// <returns>
    /// An enumerable of <see cref="IFileInfo"/> representing the found C# project files.
    /// </returns>
    public IEnumerable<IFileInfo> FindProjects(IFileOrDirectoryInfo fileOrDirectory)
    {
        if (fileOrDirectory.IsDirectory)
        {
            return this.FindProjectsInDirectory(fileOrDirectory);
        }
        else
        {
            return this.ValidateProjectFile(fileOrDirectory) ? [(IFileInfo)fileOrDirectory.FileSystemInfo] : [];
        }
    }

    /// <summary>
    /// Searches for all C# project files (*.csproj) within the specified directory and its subdirectories.
    /// </summary>
    /// <param name="directory">
    /// An <see cref="IFileOrDirectoryInfo"/> instance representing the directory to search for project files.
    /// </param>
    /// <returns>
    /// An enumerable of <see cref="IFileInfo"/> representing the found C# project files.
    /// </returns>
    private HashSet<IFileInfo> FindProjectsInDirectory(IFileOrDirectoryInfo directory)
    {
        HashSet<IFileInfo> projectFiles = [];
        var fullPath = directory.FileSystemInfo.FullName;
        foreach (var file in this.fileSystem.Directory.EnumerateFiles(fullPath, "*.csproj", SearchOption.AllDirectories))
        {
            projectFiles.Add(this.fileSystem.FileInfo.New(file));
        }

        return projectFiles;
    }

    /// <summary>
    /// Validates whether the specified file represents a valid C# project file (*.csproj).
    /// </summary>
    /// <param name="fileOrDirectory">
    /// An <see cref="IFileOrDirectoryInfo"/> instance representing the file to validate.
    /// </param>
    /// <returns>
    /// <c>true</c> if the file exists and has a .csproj extension; otherwise, <c>false</c>.
    /// </returns>
    private bool ValidateProjectFile(IFileOrDirectoryInfo fileOrDirectory)
    {
        return this.fileSystem.File.Exists(fileOrDirectory.FileSystemInfo.FullName) &&
            fileOrDirectory.FileSystemInfo.Extension == ".csproj";
    }
}
