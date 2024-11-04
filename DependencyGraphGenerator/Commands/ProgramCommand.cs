// <copyright file="ProgramCommand.cs" company="Lukas Holecy">
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
using Holecy.Console.Dependencies.ProjectFiles;

/// <summary>
/// Represents the main command responsible for processing specified file or directory paths and creating
/// dependency graph.
/// </summary>
[Command]
internal class ProgramCommand(IFileSystem fileSystem) : ICommand
{
    /// <summary>
    /// The file system abstraction used for file and directory operations.
    /// </summary>
    private readonly IFileSystem fileSystem = fileSystem;

    /// <summary>
    /// Gets positional parameter for paths (files or directories).
    /// </summary>
    [CommandParameter(0, Name = "paths", Description = "Paths to directories or files to process.", IsRequired = true)]
    public IReadOnlyList<IFileOrDirectoryInfo> Paths { get; init; } = [];

    /// <summary>
    /// Gets a value indicating whether logging is enabled.
    /// </summary>
    [CommandOption("log", 'l', Description = "Enables logging.")]
    public bool EnableLogging { get; init; }

    /// <summary>
    /// Gets the path to the log file.
    /// </summary>
    [CommandOption("log-path", Description = "Path to the log file.")]
    public string LogPath { get; init; } = string.Empty;

    /// <summary>
    /// Gets the path to the output file.
    /// </summary>
    /// <remarks>
    /// This option can be specified using -o or --output.
    /// </remarks>
    [CommandOption("output", 'o', Description = "Path to the output file.")]
    public string OutputPath { get; init; } = string.Empty;

    /// <inheritdoc/>
    public async ValueTask ExecuteAsync(IConsole console)
    {
        var projects = new ProjectFinder(this.fileSystem).FindProjects(this.Paths);
        var projectInformations = this.ExtractAllProjectsInformation(projects);

        await console.Output.WriteLineAsync($"Information about individial projects:");
        var projectInformationsStrings = string.Join(Environment.NewLine, projectInformations.Select(p => p.ToString()))
        await console.Output.WriteLineAsync(projectInformationsStrings ?? string.Empty);
    }

    private HashSet<IProjectInformation> ExtractAllProjectsInformation(IEnumerable<IFileInfo> projects)
    {
        throw new NotImplementedException();
    }
}
