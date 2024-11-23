// <copyright file="ImageType.cs" company="Lukas Holecy">
// Copyright (c) Lukas Holecy. All rights reserved.
// </copyright>

namespace Holecy.Console.Dependencies.Commands;

/// <summary>
/// Type of image to be generated.
/// </summary>
internal enum ImageType
{
    /// <summary>
    /// No image to be generated.
    /// </summary>
    None,

    /// <summary>
    /// Graph in .dot format.
    /// </summary>
    Svg,

    /// <summary>
    /// Graph in .png format.
    /// </summary>
    Png,

    /// <summary>
    /// Both .dot and .png formats.
    /// </summary>
    Both,
}
