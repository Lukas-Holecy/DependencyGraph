// <copyright file="GraphImageGenerator.cs" company="Lukas Holecy">
// Copyright (c) Lukas Holecy. All rights reserved.
// </copyright>

namespace Holecy.Console.Dependencies.Graph;

using System.Diagnostics.CodeAnalysis;
using Holecy.Console.Dependencies.Graph;
using ImageMagick;
using QuikGraph;
using QuikGraph.MSAGL;
using Microsoft.Msagl.Core.Layout;
using Msagl = Microsoft.Msagl.Drawing;

using Microsoft.Msagl.Layout.Layered;
using Microsoft.Msagl.Miscellaneous;
using Microsoft.Msagl.Core;
using System.Reflection;
using Jint;
using CliFx.Extensibility;

/// <summary>
/// Generates image from the graph.
/// </summary>
internal class GraphImageGenerator
{
    private byte[] svgBytes = [];

    /// <summary>
    /// Initializes a new instance of the <see cref="GraphImageGenerator"/> class.
    /// </summary>
    /// <param name="graph">Dependency graph from which the image will be created.</param>
    [SetsRequiredMembers]
    public GraphImageGenerator(AdjacencyGraph<Node, Edge> graph)
    {
        this.MsaglGraph = GetMsaglGraph(graph);
    }

    /// <summary>
    /// Gets the MSAGL representation of the graph given in the constructor.
    /// </summary>
    public required Msagl.Graph MsaglGraph { get; init; }

    /// <summary>
    /// Creates an SVG representation of the graph in byte array.
    /// </summary>
    /// <returns>Byte array with an svg image of the graph.</returns>
    internal byte[] GenerateGraphSvg()
    {
        var renderer = new GraphRenderer();
        var svg = renderer.RenderGraph(this.MsaglGraph.ToString());

        return [];
    }

    /// <summary>
    /// Creates a PNG representation of the graph in byte array.
    /// </summary>
    /// <returns>Byte array with an png image of the graph.</returns>
    internal byte[] GenerateGraphPng()
    {
        // Need to create bitmap from svg, so svg will be moved to variable.
        using var svgStream = new MemoryStream(this.svgBytes);
        svgStream.Position = 0;
        using var magickImage = new MagickImage(svgStream, MagickFormat.Svg);
        using var pngStream = new MemoryStream();
        magickImage.Write(pngStream, MagickFormat.Png);
        pngStream.Position = 0;

        return pngStream.ToArray();
    }

    private static Msagl.Graph GetMsaglGraph(AdjacencyGraph<Node, Edge> graph)
    {
        return graph.ToMsaglGraph();
    }
}
