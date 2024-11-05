// <copyright file="Edge.cs" company="Lukas Holecy">
// Copyright (c) Lukas Holecy. All rights reserved.
// </copyright>

namespace Holecy.Console.Dependencies.Graph
{
    using Holecy.Console.Dependencies.ProjectFiles;
    using QuikGraph;

    /// <summary>
    /// Represents an edge in a graph.
    /// </summary>
    internal record class Edge : IEdge<Node>
    {
        public Node Source => throw new NotImplementedException();

        public Node Target => throw new NotImplementedException();
    }
}
