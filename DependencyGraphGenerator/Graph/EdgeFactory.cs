// <copyright file="EdgeFactory.cs" company="Lukas Holecy">
// Copyright (c) Lukas Holecy. All rights reserved.
// </copyright>

namespace Holecy.Console.Dependencies.Graph;

using System.Collections.Generic;
using Holecy.Console.Dependencies.ProjectFiles;

/// <summary>
/// Factory for creating graph edges for Dependency Graph.
/// </summary>
internal static class EdgeFactory
{
    /// <summary>
    /// Creates edges for the graph.
    /// </summary>
    /// <param name="nodes">All nodes generated from <paramref name="projectsInformation"/>.</param>
    /// <param name="projectsInformation">Information about discovered projects.</param>
    /// <returns>Collection of all graph edges between projects and their references.</returns>
    /// <seealso cref="NodeFactory"/>
    public static HashSet<Edge> CreateEdges(HashSet<Node> nodes, HashSet<IProjectInformation> projectsInformation)
    {
        HashSet<Edge> edges = [];
        foreach (var projectInformation in projectsInformation)
        {
            edges.UnionWith(GetProjectEdges(nodes, projectInformation));
        }

        return edges;
    }

    private static IEnumerable<Edge> GetProjectEdges(HashSet<Node> nodes, IProjectInformation projectInformation)
    {
        var projectNode = nodes.FirstOrDefault(n => n.IsSameProject(new Node(projectInformation)));
        if (projectNode == null)
        {
            yield break;
        }

        foreach (var reference in projectInformation.References)
        {
            var referenceNode = nodes.FirstOrDefault(n => n.IsSameProject(new Node(reference)));
            if (referenceNode == null)
            {
                continue;
            }

            yield return new Edge(projectNode, referenceNode);
        }
    }
}
