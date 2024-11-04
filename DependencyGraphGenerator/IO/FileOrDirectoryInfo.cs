// <copyright file="FileOrDirectoryInfo.cs" company="Lukas Holecy">
// Copyright (c) Lukas Holecy. All rights reserved.
// </copyright>

namespace Holecy.Console.Dependencies.IO;

using System.IO.Abstractions;

/// <summary>
/// Represents information about a file or directory in the file system.
/// </summary>
internal class FileOrDirectoryInfo : IFileOrDirectoryInfo
{
    /// <summary>
    /// Initializes a new instance of the <see cref="FileOrDirectoryInfo"/> class with the specified path.
    /// </summary>
    /// <param name="path">The path to the file or directory.</param>
    /// <exception cref="ArgumentException">
    /// Thrown when the specified path does not exist as either a file or a directory.
    /// </exception>
    public FileOrDirectoryInfo(string path)
    : this(path, new FileSystem())
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="FileOrDirectoryInfo"/> class with the specified path and file system.
    /// </summary>
    /// <param name="path">The path to the file or directory.</param>
    /// <param name="fileSystem">The file system abstraction to use for checking the path.</param>
    /// <exception cref="ArgumentException">
    /// Thrown when the specified path does not exist as either a file or a directory.
    /// </exception>
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

    /// <summary>
    /// Gets the file system information object, which can represent either a file or a directory.
    /// </summary>
    public IFileSystemInfo FileSystemInfo { get; init; }

    /// <summary>
    /// Gets a value indicating whether the <see cref="FileSystemInfo"/> represents a directory.
    /// </summary>
    public bool IsDirectory { get; init; } = false;
}
