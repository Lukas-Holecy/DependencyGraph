// <copyright file="GraphFactory.cs" company="Lukas Holecy">
// Copyright (c) Lukas Holecy. All rights reserved.
// </copyright>

namespace Holecy.Console.Dependencies.Graph;

using System;
using System.Text.RegularExpressions;
using Holecy.Console.Dependencies.ProjectFiles;
using QuikGraph;

/// <summary>
/// Factory for creating Dependency graph.
/// </summary>
internal class GraphFactory
{
    private static readonly Regex FolderPathRegex = new(@"^[a-zA-Z]:\\(?:[^<>:""/\\|?*]+\\)*$", RegexOptions.Compiled);

    /// <summary>
    /// Creates a graph from the provided projects information.
    /// </summary>
    /// <param name="projectsInformation">information about discovered .net projects.</param>
    /// <param name="filter">Filter for the graph.</param>
    /// <returns>New graph objects with directional edges between projects and their dependencies.</returns>
    public AdjacencyGraph<Node, Edge> CreateGraph(HashSet<IProjectInformation> projectsInformation, GraphFilter filter)
    {
        var nodes = NodeFactory.CreateNodes(projectsInformation);
        if (filter == GraphFilter.PathAndPackage)
        {
            nodes = GetFilteredNodesWithPathAndPackageId(nodes);
        }

        if (filter == GraphFilter.LocalPath)
        {
            nodes = GetFilteredNodesWithLocalId(nodes);
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

    private static HashSet<Node> GetFilteredNodesWithLocalId(HashSet<Node> nodes)
    {
        return nodes
            .Where(n => !string.IsNullOrEmpty(n.Path) && FolderPathRegex.IsMatch(n.Path))
            .ToHashSet();
    }
}
