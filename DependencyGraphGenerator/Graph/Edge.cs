// <copyright file="Edge.cs" company="Lukas Holecy">
// Copyright (c) Lukas Holecy. All rights reserved.
// </copyright>

namespace Holecy.Console.Dependencies.Graph
{
    using QuikGraph;

    /// <summary>
    /// Represents an edge in a graph.
    /// </summary>
    internal record class Edge : IEdge<Node>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Edge"/> class.
        /// </summary>
        /// <param name="source">Source node (vertex) of the graph edge.</param>
        /// <param name="target">Targe node (vertex) of the graph edge.</param>
        public Edge(Node source, Node target)
        {
            this.Source = source;
            this.Target = target;
        }

        /// <inheritdoc/>
        public Node Source { get; init; }

        /// <inheritdoc/>
        public Node Target { get; init; }
    }
}
