// <copyright file="GraphImageGenerator.cs" company="Lukas Holecy">
// Copyright (c) Lukas Holecy. All rights reserved.
// </copyright>

namespace Holecy.Console.Dependencies.Graph;

/// <summary>
/// Generates image from the graph.
/// </summary>
/// <remarks>
/// Initializes a new instance of the <see cref="GraphImageGenerator"/> class.
/// </remarks>
/// <param name="graphDot">Dependency graph from which the image will be created in its dot format.</param>
internal class GraphImageGenerator(string graphDot)
{
    private readonly string graphDot = graphDot;

    /// <summary>
    /// Creates an SVG representation of the graph in byte array.
    /// </summary>
    /// <returns>Byte array with an svg image of the graph.</returns>
    internal async Task<byte[]> GenerateGraphSvg()
    {
        await using var svgStream = await GraphVizSvgGenerator.GenerateSVGStreamAsync(this.graphDot);
        if (svgStream.Length > 0)
        {
            var svgBytes = new byte[svgStream.Length];
            using var cancellationTokenSource = new CancellationTokenSource();
            await svgStream.ReadAsync(svgBytes, cancellationTokenSource.Token);
            return svgBytes;
        }

        return [];
    }

    /// <summary>
    /// Creates a PNG representation of the graph in byte array.
    /// </summary>
    /// <returns>Byte array with an png image of the graph.</returns>
    internal async Task<byte[]> GenerateGraphPng()
    {
        await using var pngStream = await GraphVizSvgGenerator.GeneratePNGStreamAsync(this.graphDot);
        if (pngStream.Length > 0)
        {
            var svgBytes = new byte[pngStream.Length];
            using var cancellationTokenSource = new CancellationTokenSource();
            await pngStream.ReadAsync(svgBytes, cancellationTokenSource.Token);
            return svgBytes;
        }

        return [];
    }
}
