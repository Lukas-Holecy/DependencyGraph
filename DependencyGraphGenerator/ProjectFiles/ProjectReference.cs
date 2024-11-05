// <copyright file="ProjectReference.cs" company="Lukas Holecy">
// Copyright (c) Lukas Holecy. All rights reserved.
// </copyright>

namespace Holecy.Console.Dependencies.ProjectFiles;

/// <summary>
/// Represents a project reference in a .net project.
/// </summary>
internal record class ProjectReference : IReference
{
    /// <summary>
    /// Gets the full path to the referenced project file.
    /// </summary>
    public required string Name { get; init; }

    /// <inheritdoc/>
    public override string ToString()
    {
        return this.Name;
    }
}
