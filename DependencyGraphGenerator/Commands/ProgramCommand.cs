// <copyright file="ProgramCommand.cs" company="Lukas Holecy">
// Copyright (c) Lukas Holecy. All rights reserved.
// </copyright>

namespace Holecy.Console.Dependencies.Commands;

using System.Collections.Generic;
using System.IO.Abstractions;
using System.Threading.Tasks;
using CliFx;
using CliFx.Attributes;
using CliFx.Infrastructure;
using Holecy.Console.Dependencies.Graph;
using Holecy.Console.Dependencies.IO;
using Holecy.Console.Dependencies.ProjectFiles;
using QuikGraph;

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
    private string graphDot = string.Empty;
    private string graphML = string.Empty;
    private string projectsList = string.Empty;

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
    /// Gets the output type (None, GraphML, Dot or Both) for outputting the graph.
    /// </summary>
    [CommandOption("output-type", 'o', Description = "Output type (None, GraphML, Dot or Both).")]
    public OutputTypes OutputType { get; init; } = OutputTypes.None;

    /// <summary>
    /// Gets the path to the output file.
    /// </summary>
    /// <remarks>
    /// This option can be specified using -o or --output.
    /// </remarks>
    [CommandOption("output-path", 'p', Description = "Path to the output file.")]
    public string OutputPath { get; init; } = string.Empty;

    /// <summary>
    /// Gets the filter for the graph. If "Local" is specified, only projects with PackageId and Path will be included.
    /// These are the projects available in the local file system.
    /// </summary>
    /// <seealso cref="NodeFilterType"/>
    [CommandOption("filter", 'f', Description = "Filter for the graph.")]
    public NodeFilterType Filter { get; init; } = NodeFilterType.All;

    /// <summary>
    /// Gets the list of projects to filter the graph by.
    /// </summary>
    [CommandOption("project-filter", Description = "Only show projects that depend on one of the listed projects.")]
    public IReadOnlyList<string> ProjectFilter { get; init; } = [];

    /// <summary>
    /// Gets the type of the image file to generate.
    /// </summary>
    /// <seealso cref="ImageType"/>
    [CommandOption("image", 'i', Description = "Format of the image file.")]
    public ImageTypes ImageType { get; init; } = ImageTypes.None;

    /// <summary>
    /// Gets the path to where to save the image file.
    /// </summary>
    [CommandOption("image-path", Description = "Path where the image file will be saved.")]
    public string ImagePath { get; init; } = string.Empty;

    /// <inheritdoc/>
    public async ValueTask ExecuteAsync(IConsole console)
    {
        var projects = new ProjectFinder(this.fileSystem).FindProjects(this.Paths);
        var projectInformation = ExtractAllProjectsInformation(projects);
        var graph = new GraphFactory(this.fileSystem).CreateGraph(projectInformation, this.Filter);

        if (this.ProjectFilter.Count > 0)
        {
            graph = graph.GetDependent(this.ProjectFilter);
        }

        await this.OutputGraph(console, graph);

        if (this.ImageType != ImageTypes.None && string.IsNullOrEmpty(this.graphDot))
        {
            this.graphDot = graph.GenerateGraphDot();
        }

        var imageCommand = new ImageCommand(this.fileSystem)
        {
            DotGraph = this.graphDot,
            ImageType = this.ImageType,
            ImagePath = this.ImagePath,
            EnableLogging = this.EnableLogging,
            LogPath = this.LogPath,
        };

        await imageCommand.ExecuteAsync(console);
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

    /// <summary>
    /// Outputs the graph to the console or to the file.
    /// </summary>
    /// <param name="console">Console abstraction for text output.</param>
    /// <param name="graph">Dependency graph.</param>
    /// <returns>Task with no return value.</returns>
    private async Task OutputGraph(IConsole console, AdjacencyGraph<Node, Edge> graph)
    {
        if (this.OutputType.HasFlag(OutputTypes.GraphML))
        {
            this.graphML = graph.SerializeGraphToMLString();
            await this.SaveOrShow(console, this.graphML, "graphml");
        }

        if (this.OutputType.HasFlag(OutputTypes.Dot))
        {
            this.graphDot = graph.GenerateGraphDot();
            await this.SaveOrShow(console, this.graphDot, "dot");
        }

        if (this.OutputType.HasFlag(OutputTypes.ListProjects))
        {
            this.projectsList = graph.ToLineSeparatedNodeString();
            await this.SaveOrShow(console, this.projectsList, "list");
        }
    }

    private async Task SaveOrShow(IConsole console, string outputString, string extension)
    {
        if (string.IsNullOrWhiteSpace(this.OutputPath))
        {
            await console.Output.WriteLineAsync(outputString);
            return;
        }

        new FileSaver(this.fileSystem).SaveStringToFile(outputString, this.OutputPath, extension);
        await console.Output.WriteLineAsync($"Graph was written to the file: {this.OutputPath}");
    }
}
