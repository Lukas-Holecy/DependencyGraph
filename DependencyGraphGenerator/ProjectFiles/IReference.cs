// <copyright file="IReference.cs" company="Lukas Holecy">
// Copyright (c) Lukas Holecy. All rights reserved.
// </copyright>

namespace Holecy.Console.Dependencies.ProjectFiles;

internal interface IReference
{
    string Name { get; }

    string Version { get; }
}
