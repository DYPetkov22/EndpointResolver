namespace ConsoleApp1.Attributes;

[AttributeUsage(AttributeTargets.Class)]
public sealed class RouteAttribute : Attribute
{
    public string Path { get; }

    public RouteAttribute(string path)
    {
        Path = path;
    }
}
