// <copyright file="GraphFilter.cs" company="Lukas Holecy">
// Copyright (c) Lukas Holecy. All rights reserved.
// </copyright>

namespace Holecy.Console.Dependencies.Commands;

/// <summary>
/// Specifies the filter for the graph.
/// </summary>
internal enum GraphFilter
{
    /// <summary>
    /// Represents a filter that includes all dependencies.
    /// </summary>
    All,

    /// <summary>
    /// Represents a filter that includes only the nods with both path and package.
    /// </summary>
    PathAndPackage,

    /// <summary>
    /// Represents a filter that includes only the nods with valid local path.
    /// </summary>
    LocalPath,
}
