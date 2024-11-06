// <copyright file="GraphFactory.cs" company="Lukas Holecy">
// Copyright (c) Lukas Holecy. All rights reserved.
// </copyright>

namespace Holecy.Console.Dependencies.Graph;

using QuikGraph;

internal class GraphFactory
{
    public AdjacencyGraph<Node> CreateGraph(HashSet<Node> nodes)
    {
        var graph = new Graph();
        foreach (var node in nodes)
        {
            graph.AddNode(node);
        }

        return graph;
    }
}
