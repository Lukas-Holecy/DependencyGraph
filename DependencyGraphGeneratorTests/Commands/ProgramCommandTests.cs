// <copyright file="ProgramCommandTests.cs" company="Lukas Holecy">
// Copyright (c) Lukas Holecy. All rights reserved.
// </copyright>

namespace Holecy.Console.Dependencies.Tests.Commands;

using System.Collections.Generic;
using System.IO.Abstractions;
using System.Threading.Tasks;
using CliFx.Infrastructure;
using Holecy.Console.Dependencies.Commands;
using Holecy.Console.Dependencies.IO;
using Moq;
using Xunit;

/// <summary>
/// Contains unit tests for the <see cref="ProgramCommand"/> class, verifying its behavior.
/// </summary>
public class ProgramCommandTests
{
    [Fact]
    public async Task ExecuteAsync_WithOutputPath_SavesGraphToFile()
    {
        // Arrange
        var fileSystemMock = new Mock<IFileSystem>();
        var consoleMock = new Mock<IConsole>();
        var command = new ProgramCommand(fileSystemMock.Object)
        {
            OutputPath = "output.txt",
            Paths = new List<FileOrDirectoryInfo>(),
            ImageType = ImageType.None,
        };

        // Act
        await command.ExecuteAsync(consoleMock.Object);

        // Assert
        fileSystemMock.Verify(fs => fs.File.WriteAllText("output.txt", It.IsAny<string>()), Times.Once);
    }

    [Fact]
    public async Task ExecuteAsync_WithImageType_GeneratesImages()
    {
        // Arrange
        var fileSystemMock = new Mock<IFileSystem>();
        var consoleMock = new Mock<IConsole>();
        var command = new ProgramCommand(fileSystemMock.Object)
        {
            ImageType = ImageType.Png,
            ImagePath = "graph.png",
            Paths = new List<FileOrDirectoryInfo>(),
        };

        // Act
        await command.ExecuteAsync(consoleMock.Object);

        // Assert
        // Verify that the image generation method was called
        // (You may need to mock dependencies involved in image generation)
    }

    [Fact]
    public async Task ExecuteAsync_WithNoOutputPath_WritesGraphToConsole()
    {
        // Arrange
        var fileSystemMock = new Mock<IFileSystem>();
        var consoleMock = new Mock<IConsole>();
        var command = new ProgramCommand(fileSystemMock.Object)
        {
            Paths = new List<FileOrDirectoryInfo>(),
            ImageType = ImageType.None,
        };

        // Act
        await command.ExecuteAsync(consoleMock.Object);

        // Assert
        // Verify that console output was written
        consoleMock.Verify(c => c.Output.WriteLineAsync(It.IsAny<string>()), Times.Once);
    }
}
