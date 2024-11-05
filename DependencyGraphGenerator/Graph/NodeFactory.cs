// <copyright file="NodeFactory.cs" company="Lukas Holecy">
// Copyright (c) Lukas Holecy. All rights reserved.
// </copyright>

namespace Holecy.Console.Dependencies.Graph;

using System;
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;
using Holecy.Console.Dependencies.ProjectFiles;
using Microsoft.Build.Evaluation;

/// <summary>
/// Factory for creating nodes.
/// </summary>
internal static class NodeFactory
{
    public static HashSet<Node> GenerateNodes(HashSet<IProjectInformation> projectsInformation)
    {
        var nodes = GetLocalProjectNodes(projectsInformation);
        nodes = AddReferencesNodes(projectsInformation, nodes);
        return nodes;
    }

    private static HashSet<Node> AddReferencesNodes(
        HashSet<IProjectInformation> projectsInformation, HashSet<Node> nodes)
    {
        foreach (var project in projectsInformation)
        {
            AddProjectReferences(project, nodes);
        }

        return nodes;
    }

    private static void AddProjectReferences(IProjectInformation project, HashSet<Node> nodes)
    {
        foreach (var reference in project.References)
        {
            var node = new Node(reference);
            AddMethod(nodes, node);
        }
    }

    private static void ReplaceExistingNodeWithCombination(HashSet<Node> nodes, Node node)
    {
        var oldNode = nodes.First(n => n.Equals(node));
        nodes.Remove(oldNode);
        nodes.Add(oldNode.Combine(node));
    }

    private static HashSet<Node> GetLocalProjectNodes(HashSet<IProjectInformation> projectsInformation)
    {
        var nodes = new HashSet<Node>();
        foreach (var projectInformation in projectsInformation)
        {
            var node = new Node(projectInformation);

            // TODO this should be implemented better.
            AddMethod(nodes, node);
        }

        return nodes;
    }

    private static void AddMethod(HashSet<Node> nodes, Node node)
    {
        if (nodes.Contains(node))
        {
            ReplaceExistingNodeWithCombination(nodes, node);
        }
        else
        {
            nodes.Add(node);
        }
    }
}
