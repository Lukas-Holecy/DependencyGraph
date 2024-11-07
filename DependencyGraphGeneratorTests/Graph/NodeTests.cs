// <copyright file="NodeTests.cs" company="Lukas Holecy">
// Copyright (c) Lukas Holecy. All rights reserved.
// </copyright>

namespace Holecy.Console.Dependencies.Tests.Graph;

using System.Runtime.InteropServices;
using Holecy.Console.Dependencies.Graph;
using Holecy.Console.Dependencies.ProjectFiles;
using Moq;
using Xunit;

/// <summary>
/// Contains unit tests for the <see cref="Node"/> class, verifying its behavior.
/// </summary>
public class NodeTests
{
    /// <summary>
    /// Tests that the constructor initializes correctly when provided with valid project information.
    /// </summary>
    /// <param name="packageId">Package id of mock project information.</param>
    /// <param name="path">Path of mock project information.</param>
    [Theory]
    [InlineData("", "")]
    [InlineData("testPackageId", "")]
    [InlineData("", "testPath")]
    [InlineData("testPackageId", "testPath")]
    [InlineData("testPackageId", null)]
    [InlineData(null, "testPath")]
    [InlineData(null, null)]
    public void Constructor_InitializesCorrectly_WhenValidProjectInfo(string? packageId, string? path)
    {
        // Arrange
        var projectInfo = new Mock<IProjectInformation>();
        projectInfo.SetupGet(pi => pi.PackageId).Returns(packageId!);
        projectInfo.SetupGet(pi => pi.Path).Returns(path!);

        // Act
        var node = new Node(projectInfo.Object);

        // Assert
        Assert.Equal(packageId ?? string.Empty, node.PackageId);
        Assert.Equal(path ?? string.Empty, node.Path);
    }

    /// <summary>
    /// Tests that the constructor initializes correctly when provided with valid reference information.
    /// </summary>
    /// <param name="packageId">Package id (NuGet name).</param>
    /// <param name="path">Path to the project file.</param>
    [Theory]
    [InlineData("", "")]
    [InlineData("testPackageId", "")]
    [InlineData("", "testPath")]
    [InlineData("testPackageId", "testPath")]
    [InlineData("testPackageId", null)]
    [InlineData(null, "testPath")]
    [InlineData(null, null)]
    public void Constructor_InitializesCorrectly_WhenUsingStrings(string? packageId, string? path)
    {
        // Act
        var node = new Node(packageId!, path!);

        // Assert
        Assert.Equal(packageId ?? string.Empty, node.PackageId);
        Assert.Equal(path ?? string.Empty, node.Path);
    }

    /// <summary>
    /// Tests that the constructor throws an exception when provided with a null parameter.
    /// </summary>
    [Fact]
    public void Constructor_ThrowsException_WhenNullParameter()
    {
        // Arrange
        IProjectInformation? projectInfo = null;

        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => new Node(projectInfo!));
    }

    /// <summary>
    /// Tests that the IsSameProject method returns true when one or both of the package IDs match.
    /// </summary>
    /// <param name="packageId1">Package Id of the first node.</param>
    /// <param name="path1">Path of the first node.</param>
    /// <param name="packageId2">Package Id of the second node.</param>
    /// <param name="path2">Path of the second node.</param>
    [Theory]
    [InlineData("", "", "", "")]
    [InlineData(null, null, null, null)]
    [InlineData("testPackageId", "testPath", "testPackageId", "testPath")]
    [InlineData("testPackageId", "testPath", "testPackageId", "")]
    [InlineData("testPackageId", "testPath", "", "testPath")]
    public void TryCompareTwoNodes_IsSameProject_WhenOneIDMatches(
        string? packageId1, string? path1, string? packageId2, string? path2)
    {
        // Arrange
        var node1 = new Node(packageId1!, path1!);
        var node2 = new Node(packageId2!, path2!);

        // Act
        var result = node1.IsSameProject(node2);

        // Assert
        Assert.True(result);
    }

    /// <summary>
    /// Tests that the IsSameProject method returns false none of the node ids match.
    /// </summary>
    /// <param name="packageId1">Package Id of the first node.</param>
    /// <param name="path1">Path of the first node.</param>
    /// <param name="packageId2">Package Id of the second node.</param>
    /// <param name="path2">Path of the second node.</param>
    [Theory]
    [InlineData("testPackageId1", "testPath1", "testPackageId2", "testPath2")]
    [InlineData("testPackageId1", "testPath1", "testPackageId2", "")]
    [InlineData("testPackageId1", "testPath1", "", "testPath2")]
    [InlineData("testPackageId1", "testPath1", "", "")]
    [InlineData("", "", "testPackageId2", "testPath2")]
    [InlineData("", "testPath1", "testPackageId2", "")]
    [InlineData("", "testPath1", "", "testPath2")]
    [InlineData("", "testPath1", "", "")]
    [InlineData("testPackageId1", "", "testPackageId2", "testPath2")]
    [InlineData("testPackageId1", "", "testPackageId2", "")]
    [InlineData("testPackageId1", "", "", "testPath2")]
    [InlineData("testPackageId1", "", "", "")]
    [InlineData("", "", "", "testPath2")]
    [InlineData("testPackageId1", "testPath1", null, "testPath2")]
    [InlineData("testPackageId1", "testPath1", null, null)]
    [InlineData(null, null, "testPackageId2", "testPath2")]
    [InlineData(null, "testPath1", "testPackageId2", null)]
    [InlineData(null, "testPath1", null, "testPath2")]
    [InlineData(null, "testPath1", null, null)]
    [InlineData("testPackageId1", null, "testPackageId2", "testPath2")]
    [InlineData("testPackageId1", null, "testPackageId2", null)]
    [InlineData("testPackageId1", null, null, "testPath2")]
    [InlineData("testPackageId1", null, null, null)]
    [InlineData(null, null, null, "testPath2")]
    public void TryCompareTwoNodes_AreNotEqual_WhenBothIDsNotMatch(
        string? packageId1, string? path1, string? packageId2, string? path2)
    {
        // Arrange
        var node1 = new Node(packageId1!, path1!);
        var node2 = new Node(packageId2!, path2!);

        // Act
        var result = node1.IsSameProject(node2);

        // Assert
        Assert.False(result);
    }
}
