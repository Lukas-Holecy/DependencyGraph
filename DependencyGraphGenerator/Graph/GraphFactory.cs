// <copyright file="GraphFactory.cs" company="Lukas Holecy">
// Copyright (c) Lukas Holecy. All rights reserved.
// </copyright>

namespace Holecy.Console.Dependencies.Graph;

using System;
using Holecy.Console.Dependencies.ProjectFiles;
using QuikGraph;

/// <summary>
/// Factory for creating Dependency graph.
/// </summary>
internal class GraphFactory
{
    /// <summary>
    /// Creates a graph from the provided projects information.
    /// </summary>
    /// <param name="projectsInformation">information about discovered .net projects.</param>
    /// <returns>New graph objects with directional edges between projects and their dependencies.</returns>
    public AdjacencyGraph<Node, Edge> CreateGraph(HashSet<IProjectInformation> projectsInformation, GraphFilter filter)
    {
        var nodes = NodeFactory.CreateNodes(projectsInformation);
        if (filter == GraphFilter.Local)
        {
            nodes = this.GetFiltereNodes(nodes);
        }

        var edges = EdgeFactory.CreateEdges(nodes, projectsInformation);
        AdjacencyGraph<Node, Edge> graph = new();
        foreach (var node in nodes)
        {
            graph.AddVertex(node);
        }

        foreach (var edge in edges)
        {
            graph.AddEdge(edge);
        }

        return graph;
    }

    private HashSet<Node> GetFiltereNodes(HashSet<Node> nodes)
    {
        return nodes.Where(n => !string.IsNullOrEmpty(n.PackageId) && !string.IsNullOrEmpty(n.Path)).ToHashSet();
    }
}
