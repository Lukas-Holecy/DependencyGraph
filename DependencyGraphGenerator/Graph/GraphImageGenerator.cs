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
        // Step 1: Create a new graph
        // Msagl.Graph graph = new Msagl.Graph("sampleGraph");
        // graph.AddEdge("Node1", "Node2");
        // graph.AddEdge("Node2", "Node3");
        // graph.AddEdge("Node3", "Node1");

        // // Customize nodes if desired
        // Msagl.Node node1 = graph.FindNode("Node1");
        // node1.Attr.Shape = Msagl.Shape.Circle;
        // node1.Attr.FillColor = Msagl.Color.LightBlue;

        // foreach(Msagl.Node node in graph.Nodes)
        // {
        //     node.LabelText = "Node";
        //     node.Attr.Shape = Msagl.Shape.Circle;
        //     node.Attr.FillColor = Msagl.Color.LightBlue;
        // }
        foreach (var node in this.MsaglGraph.Nodes)
        {
            if (node != null)
            {
                node.Attr.Shape = Msagl.Shape.Circle;
                node.Attr.FillColor = Msagl.Color.LightBlue;
                node.LabelText = "nodeId"; // Set label text to ensure node has content
            }
        }

        this.MsaglGraph.CreateGeometryGraph();

        var layoutSettings = new SugiyamaLayoutSettings();  // Layered layout suitable for directed graphs
        var graphLayout = new Microsoft.Msagl.Core.Layout.GeometryGraph();
        var cancelToken = new CancelToken();
        LayoutHelpers.CalculateLayout(this.MsaglGraph.GeometryGraph, layoutSettings, cancelToken);
        using var svgStream = new MemoryStream();
        var svgWriter = new Msagl.SvgGraphWriter(svgStream, this.MsaglGraph);

        svgWriter.Write();
        svgStream.Position = 0;
        this.svgBytes = svgStream.ToArray();

        // // Step 1: Create a new graph
        // Msagl.Graph graph = new Msagl.Graph("sampleGraph");
        // graph.AddEdge("Node1", "Node2");
        // graph.AddEdge("Node2", "Node3");
        // graph.AddEdge("Node3", "Node1");

        // // Customize nodes if desired
        // Msagl.Node node1 = graph.FindNode("Node1");
        // node1.Attr.Shape = Msagl.Shape.Circle;
        // node1.Attr.FillColor = Msagl.Color.LightBlue;

        // // Step 2: Write the graph as SVG to a file
        // using (FileStream fileStream = new FileStream("outputGraph.svg", FileMode.Create, FileAccess.Write))
        // {
        //     var svgWriter = new Msagl.SvgGraphWriter(fileStream, graph);
        //     svgWriter.Write();  // Converts and writes to the file stream
        // }

        // //Console.WriteLine("SVG file has been saved as outputGraph.svg");
        return [];//this.svgBytes;
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
