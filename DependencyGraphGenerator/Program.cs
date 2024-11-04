namespace Holecy.Console.Dependencies;

using System.IO.Abstractions;
using Microsoft.Extensions.DependencyInjection;
using CliFx;


/// <summary>
/// Entry point of the application.
/// </summary>
public class Program
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
