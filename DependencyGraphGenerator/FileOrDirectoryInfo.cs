namespace Holecy.Console.Dependencies;

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
            FileSystemInfo = fileSystem.FileInfo.New(path);
            return;
        }

        if (fileSystem.Directory.Exists(path))
        {
            FileSystemInfo = fileSystem.DirectoryInfo.New(path);
            IsDirectory = true;
            return;
        }

        throw new ArgumentException("The specified path does not exist.", nameof(path));
    }
}
