// <copyright file="StringExtensions.cs" company="Lukas Holecy">
// Copyright (c) Lukas Holecy. All rights reserved.
// </copyright>

namespace Holecy.Console.Dependencies.Extensions;

/// <summary>
/// Extension methods for <see cref="string"/> class.
/// </summary>
internal static class StringExtensions
{
    /// <summary>
    /// Returns the input string without the prefix.
    /// </summary>
    /// <param name="input">Input string.</param>
    /// <param name="prefix">Prefix to be removed if it exists.</param>
    /// <returns><paramref name="input"/> string without the <paramref name="prefix"/>.</returns>
    public static string RemovePrefix(this string input, string prefix)
    {
        ArgumentNullException.ThrowIfNull(input, nameof(input));
        ArgumentNullException.ThrowIfNull(prefix, nameof(prefix));
        if (input.StartsWith(prefix))
        {
            return input.Substring(prefix.Length);
        }

        return input;
    }
}
