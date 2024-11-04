// <copyright file="ProjectFinderTests.cs" company="Lukas Holecy">
// "This is a .NET 8 project file."
// </copyright>

namespace Holecy.Console.Dependencies.Tests.IO;

using System.Collections.Generic;
using System.IO.Abstractions;
using System.Linq;
using Holecy.Console.Dependencies.IO;
using Moq;
using Xunit;

public class ProjectFinderTests
{
    private readonly Mock<IFileSystem> mockFileSystem;
    private readonly ProjectFinder projectFinder;

    public ProjectFinderTests()
    {
        this.mockFileSystem = new Mock<IFileSystem>();
        this.projectFinder = new ProjectFinder(this.mockFileSystem.Object);
    }

    [Fact]
    public void FindProjects_WithEmptyInput_ReturnsEmpty()
    {
        // Arrange
        var input = new List<IFileOrDirectoryInfo>();

        // Act
        var result = this.projectFinder.FindProjects(input);

        // Assert
        Assert.Empty(result);
    }

    [Fact]
    public void FindProjects_WithOnlyFiles_ReturnsValidCsprojFiles()
    {
        // Arrange
        var mockFile1 = new Mock<IFileOrDirectoryInfo>();
        mockFile1.Setup(f => f.IsDirectory).Returns(false);
        mockFile1.Setup(f => f.FileSystemInfo).Returns(new Mock<IFileInfo>().Object);

        var mockFile2 = new Mock<IFileOrDirectoryInfo>();
        mockFile2.Setup(f => f.IsDirectory).Returns(false);
        var mockFileInfo2 = new Mock<IFileInfo>();
        mockFileInfo2.Setup(f => f.FullName).Returns("Project1.csproj");
        mockFileInfo2.Setup(f => f.Extension).Returns(".csproj");
        mockFile2.Setup(f => f.FileSystemInfo).Returns(mockFileInfo2.Object);

        this.mockFileSystem.Setup(fs => fs.File.Exists("Project1.csproj")).Returns(true);

        var input = new List<IFileOrDirectoryInfo> { mockFile1.Object, mockFile2.Object };

        // Act
        var result = this.projectFinder.FindProjects(input);

        // Assert
        Assert.Single(result);
        Assert.Equal("Project1.csproj", result.First().FullName);
    }

    [Fact]
    public void FindProjects_WithDirectories_ReturnsCsprojFilesInDirectories()
    {
        // Arrange
        var mockDirectory = new Mock<IFileOrDirectoryInfo>();
        mockDirectory.Setup(d => d.IsDirectory).Returns(true);
        var mockDirectoryInfo = new Mock<IDirectoryInfo>();
        mockDirectoryInfo.Setup(d => d.FullName).Returns("/path/to/directory");
        mockDirectory.Setup(d => d.FileSystemInfo).Returns(mockDirectoryInfo.Object);

        var csprojFiles = new List<string>
        {
            "/path/to/directory/Project1.csproj",
            "/path/to/directory/SubDir/Project2.csproj",
        };

        this.mockFileSystem.Setup(fs => fs.Directory.EnumerateFiles("/path/to/directory", "*.csproj", SearchOption.AllDirectories))
            .Returns(csprojFiles);

        this.mockFileSystem.Setup(fs => fs.FileInfo.New(It.IsAny<string>()))
            .Returns<string>(path =>
            {
                var mockFileInfo = new Mock<IFileInfo>();
                mockFileInfo.Setup(f => f.FullName).Returns(path);
                return mockFileInfo.Object;
            });

        var input = new List<IFileOrDirectoryInfo> { mockDirectory.Object };

        // Act
        var result = this.projectFinder.FindProjects(input).ToList();

        // Assert
        Assert.Equal(2, result.Count);
        Assert.Contains(result, f => f.FullName == "/path/to/directory/Project1.csproj");
        Assert.Contains(result, f => f.FullName == "/path/to/directory/SubDir/Project2.csproj");
    }

