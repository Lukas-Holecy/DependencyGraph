// <copyright file="DependencyCommand.cs" company="Lukas Holecy">
// Copyright (c) Lukas Holecy. All rights reserved.
// </copyright>

namespace Holecy.Console.Dependencies.Commands;

using System.IO.Abstractions;
using System.Threading.Tasks;
using CliFx;
using CliFx.Attributes;
using CliFx.Infrastructure;
using Holecy.Console.Dependencies.Graph;
using QuikGraph;

/// <summary>
/// Represents the main command responsible for processing specified file or directory paths and creating
/// dependency graph.
/// </summary>
[Command("show-dependent", Description = "Shows projects that are transitionally dependent of the specified project.")]
internal class DependencyCommand(IFileSystem fileSystem) : ICommand
{
    /// <summary>
    /// The file system abstraction used for file and directory operations.
    /// </summary>
    private readonly IFileSystem fileSystem = fileSystem;

    /// <summary>
    /// Gets positional parameter for paths (files or directories).
    /// </summary>
    [CommandParameter(0, Name = "graphml-file-path", Description = "Dot representation of a graph.", IsRequired = true)]
    public string GraphMLPath { get; init; } = string.Empty;

    /// <summary>
    /// Gets positional parameter for paths (files or directories).
    /// </summary>
    [CommandParameter(1, Name = "project-ids", Description = "Full path to the project or it's package id.", IsRequired = true)]
    public IReadOnlyList<string> ProjectIds { get; init; } = [];

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

    /// <inheritdoc/>
    public async ValueTask ExecuteAsync(IConsole console)
    {
        var graph = this.LoadGraph(this.GraphMLPath);
        var subgraph = graph.GetDependent(this.ProjectIds);
        await console.Output.WriteLineAsync(subgraph.ToLineSeparatedNodeString());
    }

    private AdjacencyGraph<Node, Edge> LoadGraph(string graphMLPath)
    {
        ArgumentNullException.ThrowIfNull(graphMLPath);
        ArgumentException.ThrowIfNullOrWhiteSpace(graphMLPath);
        if (!this.fileSystem.File.Exists(graphMLPath))
        {
            throw new FileNotFoundException($"File '{graphMLPath}' not found.");
        }

        using var fileStream = this.fileSystem.FileStream.New(graphMLPath, FileMode.Open, FileAccess.Read);
        using var reader = new StreamReader(fileStream);
        return GraphMLHandler.DeserializeGraph(reader);
    }
}
