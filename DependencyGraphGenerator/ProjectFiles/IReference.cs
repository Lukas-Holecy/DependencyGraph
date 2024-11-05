// <copyright file="IReference.cs" company="Lukas Holecy">
// Copyright (c) Lukas Holecy. All rights reserved.
// </copyright>

namespace Holecy.Console.Dependencies.ProjectFiles;

/// <summary>
/// Represents a reference in a .net project.
/// This could be a project reference or a package reference.
/// </summary>
internal interface IReference
{
    /// <summary>
    /// Gets the name of the reference. This could be the path of the project or name of the package.
    /// </summary>
    string Name { get; }
}
