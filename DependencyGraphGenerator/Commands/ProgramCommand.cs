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

[Command]
internal class ProgramCommand : ICommand
{
    private readonly IFileSystem fileSystem;

    public ProgramCommand(IFileSystem fileSystem)
    {
        this.fileSystem = fileSystem;
    }

    /// <summary>
    /// Gets positional parameter for paths (files or directories).
    /// </summary>
    [CommandParameter(0, Name = "paths", Description = "Paths to directories or files to process.", IsRequired = true)]
    public IReadOnlyList<IFileOrDirectoryInfo> Paths { get; init; } =[];

    [CommandOption("log", 'l', Description = "Enables logging.")]
    public bool EnableLogging { get; init; }

    [CommandOption("log-path", Description = "Path to the log file.")]
    public string LogPath { get; init; } = string.Empty;

    // -o or --output option to specify the output file
    [CommandOption("output", 'o', Description = "Path to the output file.")]
    public string OutputPath { get; init; } = string.Empty;

    /// <inheritdoc/>
    public async ValueTask ExecuteAsync(IConsole console)
    {
        if (this.Paths == null || this.Paths.Count == 0)
        {
            throw new CommandException("At least one path must be specified.");
        }

        await console.Output.WriteLineAsync($"Processing {this.Paths.Count} paths");

        // // Handle logging
        // if (EnableLogging)
        // {
        //     var logFilePath = LogPath ?? "default.log";
        //     await console.Output.WriteLineAsync($"Logging enabled. Log file: {logFilePath}");
        //     // Initialize logging to the specified log file
        //     // For example, set up a logger here
        // }

        // // Process each path
        // foreach (var path in Paths)
        // {
        //     if (File.Exists(path))
        //     {
        //         await console.Output.WriteLineAsync($"Processing file: {path}");
        //         // Add your file processing logic here
        //     }
        //     else if (Directory.Exists(path))
        //     {
        //         await console.Output.WriteLineAsync($"Processing directory: {path}");
        //         // Add your directory processing logic here
        //     }
        //     else
        //     {
        //         await console.Error.WriteLineAsync($"Path not found: {path}");
        //     }
        // }

        // // Handle output
        // if (!string.IsNullOrWhiteSpace(OutputPath))
        // {
        //     await console.Output.WriteLineAsync($"Output will be saved to: {OutputPath}");
        //     // Save output to the specified file
        // }
        // else
        // {
        //     await console.Output.WriteLineAsync("No output file specified.");
        //     // Handle default output behavior, if any
        // }
    }
}
