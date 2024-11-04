// <copyright file="IProjectInformation.cs" company="Lukas Holecy">
// Copyright (c) Lukas Holecy. All rights reserved.
// </copyright>

namespace Holecy.Console.Dependencies.ProjectFiles;

using System.ComponentModel.Design;
using System.IO.Abstractions;

/// <summary>
/// Information about a .net project.
/// </summary>
internal interface IProjectInformation
{
    /// <summary>
    /// Gets the name of the project.
    /// </summary>
    string Name { get; init; }

    /// <summary>
    /// Gets the full path to the project file.
    /// </summary>
    string Path { get; init; }

    /// <summary>
    /// Gets the package id of the project.
    /// </summary>
    string PackageId { get; init; }

    /// <summary>
    /// Gets the references in the project.
    /// </summary>
    HashSet<IReference> References { get; init; }
}
