// <copyright file="GraphDotGenerator.cs" company="Lukas Holecy">
// Copyright (c) Lukas Holecy. All rights reserved.
// </copyright>


namespace Holecy.Console.Dependencies.Graph;

using System;
using System.IO.Abstractions;
using System.Threading.Tasks;
using QuikGraph;
using QuikGraph.Algorithms;
using QuikGraph.Graphviz;
using QuikGraph.Graphviz.Dot;

internal class GraphDotGenerator
{
    private IFileSystem fileSystem;

    public GraphDotGenerator(IFileSystem fileSystem)
    {
        this.fileSystem = fileSystem;
    }

    internal string GenerateGraphDot(AdjacencyGraph<Node, Edge> graph, string outputPath)
    {
        var dotGraph = graph.ToGraphviz(algorithm => GetGraphRenderAlgorithm(algorithm));
        return dotGraph;
    }

    private static void GetGraphRenderAlgorithm(GraphvizAlgorithm<Node, Edge> algorithm)
    {
        algorithm.CommonVertexFormat.Shape = GraphvizVertexShape.Circle;
        // algorithm.CommonEdgeFormat.HeadArrow.Shape = GraphvizArrowShape.Tee;
        algorithm.FormatVertex += (sender, args) =>
        {
            args.VertexFormat.Label = GetVertexLabel(args.Vertex);
            if (!string.IsNullOrEmpty(args.Vertex.PackageId) && !string.IsNullOrEmpty(args.Vertex.Path))
            {
                args.VertexFormat.FillColor = GraphvizColor.LightBlue;
            }
        };
    }

    private static string GetVertexLabel(Node vertex)
    {
        if (string.IsNullOrEmpty(vertex.PackageId))
        {
            return vertex.Path;
        }

        if (string.IsNullOrEmpty(vertex.Path))
        {
            return vertex.PackageId;
        }

        return $"{vertex.PackageId}\n{vertex.Path}";

    }
}
