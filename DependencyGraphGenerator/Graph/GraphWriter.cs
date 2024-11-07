// <copyright file="GraphWriter.cs" company="Lukas Holecy">
// Copyright (c) Lukas Holecy. All rights reserved.
// </copyright>

namespace Holecy.Console.Dependencies.Graph;

using System;
using System.IO.Abstractions;
using System.Threading.Tasks;
using QuikGraph;
using QuikGraph.Algorithms;

internal class GraphWriter
{
    private IFileSystem fileSystem;

    public GraphWriter(IFileSystem fileSystem)
    {
        this.fileSystem = fileSystem;
    }

    internal async Task WriteGraphToFile(AdjacencyGraph<Node, Edge> graph, string outputPath)
    {
        throw new NotImplementedException();
    }
}
