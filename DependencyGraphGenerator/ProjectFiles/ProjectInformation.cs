// <copyright file="ProjectInformation.cs" company="Lukas Holecy">
// Copyright (c) Lukas Holecy. All rights reserved.
// </copyright>

namespace Holecy.Console.Dependencies.ProjectFiles;

using System.ComponentModel.Design;
using System.IO.Abstractions;
using System.Text;

/// <summary>
/// Information about a .net project.
/// </summary>
internal record class ProjectInformation : IProjectInformation
{
    /// <summary>
    /// Gets the name of the project.
    /// </summary>
    public required string Name { get; init; }

    /// <summary>
    /// Gets the full path to the project file.
    /// </summary>
    public required string Path { get; init; }

    /// <summary>
    /// Gets the package id of the project.
    /// </summary>
    public string PackageId { get; init; } = string.Empty;

    /// <summary>
    /// Gets the references in the project.
    /// </summary>
    public HashSet<IReference> References { get; init; } = new HashSet<IReference>();

    /// <summary>
    /// Returns a string that represents the current object.
    /// </summary>
    /// <returns>
    /// A string that represents the current object, including its name, path, package ID, and references.
    /// </returns>
    public override string ToString()
    {
        var sb = new StringBuilder();
        sb.AppendLine($"Name: {this.Name ?? "No_Name_Found"}");
        sb.AppendLine($"Path: {this.Path}");
        sb.AppendLine($"PackageId: {this.PackageId ?? "No_PackageId_Found"}");
        sb.AppendLine("References:");
        foreach (IReference reference in this.References.Where(r => r != null))
        {
            sb.AppendLine($"{reference}");
        }

        return sb.ToString();
    }
}
