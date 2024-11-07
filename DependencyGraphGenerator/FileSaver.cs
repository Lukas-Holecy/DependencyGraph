// <copyright file="FileSaver.cs" company="Lukas Holecy">
// Copyright (c) Lukas Holecy. All rights reserved.
// </copyright>

namespace Holecy.Console.Dependencies;

using System;
using System.IO;
using System.IO.Abstractions;
using System.Text;

public class FileSaver
{
    public static bool SaveStringToFile(string content, string path, IFileSystem fileSystem)
    {
        try
        {
            var outputPath = GetDotFilePath(path, fileSystem);
            string directoryPath = fileSystem.Path.GetDirectoryName(outputPath) ?? string.Empty;
            if (string.IsNullOrEmpty(directoryPath))
            {
                Console.WriteLine("Failed to get the directory path.");
                return false;
            }

            if (!fileSystem.Directory.Exists(directoryPath))
            {
                fileSystem.Directory.CreateDirectory(directoryPath);
            }

            // Overwrite the file with UTF-8 encoding
            fileSystem.File.WriteAllText(outputPath, content, Encoding.UTF8);
            Console.WriteLine($"File saved successfully at: {path}");
            return true;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Failed to save the file: {ex.Message}");
            return false;
        }
    }

    private static string GetDotFilePath(string outputPath, IFileSystem fileSystem)
    {
        if (fileSystem.Path.HasExtension(outputPath) && fileSystem.File.Exists(outputPath))
        {
            return GetFilePath(outputPath, fileSystem);
        }

        return GetDirectoryPath(outputPath, fileSystem);
    }

    private static string GetDirectoryPath(string outputPath, IFileSystem fileSystem)
    {
        var fullPath = fileSystem.Path.Combine(outputPath, "DependencyGraph.dot");
        return GetFilePath(fullPath, fileSystem);
    }

    private static string GetFilePath(string outputPath, IFileSystem fileSystem)
    {
        try
        {
            // FileInfo will throw if the path is invalid
            var fileInfo = fileSystem.FileInfo.New(outputPath);
            return outputPath;
        }
        catch (Exception)
        {
            return string.Empty;
        }
    }

}
