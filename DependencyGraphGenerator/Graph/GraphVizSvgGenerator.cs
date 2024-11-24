// <copyright file="GraphVizSvgGenerator.cs" company="Lukas Holecy">
// Copyright (c) Lukas Holecy. All rights reserved.
// </copyright>

namespace Holecy.Console.Dependencies.Graph;

using System;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;

/// <summary>
/// Class for generating svg stream from dot input using GraphViz.
/// </summary>
public static class GraphVizSvgGenerator
{
    /// <summary>
    /// Generates SVG stream from DOT input using GraphViz command line utility.
    /// This expects the dot utility to be in the PATH.
    /// </summary>
    /// <param name="dotInput">Dot representation of a graph.</param>
    /// <returns>Memory stream with svg image in it.</returns>
    public static async Task<MemoryStream> GenerateSVGStreamAsync(string dotInput)
    {
        try
        {
            return await GenerateImageStream(dotInput, "-Tsvg");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"An error occurred: {ex.Message}");
            throw;
        }
    }

    /// <summary>
    /// Generates Png stream from DOT input using GraphViz command line utility.
    /// This expects the dot utility to be in the PATH.
    /// </summary>
    /// <param name="dotInput">Dot representation of a graph.</param>
    /// <returns>Memory stream with svg image in it.</returns>
    public static async Task<MemoryStream> GeneratePNGStreamAsync(string dotInput)
    {
        try
        {
            return await GenerateImageStream(dotInput, "-Tpng");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"An error occurred: {ex.Message}");
            throw;
        }
    }

    private static async Task<MemoryStream> GenerateImageStream(string dotInput, string outputFormat)
    {
        var memoryStream = new MemoryStream();
        var process = GetDotProcessInfo(outputFormat);
        process.Start();

        await WriteDotProcessInput(dotInput, process);

        // Read the output (svg data) into the memory stream
        await process.StandardOutput.BaseStream.CopyToAsync(memoryStream);
        await process.WaitForExitAsync();

        if (process.ExitCode != 0)
        {
            throw new Exception("Graphviz process failed to generate output.");
        }

        memoryStream.Position = 0;
        return memoryStream;
    }

    private static async Task WriteDotProcessInput(string dotInput, Process process)
    {
        // Write DOT data to the input of the process
        await process.StandardInput.WriteAsync(dotInput);
        await process.StandardInput.FlushAsync();
        process.StandardInput.Close(); // Close the standard input to signal EOF
    }

    private static Process GetDotProcessInfo(string outputFormat)
    {
        var processStartInfo = new ProcessStartInfo
        {
            FileName = "dot", // Graphviz executable (assuming it's in PATH)
            Arguments = outputFormat, // Output format
            RedirectStandardInput = true, // Redirects DOT input
            RedirectStandardOutput = true, // Redirects output (Svg data)
            UseShellExecute = false,
            CreateNoWindow = true,
        };

        var process = new Process { StartInfo = processStartInfo };
        return process;
    }
}
