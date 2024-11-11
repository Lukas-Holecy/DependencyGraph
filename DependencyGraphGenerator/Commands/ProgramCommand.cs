// <copyright file="ProgramCommand.cs" company="Lukas Holecy">
// Copyright (c) Lukas Holecy. All rights reserved.
// </copyright>

namespace Holecy.Console.Dependencies.Commands;

using System.Collections.Generic;
using System.ComponentModel;
using System.IO.Abstractions;
using System.Threading.Tasks;
using CliFx;
using CliFx.Attributes;
using CliFx.Exceptions;
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

    /// <summary>
    /// Gets the filter for the graph. If "Local" is specified, only projects with PackageId and Path will be included.
    /// These are the projects available in the local file system.
    /// </summary>
    /// <seealso cref="GraphFilter"/>
    [CommandOption("filter", 'f', Description = "Filter for the graph.")]
    public GraphFilter Filter { get; init; } = GraphFilter.All;

    /// <summary>
    /// Gets the type of the image file to generate.
    /// </summary>
    /// <seealso cref="ImageType"/>
    [CommandOption("image", 'i', Description = "Format of the image file.")]
    public ImageType ImageType { get; init; } = ImageType.None;

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

        var graphDot = GraphDotGenerator.GenerateGraphDot(graph);

        if (!string.IsNullOrWhiteSpace(this.OutputPath))
        {
            new FileSaver(this.fileSystem).SaveStringToFile(graphDot, this.OutputPath);
            await console.Output.WriteLineAsync($"Graph was written to the file: {this.OutputPath}");
        }

        if (this.ImageType != ImageType.None)
        {
            await this.GenerateImages(graphDot);
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

    private async Task GenerateImages(string graph)
    {
        var imageGenerator = new GraphImageGenerator(graph);
        if (this.ImageType == ImageType.Both || this.ImageType == ImageType.Svg)
        {
            var svgStream = new MemoryStream(await imageGenerator.GenerateGraphSvg());
            new FileSaver(this.fileSystem).SaveStreamToFile(svgStream, this.ImagePath, "svg");
        }

        if (this.ImageType == ImageType.Both || this.ImageType == ImageType.Png)
        {
            var pngStream = new MemoryStream(await imageGenerator.GenerateGraphPng());
            new FileSaver(this.fileSystem).SaveStreamToFile(pngStream, this.ImagePath, "png");
        }
    }
}
