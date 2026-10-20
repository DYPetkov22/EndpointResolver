using System.Reflection;

namespace ConsoleApp1.Routing;

internal sealed class EndpointDefinition
{
    public Type ControllerType { get; }
    public MethodInfo Method { get; }

    public EndpointDefinition(Type controllerType, MethodInfo method)
    {
        ControllerType = controllerType;
        Method = method;
    }
}
