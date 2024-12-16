// <copyright file="GraphMLHandler.cs" company="Lukas Holecy">
// Copyright (c) Lukas Holecy. All rights reserved.
// </copyright>

namespace Holecy.Console.Dependencies.Graph;

using System.IO;
using System.Text;
using System.Xml;
using Holecy.Console.Dependencies.Extensions;
using QuikGraph;
using QuikGraph.Algorithms;
using QuikGraph.Serialization;

/// <summary>
/// Extension class for creating GraphML representation of a dependency graph.
/// </summary>
internal static class GraphMLHandler
{
    /// <summary>
    /// Serializes the graph to GraphML string.
    /// </summary>
    /// <param name="graph">Dependency graph. QuikGraph Adjacency graph object.</param>
    /// <returns>String with full graphs GraphML xml representation.</returns>
    public static string SerializeGraphToMLString(this AdjacencyGraph<Node, Edge> graph)
    {
        var stringBuilder = new StringBuilder();
        using var writer = XmlWriter.Create(stringBuilder);
        graph.SerializeToGraphML<Node, Edge, AdjacencyGraph<Node, Edge>>(
            writer,
            node => node.ToString(), // Serialize node as "PackageId:{node.PackageId},Path:{node.Path}"
            _ => graph.GetEdgeIdentity().ToString()); // $"{edge.Source}->{edge.Target}"); // Serialize edge as "Source->Target"
        writer.Flush();
        return stringBuilder.ToString();
    }

    /// <summary>
    /// Deserializes the graph from GraphML file.
    /// </summary>
    /// <param name="graphML">Dependency graph in graphML string.</param>
    /// <returns>Dependency graph.</returns>
    public static AdjacencyGraph<Node, Edge> DeserializeGraph(string graphML)
    {
        using var reader = new StringReader(graphML);
        return DeserializeGraph(reader);
    }

    /// <summary>
    /// Deserializes the graph from GraphML file.
    /// </summary>
    /// <param name="reader">Dependency graph in graphML string reader.</param>
    /// <returns>Dependency graph.</returns>
    public static AdjacencyGraph<Node, Edge> DeserializeGraph(TextReader reader)
    {
        var graph = new AdjacencyGraph<Node, Edge>();
        graph.DeserializeFromGraphML(
            reader,
            id =>
            {
                var parts = id.Split(',');
                return new Node(parts[0].RemovePrefix("PackageId:"), parts[1].RemovePrefix("Path:"));
            },
            (source, target, _) => new Edge(source, target));
        return graph;
    }
}
