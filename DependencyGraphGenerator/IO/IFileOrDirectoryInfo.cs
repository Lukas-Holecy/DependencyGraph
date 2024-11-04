// <copyright file="IFileOrDirectoryInfo.cs" company="Lukas Holecy">
// "This is a .NET 8 project file."
// </copyright>

namespace Holecy.Console.Dependencies.IO;

using System.IO.Abstractions;

public interface IFileOrDirectoryInfo
{
    IFileSystemInfo FileSystemInfo { get; }

    bool IsDirectory { get; }
}
