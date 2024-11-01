using Holecy.Console.Dependencies;
using Moq;
using System.IO.Abstractions;
using Xunit;

public class FileOrDirectoryInfoTests
{
    [Fact]
    public void Constructor_ShouldInitializeAsFile_WhenPathIsFile()
    {
        // Arrange
        var path = "test.txt";
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
