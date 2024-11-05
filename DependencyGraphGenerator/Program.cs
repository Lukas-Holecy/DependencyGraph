// <copyright file="Program.cs" company="Lukas Holecy">
// Copyright (c) Lukas Holecy. All rights reserved.
// </copyright>

namespace Holecy.Console.Dependencies;

using System.IO.Abstractions;
using CliFx;
using CliFx.Extensibility;
using Holecy.Console.Dependencies.IO;
using Microsoft.Extensions.DependencyInjection;

/// <summary>
/// Entry point of the application.
/// </summary>
public static class Program
{
    /// <summary>
    /// The main entry point for the application.
    /// </summary>
    /// <param name="args">An array of command-line arguments.</param>
    /// <returns>
    /// A task that represents the asynchronous operation.
    /// The task result contains an integer exit code.
    /// </returns>
    public static async Task<int> Main(string[] args)
    {
        var serviceProvider = SetUpDependencyInjection();

        return await new CliApplicationBuilder()
            .AddCommand<Commands.ProgramCommand>()
            .AddCommand<Commands.ListProjectsCommand>()
            .UseTypeActivator(serviceProvider.GetService!)
            .Build()
            .RunAsync();
    }

    private static ServiceProvider SetUpDependencyInjection()
    {
        var services = new ServiceCollection()
            .AddSingleton<IFileSystem, FileSystem>()
            .AddTransient<Commands.ProgramCommand>()
            .AddTransient<Commands.ListProjectsCommand>();

        var serviceProvider = services.BuildServiceProvider();
        return serviceProvider;
    }
}
