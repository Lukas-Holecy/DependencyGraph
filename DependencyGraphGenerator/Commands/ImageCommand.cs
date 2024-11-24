// <copyright file="ImageCommand.cs" company="Lukas Holecy">
// Copyright (c) Lukas Holecy. All rights reserved.
// </copyright>

namespace Holecy.Console.Dependencies.Commands;

using System.IO.Abstractions;
using System.Threading.Tasks;
using CliFx;
using CliFx.Attributes;
using CliFx.Infrastructure;
using Holecy.Console.Dependencies.Graph;
using Holecy.Console.Dependencies.IO;

/// <summary>
/// Represents the main command responsible for processing specified file or directory paths and creating
/// dependency graph.
/// </summary>
[Command("create-image", Description = "Creates an image of the specified graph.")]
internal class ImageCommand(IFileSystem fileSystem) : ICommand
{
    /// <summary>
    /// The file system abstraction used for file and directory operations.
    /// </summary>
    private readonly IFileSystem fileSystem = fileSystem;

    /// <summary>
    /// Gets positional parameter for paths (files or directories).
    /// </summary>
    [CommandParameter(0, Name = "dot-graph", Description = "Dot representation of a graph.", IsRequired = true)]
    public string DotGraph { get; init; } = string.Empty;

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
        if (this.ImageType != ImageTypes.None)
        {
            await this.GenerateImages();
        }
    }

    private async Task GenerateImages()
    {
        var imageGenerator = new GraphImageGenerator(this.DotGraph);
        if (this.ImageType.HasFlag(ImageTypes.Svg))
        {
            var svgStream = new MemoryStream(await imageGenerator.GenerateGraphSvg());
            new FileSaver(this.fileSystem).SaveStreamToFile(svgStream, this.ImagePath, "svg");
        }

        if (this.ImageType.HasFlag(ImageTypes.Png))
        {
            var pngStream = new MemoryStream(await imageGenerator.GenerateGraphPng());
            new FileSaver(this.fileSystem).SaveStreamToFile(pngStream, this.ImagePath, "png");
        }
    }
}
