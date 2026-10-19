using System.Reflection;

namespace EndpointResolver.Routing;

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
