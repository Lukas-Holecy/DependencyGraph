// <copyright file="ListProjectsCommand.cs" company="Lukas Holecy">
// Copyright (c) Lukas Holecy. All rights reserved.
// </copyright>

namespace Holecy.Console.Dependencies.Commands;

using System.Collections.Generic;
using System.IO.Abstractions;
using System.Threading.Tasks;
using CliFx;
using CliFx.Attributes;
using CliFx.Exceptions;
using CliFx.Infrastructure;
using Holecy.Console.Dependencies.IO;

[Command("list-projects", Description = "Lists all projects in the specified directories.")]
internal class ListProjectsCommand(IFileSystem fileSystem): ICommand
{
    private readonly IFileSystem fileSystem = fileSystem;

    /// <summary>
    /// Gets positional parameter for paths (files or directories).
    /// </summary>
    [CommandParameter(0, Name = "paths", Description = "Paths to directories or files to process.", IsRequired = true)]
    public IReadOnlyList<IFileOrDirectoryInfo> Paths { get; init; } =[];

    [CommandOption("log", 'l', Description = "Enables logging.")]
    public bool EnableLogging { get; init; }

    [CommandOption("log-path", Description = "Path to the log file.")]
    public string LogPath { get; init; } = string.Empty;

    /// <inheritdoc/>
    public async ValueTask ExecuteAsync(IConsole console)
    {
        var projects = new ProjectFinder(this.fileSystem).FindProjects(this.Paths);
        await console.Output.WriteLineAsync($"Found {projects.Count()} projects:");
        await console.Output.WriteLineAsync(string.Join(Environment.NewLine, projects.Select(p => p.FullName)));
    }
}
