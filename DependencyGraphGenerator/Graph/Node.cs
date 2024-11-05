// <copyright file="Node.cs" company="Lukas Holecy">
// Copyright (c) Lukas Holecy. All rights reserved.
// </copyright>

// TODO create tests
namespace Holecy.Console.Dependencies.Graph;

using System;
using Holecy.Console.Dependencies.ProjectFiles;
using Microsoft.CodeAnalysis.CSharp.Syntax;

/// <summary>
/// Represents a node representing a project in a dependency graph.
/// </summary>
internal class Node
{
    /// <summary>
    /// Initializes a new instance of the <see cref="Node"/> class.
    /// </summary>
    /// <param name="projectInfo">Information about the project represented by this node.</param>
    public Node(IProjectInformation projectInfo)
    {
        ArgumentNullException.ThrowIfNull(projectInfo, nameof(projectInfo));

        this.PackageId = projectInfo.PackageId;
        this.Path = projectInfo.Path;
    }

    public Node(IReference reference)
    {
        var result = reference switch
        {
            PackageReference packageReference => this.PackageId = packageReference.Name,
            ProjectReference projectReference => this.Path = projectReference.Name,
            AssemblyReference assemblyReference => this.Path = assemblyReference.Name,
            _ => throw new ArgumentException("Unknown reference type.", nameof(reference)),
        };
    }

    private Node(string packageId, string path)
    {
        this.PackageId = packageId ?? string.Empty;
        this.Path = path ?? string.Empty;
    }

    /// <summary>
    /// Gets the package Id of the node representing the projects package ID or package reference package name.
    /// </summary>
    /// <remarks>
    /// Each node can be represented either by a package name or by full path of the project file.
    /// So it has two ids. Just one of them is enough for a node to match.
    /// Package reference will have only this, project reference can have both and traditional assembly
    /// reference will have only the path.
    /// </remarks>
    public string PackageId { get; init; } = string.Empty;

    /// <summary>
    /// Gets the path of the node representing the projects full path.
    /// </summary>
    /// <remarks>
    /// Each node can be represented either by a package name or by full path of the project file.
    /// So it has two ids. Just one of them is enough for a node to match.
    /// Package reference will have only this, project reference can have both and traditional assembly
    /// reference will have only the path.
    /// </remarks>
    public string Path { get; init; } = string.Empty;

    /// <inheritdoc/>
    public override bool Equals(object? obj)
    {
        return obj is Node other && this.PackageId == other.PackageId && this.Path == other.Path;
    }

    /// <inheritdoc/>
    public override string ToString()
    {
        return string.IsNullOrEmpty(this.PackageId) ? this.Path : $"{this.PackageId}, {this.Path}";
    }

    public Node Combine(Node other)
    {
        if (this.PackageId != other.PackageId)
        {
            return this;
        }

        return new Node(
            string.IsNullOrEmpty(this.PackageId) ? this.PackageId : other.PackageId,
            string.IsNullOrEmpty(this.Path) ? this.Path : other.Path);
    }

    public override int GetHashCode()
    {
        // If PackageId is not empty, use its hash code.
        // If PackageId is empty, use Path's hash code.
        // If both are empty, return a default hash code (0).
        return !string.IsNullOrEmpty(this.PackageId) ? this.PackageId.GetHashCode() :
            !string.IsNullOrEmpty(this.Path) ? this.Path.GetHashCode() : 0;
    }
}
