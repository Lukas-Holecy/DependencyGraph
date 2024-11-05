// <copyright file="PackageReference.cs" company="Lukas Holecy">
// Copyright (c) Lukas Holecy. All rights reserved.
// </copyright>

namespace Holecy.Console.Dependencies.ProjectFiles;

/// <summary>
/// Represents a package reference in a .net project.
/// </summary>
internal record class PackageReference : IReference
{
    /// <summary>
    /// Gets the name of the referenced package.
    /// </summary>
    public required string Name { get; init; }

    /// <summary>
    /// Gets the version of the referenced package.
    /// </summary>
    public required string Version { get; init; }

    /// <inheritdoc/>
    public override string ToString()
    {
        return this.Name + " " + this.Version;
    }
}
