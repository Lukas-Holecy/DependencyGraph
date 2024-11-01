namespace Holecy.Console.Dependencies;

using CliFx;

/// <summary>
/// Entry point of the application.
/// </summary>
public class Program
{
    public static async Task<int> Main(string[] args)
    {
        return await new CliApplicationBuilder()
            .AddCommandsFromThisAssembly()
            .Build()
            .RunAsync();
    }
}
