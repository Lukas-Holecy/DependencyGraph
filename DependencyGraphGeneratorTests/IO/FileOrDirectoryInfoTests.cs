// <copyright file="FileOrDirectoryInfoTests.cs" company="Lukas Holecy">
// Copyright (c) Lukas Holecy. All rights reserved.
// </copyright>

namespace Holecy.Console.Dependencies.Tests.IO;

using System.IO.Abstractions;
using Holecy.Console.Dependencies.IO;
using Moq;
using Xunit;

/// <summary>
/// Contains unit tests for the <see cref="FileOrDirectoryInfo"/> class, verifying its behavior
/// when initialized with file and directory paths, as well as handling invalid paths.
/// </summary>
public class FileOrDirectoryInfoTests
{
    /// <summary>
    /// Tests that the constructor initializes as a file when the provided path points to an existing file.
    /// </summary>
    [Fact]
    public void Constructor_ShouldInitializeAsFile_WhenPathIsFile()
    {
        // Arrange
        const string path = "test.txt";
        var mockFileSystem = new Mock<IFileSystem>();
        var mockFileInfo = new Mock<IFileInfo>();
        mockFileSystem.Setup(fs => fs.File.Exists(path)).Returns(true);
        mockFileSystem.Setup(fs => fs.FileInfo.New(path)).Returns(mockFileInfo.Object);

        // Act
        var fileOrDirectoryInfo = new FileOrDirectoryInfo(path, mockFileSystem.Object);

        // Assert
        Assert.NotNull(fileOrDirectoryInfo.FileSystemInfo);
        Assert.False(fileOrDirectoryInfo.IsDirectory);
    }

    /// <summary>
    /// Tests that the constructor initializes as a directory when the provided path points to an existing directory.
    /// </summary>
    [Fact]
    public void Constructor_ShouldInitializeAsDirectory_WhenPathIsDirectory()
    {
        // Arrange
        var path = "testDir";
        var mockFileSystem = new Mock<IFileSystem>();
        var mockDirectoryInfo = new Mock<IDirectoryInfo>();
        mockFileSystem.Setup(fs => fs.File.Exists(path)).Returns(false);
        mockFileSystem.Setup(fs => fs.Directory.Exists(path)).Returns(true);
        mockFileSystem.Setup(fs => fs.DirectoryInfo.New(path)).Returns(mockDirectoryInfo.Object);

        // Act
        var fileOrDirectoryInfo = new FileOrDirectoryInfo(path, mockFileSystem.Object);

        // Assert
        Assert.NotNull(fileOrDirectoryInfo.FileSystemInfo);
        Assert.True(fileOrDirectoryInfo.IsDirectory);
    }

    /// <summary>
    /// Tests that the constructor throws an <see cref="ArgumentException"/> when the provided path is invalid
    /// (null, empty, whitespace, or nonexistent).
    /// </summary>
    /// <param name="path">The path to test.</param>
    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData("nonexistent")]
    public void Constructor_ShouldThrowArgumentNullException_WhenPathIsNotValid(string? path)
    {
        // Arrange
        var mockFileSystem = new Mock<IFileSystem>();
        mockFileSystem.Setup(fs => fs.File.Exists(path)).Returns(false);
        mockFileSystem.Setup(fs => fs.Directory.Exists(path)).Returns(false);

        // Assert
        Assert.Throws<ArgumentException>(() => new FileOrDirectoryInfo(path!, mockFileSystem.Object));
    }
}
