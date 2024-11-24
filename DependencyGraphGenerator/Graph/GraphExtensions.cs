// <copyright file="GraphExtensions.cs" company="Lukas Holecy">
// Copyright (c) Lukas Holecy. All rights reserved.
// </copyright>

namespace Holecy.Console.Dependencies.Graph;

using System.Text;
using QuikGraph;

/// <summary>
/// Extension method for converting a graph to a list of nodes.
/// </summary>
internal static class GraphExtensions
{
    /// <summary>
    /// Converts a graph to a list of nodes.
    /// </summary>
    /// <param name="graph">DependencyGraph.</param>
    /// <returns>String where each line is an individual node.</returns>
    public static string ToLineSeparatedNodeString(this AdjacencyGraph<Node, Edge> graph)
    {
        var sb = new StringBuilder();

        foreach (var vertex in graph.Vertices)
        {
            sb.AppendLine(vertex.ToString());
        }

        return sb.ToString();
    }

    /// <summary>
    /// Reverses the graph.
    /// </summary>
    /// <param name="originalGraph">Graph in which all edges will be reversed.</param>
    /// <returns>New graph where all edges are reversed.</returns>
    public static AdjacencyGraph<Node, Edge> Reverse(this AdjacencyGraph<Node, Edge> originalGraph)
    {
        // Create a reverse graph
        var reverseGraph = new AdjacencyGraph<Node, Edge>();
        foreach (var vertex in originalGraph.Vertices)
        {
            reverseGraph.AddVertex(vertex);
        }

        foreach (var edge in originalGraph.Edges)
        {
            reverseGraph.AddEdge(new Edge(edge.Target, edge.Source));
        }

        return reverseGraph;
    }
}
