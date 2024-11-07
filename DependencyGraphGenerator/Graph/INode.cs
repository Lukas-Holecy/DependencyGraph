// <copyright file="INode.cs" company="Lukas Holecy">
// Copyright (c) Lukas Holecy. All rights reserved.
// </copyright>

namespace Holecy.Console.Dependencies.Graph;

/// <summary>
/// Represents a node in the dependency graph.
/// </summary>
internal interface INode
{
    /// <summary>
    /// Gets the package (NuGet) ID of the node.
    /// </summary>
    string PackageId { get; }

    /// <summary>
    /// Gets the path of the node.
    /// </summary>
    string Path { get; }

    /// <summary>
    /// Determines whether the node represents the same project as the other node.
    /// For example one project might reference a NuGet package, while the other might reference the project itself.
    /// Both point to the same project.
    /// </summary>
    /// <param name="other">Another node object.</param>
    /// <returns>True if either the path or package id is the same.</returns>
    bool IsSameProject(Node other);
}
