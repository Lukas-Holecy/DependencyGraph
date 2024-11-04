// <copyright file="Program.cs" company="Lukas Holecy">
// "This is a .NET 8 project file."
// </copyright>

namespace Holecy.Console.Dependencies;

using System.IO.Abstractions;
using CliFx;
using Microsoft.Extensions.DependencyInjection;

/// <summary>
/// Entry point of the application.
/// </summary>
public static class Program
{
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
