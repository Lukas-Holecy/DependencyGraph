// <copyright file="IFileOrDirectoryInfo.cs" company="Lukas Holecy">
// Copyright (c) Lukas Holecy. All rights reserved.
// </copyright>

namespace Holecy.Console.Dependencies.IO;

using System.IO.Abstractions;

/// <summary>
/// Represents information about a file or directory in the file system.
/// </summary>
public interface IFileOrDirectoryInfo
{
    /// <summary>
    /// Gets the file system information object, which can represent either a file or a directory.
    /// </summary>
    IFileSystemInfo FileSystemInfo { get; }

    /// <summary>
    /// Gets a value indicating whether the <see cref="FileSystemInfo"/> represents a directory.
    /// </summary>
    bool IsDirectory { get; }
}
