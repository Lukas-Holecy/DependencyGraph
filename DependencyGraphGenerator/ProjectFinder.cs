namespace Holecy.Console.Dependencies;

using System.IO.Abstractions;

internal class ProjectFinder(IFileSystem fileSystem)
{
    private readonly IFileSystem fileSystem = fileSystem;

    public IEnumerable<IFileInfo> FindProjects(IEnumerable<FileOrDirectoryInfo> filesAndDirectories)
    {
        List<IFileInfo> projectFiles = [];
        foreach (var fileOrDirectory in filesAndDirectories)
        {
            projectFiles.AddRange(FindProjects(fileOrDirectory));
        }

        return projectFiles;
    }

    public IEnumerable<IFileInfo> FindProjects(FileOrDirectoryInfo fileOrDirectory)
    {
        if (fileOrDirectory.IsDirectory)
        {
            return FindProjectsInDirectory(fileOrDirectory);
        }
        else
        {
            return  ValidateProjectFile(fileOrDirectory) ? [(IFileInfo)fileOrDirectory.FileSystemInfo] : [];
        }
    }

    private IEnumerable<IFileInfo> FindProjectsInDirectory(FileOrDirectoryInfo directory)
    {
        HashSet<IFileInfo> projectFiles = [];
        var fullPath = directory.FileSystemInfo.FullName;
        foreach (var file in fileSystem.Directory.EnumerateFiles(fullPath, "*.csproj", SearchOption.AllDirectories))
        {
            projectFiles.Add(fileSystem.FileInfo.New(file));
        }

        return projectFiles;
    }

    private bool ValidateProjectFile(FileOrDirectoryInfo fileOrDirectory)
    {
        return fileSystem.File.Exists(fileOrDirectory.FileSystemInfo.FullName) &&
            fileOrDirectory.FileSystemInfo.Extension == ".csproj";
    }
}
