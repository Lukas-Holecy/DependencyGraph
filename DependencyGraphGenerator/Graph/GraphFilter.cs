// <copyright file="GraphFilter.cs" company="Lukas Holecy">
// Copyright (c) Lukas Holecy. All rights reserved.
// </copyright>

namespace Holecy.Console.Dependencies.Graph;

using System;
using System.Collections.Generic;
using System.Linq;
using QuikGraph;
using QuikGraph.Algorithms.Search;

/// <summary>
/// Class for filtering a graph based on a set of nodes.
/// </summary>
internal static class GraphFilter
{
    /// <summary>
    /// Returns a subgraph of the original graph containing only the nodes and edges that are dependent
    /// on the filter nodes.
    /// </summary>
    /// <param name="originalGraph">Full dependency graph.</param>
    /// <param name="filterProjects">
    /// Project names or package ids of project from which all dependent projects are searched.
    /// </param>
    /// <returns>Graph with only nodes that depend on the <paramref name="filterProjects"/>.</returns>
    public static AdjacencyGraph<Node, Edge> GetDependent(
        this AdjacencyGraph<Node, Edge> originalGraph, IEnumerable<string> filterProjects)
    {
        var nodes = originalGraph.Vertices.Where(
            node => filterProjects.Contains(node.PackageId) || filterProjects.Contains(node.Path));

        return GetDependent(originalGraph, nodes);
    }

    /// <summary>
    /// Returns a subgraph of the original graph containing only the nodes and edges that are dependent
    /// on the filter nodes.
    /// </summary>
    /// <param name="originalGraph">Full dependency graph.</param>
    /// <param name="filterNode">Project node from which all dependent projects are searched.</param>
    /// <returns>Graph with only nodes that depend on the <paramref name="filterNode"/>.</returns>
    public static AdjacencyGraph<Node, Edge> GetDependent(
        this AdjacencyGraph<Node, Edge> originalGraph, Node filterNode)
    {
        return GetDependent(originalGraph, [filterNode]);
    }

    /// <summary>
    /// Returns a subgraph of the original graph containing only the nodes and edges that are dependent
    /// on the filter nodes.
    /// </summary>
    /// <param name="originalGraph">Full dependency graph.</param>
    /// <param name="filterNodes">Project nodes from which all dependent projects are searched.</param>
    /// <returns>Graph with only nodes that depend on the <paramref name="filterNodes"/>.</returns>
    public static AdjacencyGraph<Node, Edge> GetDependent(
        this AdjacencyGraph<Node, Edge> originalGraph, IEnumerable<Node> filterNodes)
    {
        var existingNodes = filterNodes.Where(originalGraph.ContainsVertex).Distinct();
        var reversedGraph = originalGraph.Reverse();

        var depthSearch = new DepthFirstSearchAlgorithm<Node, Edge>(reversedGraph);
        var visitedNodes = new HashSet<Node>();
        depthSearch.DiscoverVertex += node => visitedNodes.Add(node);
        var visitedEdges = new HashSet<Edge>();
        depthSearch.ExamineEdge += edge => visitedEdges.Add(edge);

        foreach (var node in existingNodes)
        {
            depthSearch.SetRootVertex(node);
            depthSearch.Compute();
        }

        return GenerateSubgraph(visitedNodes, visitedEdges);
    }

    private static AdjacencyGraph<Node, Edge> GenerateSubgraph(HashSet<Node> visitedNodes, HashSet<Edge> visitedEdges)
    {
        var subgraph = new AdjacencyGraph<Node, Edge>();

        foreach (var node in visitedNodes.Distinct())
        {
            subgraph.AddVertex(node);
        }

        foreach (var edge in visitedEdges.Distinct())
        {
            if (subgraph.ContainsVertex(edge.Source) && subgraph.ContainsVertex(edge.Target))
            {
                subgraph.AddEdge(edge);
            }
        }

        return subgraph;
    }
}