    [Fact]
    public void FindProjects_MixedFilesAndDirectories_ReturnsAllValidCsprojFiles()
    {
        // Arrange
        // Mock a directory
        var mockDirectory = new Mock<IFileOrDirectoryInfo>();
        mockDirectory.Setup(d => d.IsDirectory).Returns(true);
        var mockDirectoryInfo = new Mock<IDirectoryInfo>();
        mockDirectoryInfo.Setup(d => d.FullName).Returns("/path/to/directory");
        mockDirectory.Setup(d => d.FileSystemInfo).Returns(mockDirectoryInfo.Object);

        // Mock files
        var mockFile1 = new Mock<IFileOrDirectoryInfo>();
        mockFile1.Setup(f => f.IsDirectory).Returns(false);
        var mockFileInfo1 = new Mock<IFileInfo>();
        mockFileInfo1.Setup(f => f.FullName).Returns("ValidProject.csproj");
        mockFileInfo1.Setup(f => f.Extension).Returns(".csproj");
        mockFile1.Setup(f => f.FileSystemInfo).Returns(mockFileInfo1.Object);

        var mockFile2 = new Mock<IFileOrDirectoryInfo>();
        mockFile2.Setup(f => f.IsDirectory).Returns(false);
        var mockFileInfo2 = new Mock<IFileInfo>();
        mockFileInfo2.Setup(f => f.FullName).Returns("InvalidProject.txt");
        mockFileInfo2.Setup(f => f.Extension).Returns(".txt");
        mockFile2.Setup(f => f.FileSystemInfo).Returns(mockFileInfo2.Object);

        // Setup file existence
        this.mockFileSystem.Setup(fs => fs.File.Exists("ValidProject.csproj")).Returns(true);
        this.mockFileSystem.Setup(fs => fs.File.Exists("InvalidProject.txt")).Returns(true);

        // Setup directory enumeration
        var csprojFiles = new List<string>
        {
            "/path/to/directory/ProjectA.csproj",
            "/path/to/directory/ProjectB.csproj",
        };

        this.mockFileSystem.Setup(fs => fs.Directory.EnumerateFiles("/path/to/directory", "*.csproj", SearchOption.AllDirectories))
            .Returns(csprojFiles);

        this.mockFileSystem.Setup(fs => fs.FileInfo.New(It.IsAny<string>()))
            .Returns<string>(path =>
            {
                var mockFileInfo = new Mock<IFileInfo>();
                mockFileInfo.Setup(f => f.FullName).Returns(path);
                return mockFileInfo.Object;
            });

        var input = new List<IFileOrDirectoryInfo> { mockDirectory.Object, mockFile1.Object, mockFile2.Object };

        // Act
        var result = this.projectFinder.FindProjects(input).ToList();

        // Assert
        Assert.Equal(3, result.Count);
        Assert.Contains(result, f => f.FullName == "/path/to/directory/ProjectA.csproj");
        Assert.Contains(result, f => f.FullName == "/path/to/directory/ProjectB.csproj");
        Assert.Contains(result, f => f.FullName == "ValidProject.csproj");

        // "InvalidProject.txt" should not be included
    }

    [Fact]
    public void FindProjects_WithNonExistingFile_ReturnsEmptyForThatFile()
    {
        // Arrange
        var mockFile = new Mock<IFileOrDirectoryInfo>();
        mockFile.Setup(f => f.IsDirectory).Returns(false);
        var mockFileInfo = new Mock<IFileInfo>();
        mockFileInfo.Setup(f => f.FullName).Returns("NonExisting.csproj");
        mockFileInfo.Setup(f => f.Extension).Returns(".csproj");
        mockFile.Setup(f => f.FileSystemInfo).Returns(mockFileInfo.Object);

        this.mockFileSystem.Setup(fs => fs.File.Exists("NonExisting.csproj")).Returns(false);

        var input = new List<IFileOrDirectoryInfo> { mockFile.Object };

        // Act
        var result = this.projectFinder.FindProjects(input);

        // Assert
        Assert.Empty(result);
    }

    [Fact]
    public void FindProjectsInDirectory_WithNoCsprojFiles_ReturnsEmpty()
    {
        // Arrange
        var mockDirectory = new Mock<IFileOrDirectoryInfo>();
        mockDirectory.Setup(d => d.IsDirectory).Returns(true);
        var mockDirectoryInfo = new Mock<IDirectoryInfo>();
        mockDirectoryInfo.Setup(d => d.FullName).Returns("/empty/directory");
        mockDirectory.Setup(d => d.FileSystemInfo).Returns(mockDirectoryInfo.Object);

        this.mockFileSystem.Setup(fs => fs.Directory.EnumerateFiles("/empty/directory", "*.csproj", SearchOption.AllDirectories))
            .Returns(new List<string>()); // No .csproj files

        var input = new List<IFileOrDirectoryInfo> { mockDirectory.Object };

        // Act
        var result = this.projectFinder.FindProjects(input);

        // Assert
        Assert.Empty(result);
    }

    [Fact]
    public void FindProjectFile_WithInvalidExtension_ReturnsEmpty()
    {
        // Arrange
        var mockFile = new Mock<IFileOrDirectoryInfo>();
        mockFile.Setup(f => f.IsDirectory).Returns(false);
        var mockFileInfo = new Mock<IFileInfo>();
        mockFileInfo.Setup(f => f.FullName).Returns("Invalid.txt");
        mockFileInfo.Setup(f => f.Extension).Returns(".txt");
        mockFile.Setup(f => f.FileSystemInfo).Returns(mockFileInfo.Object);

        this.mockFileSystem.Setup(fs => fs.File.Exists("Invalid.txt")).Returns(true);

        // Act
        var result = this.projectFinder.FindProjects(mockFile.Object);

        // Assert
        Assert.Empty(result);
    }

    [Fact]
    public void FindProjectFiles_WithNonExistingFile_ReturnsEmpty()
    {
        // Arrange
        var mockFile = new Mock<IFileOrDirectoryInfo>();
        mockFile.Setup(f => f.IsDirectory).Returns(false);
        var mockFileInfo = new Mock<IFileInfo>();
        mockFileInfo.Setup(f => f.FullName).Returns("NonExisting.csproj");
        mockFileInfo.Setup(f => f.Extension).Returns(".csproj");
        mockFile.Setup(f => f.FileSystemInfo).Returns(mockFileInfo.Object);

        this.mockFileSystem.Setup(fs => fs.File.Exists("NonExisting.csproj")).Returns(false);

        // Act
        var result = this.projectFinder.FindProjects(mockFile.Object);

        // Assert
        Assert.Empty(result);
    }
}
