// <copyright file="ProjectFinder.cs" company="Lukas Holecy">
// "This is a .NET 8 project file."
// </copyright>

namespace Holecy.Console.Dependencies.IO;

using System.IO.Abstractions;

internal class ProjectFinder(IFileSystem fileSystem)
{
    private readonly IFileSystem fileSystem = fileSystem;

    public IEnumerable<IFileInfo> FindProjects(IEnumerable<IFileOrDirectoryInfo> filesAndDirectories)
    {
        List<IFileInfo> projectFiles = [];
        foreach (var fileOrDirectory in filesAndDirectories)
        {
            projectFiles.AddRange(this.FindProjects(fileOrDirectory));
        }

        return projectFiles;
    }

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

    private IEnumerable<IFileInfo> FindProjectsInDirectory(IFileOrDirectoryInfo directory)
    {
        HashSet<IFileInfo> projectFiles = [];
        var fullPath = directory.FileSystemInfo.FullName;
        foreach (var file in this.fileSystem.Directory.EnumerateFiles(fullPath, "*.csproj", SearchOption.AllDirectories))
        {
            projectFiles.Add(this.fileSystem.FileInfo.New(file));
        }

        return projectFiles;
    }

    private bool ValidateProjectFile(IFileOrDirectoryInfo fileOrDirectory)
    {
        return this.fileSystem.File.Exists(fileOrDirectory.FileSystemInfo.FullName) &&
            fileOrDirectory.FileSystemInfo.Extension == ".csproj";
    }
}
