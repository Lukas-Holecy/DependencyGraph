// <copyright file="FileSaver.cs" company="Lukas Holecy">
// Copyright (c) Lukas Holecy. All rights reserved.
// </copyright>

namespace Holecy.Console.Dependencies.IO;

using System;
using System.IO;
using System.IO.Abstractions;
using System.Text;

/// <summary>
/// Saves the content to the file.
/// </summary>
/// <param name="fileSystem">File System abstraction.</param>
public class FileSaver(IFileSystem fileSystem)
{
    private readonly IFileSystem fileSystem = fileSystem;

    /// <summary>
    /// Saves the content to the file as UTF-8 string.
    /// </summary>
    /// <param name="content">String to save.</param>
    /// <param name="path">Path to the file. This can be either a file path or directory.
    /// If directory is used, the file DependencyGraph.dot will be used.
    /// </param>
    /// <param name="extension">
    /// Extension of the file. If left empty, extension in the <paramref name="path"/> will be used.
    /// </param>
    /// <returns>True if successful, false otherwise.</returns>
    public bool SaveStringToFile(string content, string path, string extension = "")
    {
        try
        {
            var outputPath = this.GetDotFilePath(path, extension);
            string directoryPath = this.fileSystem.Path.GetDirectoryName(outputPath) ?? string.Empty;
            if (string.IsNullOrEmpty(directoryPath))
            {
                Console.WriteLine("Failed to get the directory path.");
                return false;
            }

            if (!this.fileSystem.Directory.Exists(directoryPath))
            {
                this.fileSystem.Directory.CreateDirectory(directoryPath);
            }

            // Overwrite the file with UTF-8 encoding
            this.fileSystem.File.WriteAllText(outputPath, content, Encoding.UTF8);
            Console.WriteLine($"File saved successfully at: {path}");
            return true;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Failed to save the file: {ex.Message}");
            return false;
        }
    }

    /// <summary>
    /// Saves the content to the file as bytes (stream).
    /// </summary>
    /// <param name="content">Stream to save.</param>
    /// <param name="path">Path to the file. This can be either a file path or directory.
    /// If directory is used, the file DependencyGraph.dot will be used.
    /// </param>
    /// <param name="extension">
    /// Extension of the file. If left empty, extension in the <paramref name="path"/> will be used.
    /// </param>
    /// <returns>True if successful, false otherwise.</returns>
    public bool SaveStreamToFile(MemoryStream content, string path, string extension = "")
    {
        try
        {
            var outputPath = this.GetDotFilePath(path, extension);
            if (!string.IsNullOrEmpty(extension))
            {
                outputPath = this.fileSystem.Path.ChangeExtension(outputPath, extension);
            }

            string directoryPath = this.fileSystem.Path.GetDirectoryName(outputPath) ?? string.Empty;
            if (string.IsNullOrEmpty(directoryPath))
            {
                Console.WriteLine("Failed to get the directory path.");
                return false;
            }

            if (!this.fileSystem.Directory.Exists(directoryPath))
            {
                this.fileSystem.Directory.CreateDirectory(directoryPath);
            }

            this.fileSystem.File.WriteAllBytes(outputPath, content.ToArray());
            Console.WriteLine($"File saved successfully at: {path}");
            return true;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Failed to save the file: {ex.Message}");
            return false;
        }
    }

    private string GetDotFilePath(string outputPath, string extension)
    {
        if (this.fileSystem.Path.HasExtension(outputPath) && this.fileSystem.File.Exists(outputPath))
        {
            return this.GetFilePath(outputPath, extension);
        }

        return this.GetDirectoryPath(outputPath, extension);
    }

    private string GetDirectoryPath(string outputPath, string extension)
    {
        var fullPath = this.fileSystem.Path.Combine(outputPath, "DependencyGraph");
        fullPath = this.fileSystem.Path.ChangeExtension(fullPath, extension);
        return this.GetFilePath(fullPath, extension);
    }

    private string GetFilePath(string outputPath, string extension)
    {
        if (!string.IsNullOrEmpty(extension))
        {
            outputPath = this.fileSystem.Path.ChangeExtension(outputPath, extension);
        }

        try
        {
            // FileInfo will throw if the path is invalid
            var fileInfo = this.fileSystem.FileInfo.New(outputPath);
            return outputPath;
        }
        catch (Exception)
        {
            return string.Empty;
        }
    }
}
