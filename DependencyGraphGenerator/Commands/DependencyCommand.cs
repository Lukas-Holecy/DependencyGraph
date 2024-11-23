// <copyright file="DependencyCommand.cs" company="Lukas Holecy">
// Copyright (c) Lukas Holecy. All rights reserved.
// </copyright>

namespace Holecy.Console.Dependencies.Commands;

using System.IO.Abstractions;
using System.Threading.Tasks;
using CliFx;
using CliFx.Attributes;
using CliFx.Infrastructure;

/// <summary>
/// Represents the main command responsible for processing specified file or directory paths and creating
/// dependency graph.
/// </summary>
[Command]
internal class DependencyCommand(IFileSystem fileSystem) : ICommand
{
    /// <summary>
    /// Gets the file system abstraction used for file and directory operations.
    /// </summary>
    public IFileSystem FileSystem { get; } = fileSystem;

    /// <inheritdoc/>
    public ValueTask ExecuteAsync(IConsole console)
    {
        throw new NotImplementedException();
    }
}
