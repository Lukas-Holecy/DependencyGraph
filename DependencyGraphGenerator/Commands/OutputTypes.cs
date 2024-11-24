// <copyright file="OutputTypes.cs" company="Lukas Holecy">
// Copyright (c) Lukas Holecy. All rights reserved.
// </copyright>

namespace Holecy.Console.Dependencies.Commands;

/// <summary>
/// Output type for the graph.
/// </summary>
[Flags]
public enum OutputTypes
{
    /// <summary>
    /// No output type.
    /// </summary>
    None = 0,

    /// <summary>
    /// Outputs the graphs as GraphML file.
    /// </summary>
    GraphML = 1 << 0,

    /// <summary>
    /// Outputs the graphs as Dot file.
    /// </summary>
    Dot = 1 << 1,

    /// <summary>
    /// Output a list of all projects, each on a new line.
    /// </summary>
    ListProjects = 1 << 2,

    /// <summary>
    /// All output types.
    /// </summary>
    All = GraphML | Dot | ListProjects,
}
