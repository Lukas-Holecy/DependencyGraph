// <copyright file="GraphFactory.cs" company="Lukas Holecy">
// Copyright (c) Lukas Holecy. All rights reserved.
// </copyright>

namespace Holecy.Console.Dependencies.Graph;

using System.IO.Abstractions;
using Holecy.Console.Dependencies.Commands;
using Holecy.Console.Dependencies.ProjectFiles;
using QuikGraph;

/// <summary>
/// Factory for creating Dependency graph.
/// </summary>
internal class GraphFactory(IFileSystem fileSystem)
{
    private readonly IFileSystem fileSystem = fileSystem;

    /// <summary>
    /// Creates a graph from the provided projects information.
    /// </summary>
    /// <param name="projectsInformation">information about discovered .net projects.</param>
    /// <param name="filter">Filter for the graph.</param>
    /// <returns>New graph objects with directional edges between projects and their dependencies.</returns>
    public AdjacencyGraph<Node, Edge> CreateGraph(
        HashSet<IProjectInformation> projectsInformation, NodeFilterType filter)
    {
        var nodes = NodeFactory.CreateNodes(projectsInformation);
        if (filter == NodeFilterType.PathAndPackage)
        {
            nodes = GetFilteredNodesWithPathAndPackageId(nodes);
        }

        if (filter == NodeFilterType.LocalPath)
        {
            nodes = this.GetFilteredNodesWithLocalId(nodes);
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

    private static HashSet<Node> GetFilteredNodesWithPathAndPackageId(HashSet<Node> nodes)
    {
        return nodes.Where(n => !string.IsNullOrEmpty(n.PackageId) && !string.IsNullOrEmpty(n.Path)).ToHashSet();
    }

    private HashSet<Node> GetFilteredNodesWithLocalId(HashSet<Node> nodes)
    {
        return nodes
            .Where(n => this.IsLocalProject(n))
            .ToHashSet();
    }

    private bool IsLocalProject(Node n)
    {
        return !string.IsNullOrEmpty(n.Path) &&
        this.fileSystem.Path.GetExtension(n.Path) == ".csproj" &&
        this.fileSystem.File.Exists(n.Path);
    }
}
