// <copyright file="GraphImageGenerator.cs" company="Lukas Holecy">
// Copyright (c) Lukas Holecy. All rights reserved.
// </copyright>

namespace Holecy.Console.Dependencies.Commands;

using System;
using System.Diagnostics.CodeAnalysis;
using System.Drawing;
using Holecy.Console.Dependencies.Graph;
using QuikGraph;
using Msagl = Microsoft.Msagl.Drawing;

internal class GraphImageGenerator
{
    [SetsRequiredMembers]
    public GraphImageGenerator(AdjacencyGraph<Node, Edge> graph)
    {
        this.MsaglGraph = this.GetMsaglGraph(graph);
    }

    public required Msagl.Graph MsaglGraph { get; init; }

    internal MemoryStream GenerateGraphSvg()
    {
        var svgStream = new MemoryStream();
        var svgWriter = new Msagl.SvgGraphWriter(svgStream, this.MsaglGraph);
        svgWriter.Write();
        svgStream.Position = 0;
        return svgStream;
    }

    internal MemoryStream GenerateGraphPng()
    {
        // Need to create bitmap from svg, so svg will be moved to variable.

        var pngStream = new MemoryStream();
        bitmap.Save(pngStream, ImageFormat.Png);
        pngStream.Position = 0;
        return pngStream;
    }

    private Msagl.Graph GetMsaglGraph(AdjacencyGraph<Node, Edge> graph)
    {
        return graph.ToMsaglGraph();
    }

}
