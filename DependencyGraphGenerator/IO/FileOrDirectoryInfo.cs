// <copyright file="FileOrDirectoryInfo.cs" company="Lukas Holecy">
// "This is a .NET 8 project file."
// </copyright>

namespace Holecy.Console.Dependencies.IO;

using System.IO.Abstractions;

internal class FileOrDirectoryInfo
{
    public IFileSystemInfo FileSystemInfo { get; init; }

    public bool IsDirectory { get; init; } = false;

    public FileOrDirectoryInfo(string path)
    : this(path, new FileSystem())
    {
    }

    public FileOrDirectoryInfo(string path, IFileSystem fileSystem)
    {
        if (fileSystem.File.Exists(path))
        {
            this.FileSystemInfo = fileSystem.FileInfo.New(path);
            return;
        }

        if (fileSystem.Directory.Exists(path))
        {
            this.FileSystemInfo = fileSystem.DirectoryInfo.New(path);
            this.IsDirectory = true;
            return;
        }

        throw new ArgumentException("The specified path does not exist.", nameof(path));
    }
}
