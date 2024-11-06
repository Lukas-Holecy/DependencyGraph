// <copyright file="ProjectInfoExtractorTests.cs" company="Lukas Holecy">
// Copyright (c) Lukas Holecy. All rights reserved.
// </copyright>

namespace Holecy.Console.Dependencies.Tests.ProjectFiles;

using System.IO.Abstractions.TestingHelpers;
using Holecy.Console.Dependencies.ProjectFiles;
using Xunit;

/// <summary>
/// Contains unit tests for the <see cref="ProjectInfoExtractor"/> class.
/// </summary>
public class ProjectInfoExtractorTests
{
    /// <summary>
    /// Tests that <see cref="ProjectInfoExtractor.TryExtractProjectInformation"/>
    /// returns <c>true</c> and correctly extracts information from a valid project file.
    /// </summary>
    /// <param name="projectContent">The content of the project file.</param>
    [Theory]
    [InlineData(
        """
            <Project Sdk="Microsoft.NET.Sdk">
                <PropertyGroup>
                <AssemblyName>TestAssembly</AssemblyName>
                <PackageId>TestPackage</PackageId>
                </PropertyGroup>
                <ItemGroup>
                <PackageReference Include="Newtonsoft.Json" Version="12.0.3" />
                <ProjectReference Include="..\AnotherProject\AnotherProject.csproj" />
                <Reference Include="System.Xml" />
                </ItemGroup>
            </Project>
        """)]
    public void TryExtractProjectInformation_ValidProjectFile_ReturnsTrue(string projectContent)
    {
        // Arrange
        var mockFileSystem = new MockFileSystem();
        var projectPath = @"C:\TestProject\TestProject.csproj";
        mockFileSystem.AddFile(projectPath, new MockFileData(projectContent));

        var fileInfo = mockFileSystem.FileInfo.New(projectPath);

        // Act
        var result = ProjectInfoExtractor.TryExtractProjectInformation(fileInfo, out var projectInfo, mockFileSystem);

        // Assert
        Assert.True(result);
        Assert.NotNull(projectInfo);
        Assert.Equal("TestAssembly", projectInfo.Name);
        Assert.Equal("TestPackage", projectInfo.PackageId);
        Assert.Equal(projectPath, projectInfo.Path);
        Assert.NotEmpty(projectInfo.References);
        Assert.Contains(projectInfo.References, r => r is PackageReference && r.Name == "Newtonsoft.Json");
        Assert.Contains(projectInfo.References, r => r is ProjectReference && r.Name == @"C:\AnotherProject\AnotherProject.csproj");
        Assert.Contains(projectInfo.References, r => r is AssemblyReference && r.Name == "System.Xml");
    }

        /// <summary>
    /// Tests that <see cref="ProjectInfoExtractor.TryExtractProjectInformation"/>
    /// returns <c>true</c> and correctly extracts information from a valid project file.
    /// </summary>
    /// <param name="projectContent">The content of the project file.</param>
    [Theory]
    [InlineData(
        """
            <Project Sdk="Microsoft.NET.Sdk">
                <PropertyGroup>
                <AssemblyName>TestAssembly</AssemblyName>
                <PackageId>TestPackage</PackageId>
                </PropertyGroup>
                <ItemGroup>
                <PackageReference Include="Newtonsoft.Json" Version="12.0.3" />
                <ProjectReference Include="..\AnotherProject\AnotherProject.csproj" />
                <Reference Include="System.Xml" />
                </ItemGroup>
            </Project>
        """)]
    public void TryExtractProjectInformationFromAlternativePath_ValidProjectFile_ReturnsTrue(string projectContent)
    {
        // Arrange
        var mockFileSystem = new MockFileSystem();
        var projectPath = @"C:\Source\Projects\TestProject\TestProject.csproj";
        mockFileSystem.AddFile(projectPath, new MockFileData(projectContent));

        var fileInfo = mockFileSystem.FileInfo.New(projectPath);

        // Act
        var result = ProjectInfoExtractor.TryExtractProjectInformation(fileInfo, out var projectInfo, mockFileSystem);

        // Assert
        Assert.True(result);
        Assert.NotNull(projectInfo);
        Assert.Equal("TestAssembly", projectInfo.Name);
        Assert.Equal("TestPackage", projectInfo.PackageId);
        Assert.Equal(projectPath, projectInfo.Path);
        Assert.NotEmpty(projectInfo.References);
        Assert.Contains(projectInfo.References, r => r is PackageReference && r.Name == "Newtonsoft.Json");
        Assert.Contains(projectInfo.References, r => r is ProjectReference && r.Name == @"C:\Source\Projects\AnotherProject\AnotherProject.csproj");
        Assert.Contains(projectInfo.References, r => r is AssemblyReference && r.Name == "System.Xml");
    }

    /// <summary>
    /// Tests that <see cref="ProjectInfoExtractor.TryExtractProjectInformation"/>
    /// returns <c>false</c> when provided with an invalid project file.
    /// </summary>
    [Fact]
    public void TryExtractProjectInformation_InvalidProjectFile_ReturnsFalse()
    {
        // Arrange
        var mockFileSystem = new MockFileSystem();
        var projectPath = @"C:\TestProject\InvalidProject.csproj";
        mockFileSystem.AddFile(projectPath, new MockFileData("Invalid Content"));

        var fileInfo = mockFileSystem.FileInfo.New(projectPath);

        // Act
        var result = ProjectInfoExtractor.TryExtractProjectInformation(fileInfo, out var projectInfo);

        // Assert
        Assert.False(result);
        Assert.Null(projectInfo);
    }

    /// <summary>
    /// Tests that <see cref="ProjectInfoExtractor.TryExtractProjectInformation"/>
    /// returns <c>false</c> when the project file does not exist.
    /// </summary>
    [Fact]
    public void TryExtractProjectInformation_FileDoesNotExist_ReturnsFalse()
    {
        // Arrange
        var mockFileSystem = new MockFileSystem();
        const string projectPath = @"C:\TestProject\NonExistent.csproj";
        var fileInfo = mockFileSystem.FileInfo.New(projectPath);

        // Act
        var result = ProjectInfoExtractor.TryExtractProjectInformation(fileInfo, out var projectInfo);

        // Assert
        Assert.False(result);
        Assert.Null(projectInfo);
    }
}
