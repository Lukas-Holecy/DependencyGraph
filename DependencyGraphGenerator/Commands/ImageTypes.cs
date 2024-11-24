// <copyright file="ImageTypes.cs" company="Lukas Holecy">
// Copyright (c) Lukas Holecy. All rights reserved.
// </copyright>

namespace Holecy.Console.Dependencies.Commands;

/// <summary>
/// Type of image to be generated.
/// </summary>
[Flags]
internal enum ImageTypes
{
    /// <summary>
    /// No image to be generated.
    /// </summary>
    None = 0,

    /// <summary>
    /// Graph in .dot format.
    /// </summary>
    Svg = 1 << 0,

    /// <summary>
    /// Graph in .png format.
    /// </summary>
    Png = 1 << 1,

    /// <summary>
    /// Both .dot and .png formats.
    /// </summary>
    All = Svg | Png,
}
