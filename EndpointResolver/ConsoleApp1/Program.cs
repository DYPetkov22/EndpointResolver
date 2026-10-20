using ConsoleApp1.Routing;

namespace ConsoleApp1;

internal class Program
{
    private static void Main(string[] args)
    {
        var resolver = new EndpointResolver();

        resolver.BuildEndpoints();

        Console.WriteLine("Available endpoints:");
        foreach (string endpoint in resolver.GetEndpoints())
        {
            Console.WriteLine($"GET https://localhost/{endpoint}");
        }

        Console.WriteLine();

        string url = args.Length > 0
            ? args[0]
            : "https://localhost/api/Watch/PlayVideo?id=10";

        Console.WriteLine($"Resolving: {url}");
        resolver.ExecuteEndpoint(url);
    }
}
