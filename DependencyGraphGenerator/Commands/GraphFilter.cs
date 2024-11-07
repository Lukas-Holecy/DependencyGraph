// <copyright file="GraphFilter.cs" company="Lukas Holecy">
// Copyright (c) Lukas Holecy. All rights reserved.
// </copyright>

namespace Holecy.Console.Dependencies;

internal enum GraphFilter
{
    /// <summary>
    /// Represents a filter that includes all dependencies.
    /// </summary>
    All,

    /// <summary>
    /// Represents a filter that includes only to other local projects.
    /// </summary>
    Local,
}
