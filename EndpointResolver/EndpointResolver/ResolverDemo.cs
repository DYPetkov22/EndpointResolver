namespace EndpointResolver;

/// <summary>
/// Original reflection-based EndpointResolver example, runnable with --demo.
/// It invokes the controller method locally and does not make an HTTP request.
/// </summary>
public static class ResolverDemo
{
    public static void Run(string[] args)
    {
        var resolver = new Routing.EndpointResolver();
        resolver.BuildEndpoints();

        Console.WriteLine("Available reflection endpoints:");
        foreach (string endpoint in resolver.GetEndpoints())
        {
            Console.WriteLine($"GET https://localhost/{endpoint}");
        }

        string url = args.SkipWhile(arg =>
                !string.Equals(arg, "--demo", StringComparison.OrdinalIgnoreCase))
            .Skip(1)
            .FirstOrDefault()
            ?? "https://localhost/api/Watch/PlayVideo?id=10";

        Console.WriteLine($"\nResolving locally: {url}");
        try
        {
            resolver.ExecuteEndpoint(url);
        }
        catch (Exception exception) when (
            exception is ArgumentException or FormatException or OverflowException)
        {
            Console.Error.WriteLine($"Cannot resolve endpoint: {exception.Message}");
            Environment.ExitCode = 1;
        }
    }
}
