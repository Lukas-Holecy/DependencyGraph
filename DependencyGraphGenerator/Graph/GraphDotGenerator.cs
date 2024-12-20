// <copyright file="GraphDotGenerator.cs" company="Lukas Holecy">
// Copyright (c) Lukas Holecy. All rights reserved.
// </copyright>

namespace Holecy.Console.Dependencies.Graph;

using QuikGraph;
using QuikGraph.Graphviz;
using QuikGraph.Graphviz.Dot;

/// <summary>
/// Generates dot representation of the graph.
/// </summary>
internal static class GraphDotGenerator
{
    /// <summary>
    /// Generates dot representation of the graph.
    /// </summary>
    /// <param name="graph">Dependency graph.</param>
    /// <returns>String with dot representation of the graph.</returns>
    internal static string GenerateGraphDot(this AdjacencyGraph<Node, Edge> graph)
    {
        return graph.ToGraphviz(algorithm => GetGraphRenderAlgorithm(algorithm));
    }

    private static void GetGraphRenderAlgorithm(GraphvizAlgorithm<Node, Edge> algorithm)
    {
        algorithm.CommonVertexFormat.Shape = GraphvizVertexShape.Circle;
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
