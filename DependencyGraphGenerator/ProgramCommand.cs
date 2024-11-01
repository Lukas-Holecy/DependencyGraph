namespace Holecy.Console.Dependencies;

using CliFx;
using CliFx.Attributes;
using CliFx.Exceptions;
using CliFx.Infrastructure;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

[Command(Description = "Processes files or directories with optional logging and output.")]
public class ProcessCommand : ICommand
{
    // Positional parameter for paths (files or directories)
    [CommandParameter(0, Name = "paths", Description = "Paths to directories or files to process.", IsRequired = true)]
    public IReadOnlyList<string> Paths { get; init; }

    // -l or --log switch to enable logging
    [CommandOption("log", 'l', Description = "Enable logging.")]
    public bool EnableLogging { get; init; }

    // --log-path option to set the path to the log file
    [CommandOption("log-path", Description = "Path to the log file.")]
    public string LogPath { get; init; }

    // -o or --output option to specify the output file
    [CommandOption("output", 'o', Description = "Path to the output file.")]
    public string OutputPath { get; init; }

    public async ValueTask ExecuteAsync(IConsole console)
    {
        // Validate paths
        if (Paths == null || Paths.Count == 0)
        {
            throw new CommandException("At least one path must be specified.");
        }

        // Handle logging
        if (EnableLogging)
        {
            var logFilePath = LogPath ?? "default.log";
            await console.Output.WriteLineAsync($"Logging enabled. Log file: {logFilePath}");
            // Initialize logging to the specified log file
            // For example, set up a logger here
        }

        // Process each path
        foreach (var path in Paths)
        {
            if (File.Exists(path))
            {
                await console.Output.WriteLineAsync($"Processing file: {path}");
                // Add your file processing logic here
            }
            else if (Directory.Exists(path))
            {
                await console.Output.WriteLineAsync($"Processing directory: {path}");
                // Add your directory processing logic here
            }
            else
            {
                await console.Error.WriteLineAsync($"Path not found: {path}");
            }
        }

        // Handle output
        if (!string.IsNullOrWhiteSpace(OutputPath))
        {
            await console.Output.WriteLineAsync($"Output will be saved to: {OutputPath}");
            // Save output to the specified file
        }
        else
        {
            await console.Output.WriteLineAsync("No output file specified.");
            // Handle default output behavior, if any
        }
    }
}
