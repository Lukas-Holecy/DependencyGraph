// <copyright file="AssemblyReference.cs" company="Lukas Holecy">
// Copyright (c) Lukas Holecy. All rights reserved.
// </copyright>

namespace Holecy.Console.Dependencies.ProjectFiles;

/// <summary>
/// Represents a traditional assembly reference in a .net project.
/// </summary>
internal record class AssemblyReference : IReference
{
    /// <summary>
    /// Gets tje assembly reference name.
    /// </summary>
    public required string Name { get; init; }

    /// <inheritdoc/>
    public override string ToString()
    {
        return this.Name;
    }
}
