// <copyright file="NodeFactory.cs" company="Lukas Holecy">
// Copyright (c) Lukas Holecy. All rights reserved.
// </copyright>

namespace Holecy.Console.Dependencies.Graph;

using System;
using System.Collections.Generic;
using System.Linq;
using Holecy.Console.Dependencies.ProjectFiles;

/// <summary>
/// Factory for creating nodes.
/// </summary>
internal static class NodeFactory
{
    /// <summary>
    /// Generates nodes from the provided projects information.
    /// </summary>
    /// <param name="projectsInformation">Collection of information about project files.</param>
    /// <returns>Collection of nodes to be used in Dependency Graph.</returns>
    public static HashSet<Node> CreateNodes(HashSet<IProjectInformation> projectsInformation)
    {
        var nodes = GetLocalProjectNodes(projectsInformation);
        AddReferencesNodes(projectsInformation, nodes);
        return nodes;
    }

    private static void AddReferencesNodes(HashSet<IProjectInformation> projectsInformation, HashSet<Node> nodes)
    {
        foreach (var project in projectsInformation)
        {
            AddProjectReferences(project, nodes);
        }
    }

    private static void AddProjectReferences(IProjectInformation project, HashSet<Node> nodes)
    {
        foreach (var reference in project.References)
        {
            var node = new Node(reference);
            AddMethod(nodes, node);
        }
    }

    private static HashSet<Node> GetLocalProjectNodes(HashSet<IProjectInformation> projectsInformation)
    {
        var nodes = new HashSet<Node>();
        foreach (var projectInformation in projectsInformation)
        {
            var node = new Node(projectInformation);
            AddMethod(nodes, node);
        }

        return nodes;
    }

    private static void AddMethod(HashSet<Node> nodes, Node node)
    {
        var existingNodes = nodes.Where(n => n.IsSameProject(node));
        if (existingNodes.Any())
        {
            ReplaceExistingNodesIfNecessary(nodes, node, existingNodes);
            return;
        }

        nodes.Add(node);
    }

    /// <summary>
    /// I only need to replace the existing nodes if the new node has both the packageId and path and
    /// if the existing one is missing one of them.
    /// </summary>
    /// <param name="nodes">Collection of nodes.</param>
    /// <param name="newNode">New node that can be added to the collection.</param>
    /// <param name="existingNodes">Collection of nodes already existing in the collection.</param>
    private static void ReplaceExistingNodesIfNecessary(
        HashSet<Node> nodes, Node newNode, IEnumerable<Node> existingNodes)
    {
        if (existingNodes.Count() > 1)
        {
            KeepNodeWithBothIds(nodes, newNode, existingNodes);
        }

        ReplaceExistingNodeIfNecessary(nodes, newNode, existingNodes.First());
    }

    private static void KeepNodeWithBothIds(HashSet<Node> nodes, Node newNode, IEnumerable<Node> existingNodes)
    {
        throw new NotImplementedException();
    }

    private static void ReplaceExistingNodeIfNecessary(HashSet<Node> nodes, Node newNode, Node existingNode)
    {
        if (string.IsNullOrEmpty(newNode.PackageId) || string.IsNullOrEmpty(newNode.Path))
        {
            return;
        }

        if (string.IsNullOrEmpty(existingNode.PackageId) || string.IsNullOrEmpty(existingNode.Path))
        {
            nodes.Remove(existingNode);
            nodes.Add(newNode);
        }
    }
}
