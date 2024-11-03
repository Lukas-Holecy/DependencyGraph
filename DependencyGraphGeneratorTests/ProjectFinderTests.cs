namespace Holecy.Console.Dependencies.Tests;

using System.Collections.Generic;
using System.IO.Abstractions;
using Moq;
using Xunit;

public class ProjectFinderTest
{
    private readonly Mock<IFileSystem> _fileSystemMock;
    private readonly ProjectFinder _projectFinder;

    public ProjectFinderTest()
    {
        _fileSystemMock = new Mock<IFileSystem>();
        _projectFinder = new ProjectFinder(_fileSystemMock.Object);
    }

    [Fact]
    public void FindProjects_WithEmptyInput_ReturnsEmpty()
    {
        // Arrange
        var input = new List<FileOrDirectoryInfo>();

        // Act
        var result = _projectFinder.FindProjects(input);

        // Assert
        Assert.Empty(result);
    }

    [Fact]
    public void FindProjects_WithSingleProjectFile_ReturnsProjectFile()
    {
        // Arrange
        var fileInfoMock = new Mock<IFileInfo>();
        fileInfoMock.Setup(f => f.Extension).Returns(".csproj");
        var fileOrDirectory = new FileOrDirectoryInfo(fileInfoMock.Object, false);
        var input = new List<FileOrDirectoryInfo> { fileOrDirectory };

        _fileSystemMock.Setup(fs => fs.File.Exists(It.IsAny<string>())).Returns(true);

        // Act
        var result = _projectFinder.FindProjects(input);

        // Assert
        Assert.Single(result);
    }

    [Fact]
    public void FindProjects_WithDirectoryContainingProjects_ReturnsAllProjectFiles()
    {
        // Arrange
        var directoryInfoMock = new Mock<IFileInfo>();
        directoryInfoMock.Setup(d => d.FullName).Returns("/projects");

        var dir = new FileOrDirectoryInfo(directoryInfoMock.Object, true);
        var input = new List<FileOrDirectoryInfo> { dir };

        var projectPaths = new List<string>
        {
            "/projects/ProjectA/ProjectA.csproj",
            "/projects/ProjectB/ProjectB.csproj"
        };

        _fileSystemMock.Setup(fs => fs.Directory.EnumerateFiles("/projects", "*.csproj", SearchOption.AllDirectories))
                       .Returns(projectPaths);

        _fileSystemMock.Setup(fs => fs.FileInfo.New(It.IsAny<string>())).Returns((string path) =>
        {
            var mockFileInfo = new Mock<IFileInfo>();
            mockFileInfo.Setup(f => f.FullName).Returns(path);
            return mockFileInfo.Object;
        });

        // Act
        var result = _projectFinder.FindProjects(input);

        // Assert
        Assert.Equal(2, result.Count());
    }

    [Fact]
    public void ValidateProjectFile_WithValidCsproj_ReturnsTrue()
    {
        // Arrange
        var fileInfoMock = new Mock<IFileInfo>();
        fileInfoMock.Setup(f => f.Extension).Returns(".csproj");
        var fileOrDirectory = new FileOrDirectoryInfo(fileInfoMock.Object, false);

        _fileSystemMock.Setup(fs => fs.File.Exists(It.IsAny<string>())).Returns(true);

        // Act
        var method = typeof(ProjectFinder).GetMethod("ValidateProjectFile", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        var result = (bool)method.Invoke(_projectFinder, new object[] { fileOrDirectory });

        // Assert
        Assert.True(result);
    }

    [Fact]
    public void ValidateProjectFile_WithInvalidExtension_ReturnsFalse()
    {
        // Arrange
        var fileInfoMock = new Mock<IFileInfo>();
        fileInfoMock.Setup(f => f.Extension).Returns(".txt");
        var fileOrDirectory = new FileOrDirectoryInfo(fileInfoMock.Object, false);

        _fileSystemMock.Setup(fs => fs.File.Exists(It.IsAny<string>())).Returns(true);

        // Act
        var method = typeof(ProjectFinder).GetMethod("ValidateProjectFile", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        var result = (bool)method.Invoke(_projectFinder, new object[] { fileOrDirectory });

        // Assert
        Assert.False(result);
    }

    [Fact]
    public void FindProjects_WithNonexistentFile_ReturnsEmpty()
    {
        // Arrange
        var fileInfoMock = new Mock<IFileInfo>();
        fileInfoMock.Setup(f => f.Extension).Returns(".csproj");
        var fileOrDirectory = new FileOrDirectoryInfo(fileInfoMock.Object, false);
        var input = new List<FileOrDirectoryInfo> { fileOrDirectory };

        _fileSystemMock.Setup(fs => fs.File.Exists(It.IsAny<string>())).Returns(false);

        // Act
        var result = _projectFinder.FindProjects(input);

        // Assert
        Assert.Empty(result);
    }
}
