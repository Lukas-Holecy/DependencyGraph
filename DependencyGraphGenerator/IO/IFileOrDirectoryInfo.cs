// <copyright file="IFileOrDirectoryInfo.cs" company="Lukas Holecy">
// Copyright (c) Lukas Holecy. All rights reserved.
// </copyright>

namespace Holecy.Console.Dependencies.IO;

using System.IO.Abstractions;

public interface IFileOrDirectoryInfo
{
    IFileSystemInfo FileSystemInfo { get; }

    bool IsDirectory { get; }
}
