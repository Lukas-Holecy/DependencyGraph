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
using Holecy.Console.Dependencies.Graph;
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
    public IReadOnlyList<FileOrDirectoryInfo> Paths { get; init; } = [];

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

    [CommandOption("filter", 'f', Description = "Filter for the graph.")]
    public GraphFilter Filter { get; init; } = GraphFilter.All;

    /// <inheritdoc/>
    public async ValueTask ExecuteAsync(IConsole console)
    {
        var projects = new ProjectFinder(this.fileSystem).FindProjects(this.Paths);
        var projectInformation = ExtractAllProjectsInformation(projects);
        var graph = new GraphFactory().CreateGraph(projectInformation, this.Filter);

        await console.Output.WriteLineAsync($"Information about individual projects:");
        var projectInformationStrings = string.Join(Environment.NewLine, projectInformation.Select(p => p.ToString()));
        await console.Output.WriteLineAsync(projectInformationStrings ?? string.Empty);

        var nodesStrings = string.Join(Environment.NewLine, graph.Vertices.Select(n => n.ToString()));
        await console.Output.WriteLineAsync($"Nodes in the graph:");
        await console.Output.WriteLineAsync(nodesStrings ?? string.Empty);

        if (!string.IsNullOrWhiteSpace(this.OutputPath))
        {
            var graphDot = new GraphDotGenerator(this.fileSystem).GenerateGraphDot(graph, this.OutputPath);
            FileSaver.SaveStringToFile(graphDot, this.OutputPath, this.fileSystem);
            await console.Output.WriteLineAsync($"Graph was written to the file: {this.OutputPath}");
        }
    }

    private static HashSet<IProjectInformation> ExtractAllProjectsInformation(IEnumerable<IFileInfo> projects)
    {
        var allProjectsInformation = new HashSet<IProjectInformation>();
        foreach (var project in projects)
        {
            if (ProjectInfoExtractor.TryExtractProjectInformation(project, out var projectInformation))
            {
                allProjectsInformation.Add(projectInformation!);
            }
        }

        return allProjectsInformation;
    }
}
