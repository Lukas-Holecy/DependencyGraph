// <copyright file="StringExtensionsTests.cs" company="Lukas Holecy">
// Copyright (c) Lukas Holecy. All rights reserved.
// </copyright>

namespace Holecy.Console.Dependencies.Tests.Extensions;

using Holecy.Console.Dependencies.Extensions;
using Xunit;

/// <summary>
/// Tests for <see cref="StringExtensions"/> class.
/// </summary>
public class StringExtensionsTests
{
    /// <summary>
    /// Tests the <see cref="StringExtensions.RemovePrefix"/> method.
    /// </summary>
    /// <param name="input">The input string to remove the prefix from.</param>
    /// <param name="prefix">The prefix to remove from the input string.</param>
    /// <param name="expected">The expected result after the prefix is removed.</param>
    [Theory]
    [InlineData("PackageId:SomeValue", "PackageId:", "SomeValue")]
    [InlineData("Path:SomePath", "Path:", "SomePath")]
    [InlineData("NoPrefix", "Prefix:", "NoPrefix")]
    [InlineData("Prefix:NoMatch", "NoMatch:", "Prefix:NoMatch")]
    [InlineData("", "Prefix:", "")]
    [InlineData("Prefix:", "Prefix:", "")]
    public void RemovePrefix_RemovesPrefixCorrectly(string input, string prefix, string expected)
    {
        // Act
        var result = StringExtensions.RemovePrefix(input, prefix);

        // Assert
        Assert.Equal(expected, result);
    }

    /// <summary>
    /// Tests the <see cref="StringExtensions.RemovePrefix"/> method when the input is null.
    /// </summary>
    [Fact]
    public void RemovePrefix_InputIsNull_ThrowsArgumentNullException()
    {
        // Arrange
        const string? input = null;
        const string prefix = "Prefix:";

        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => StringExtensions.RemovePrefix(input!, prefix));
    }

    /// <summary>
    /// Tests the <see cref="StringExtensions.RemovePrefix"/> method when the prefix is null.
    /// </summary>
    [Fact]
    public void RemovePrefix_PrefixIsNull_ThrowsArgumentNullException()
    {
        // Arrange
        const string input = "Prefix:Value";
        const string? prefix = null;

        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => StringExtensions.RemovePrefix(input, prefix!));
    }
}
