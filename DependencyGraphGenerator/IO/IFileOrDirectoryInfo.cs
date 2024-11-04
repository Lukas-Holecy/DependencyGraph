namespace Holecy.Console.Dependencies.IO;

using System.IO.Abstractions;

public interface IFileOrDirectoryInfo
{
    IFileSystemInfo FileSystemInfo { get; }
    bool IsDirectory { get; }
}
