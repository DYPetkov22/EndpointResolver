using System.Globalization;
using System.Reflection;
using ConsoleApp1.Attributes;
using ConsoleApp1.Controllers;

namespace ConsoleApp1.Routing;

public sealed class EndpointResolver
{
    private readonly Dictionary<string, EndpointDefinition> _endpoints =
        new(StringComparer.OrdinalIgnoreCase);

    public void BuildEndpoints()
    {
        _endpoints.Clear();

        List<Type> controllers = Assembly
            .GetExecutingAssembly()
            .GetTypes()
            .Where(type =>
                !type.IsAbstract &&
                typeof(ControllerBase).IsAssignableFrom(type))
            .ToList();

        foreach (Type controller in controllers)
        {
            RouteAttribute? route = controller.GetCustomAttribute<RouteAttribute>();

            if (route is null)
            {
                continue;
            }

            List<MethodInfo> methods = controller
                .GetMethods(
                    BindingFlags.Public |
                    BindingFlags.Instance |
                    BindingFlags.DeclaredOnly)
                .Where(method =>
                    method.GetCustomAttribute<HttpGetAttribute>() is not null)
                .ToList();

            foreach (MethodInfo method in methods)
            {
                string controllerName = controller.Name.EndsWith("Controller")
                    ? controller.Name[..^"Controller".Length]
                    : controller.Name;

                string endpointPath = route.Path
                    .Replace("[controller]", controllerName, StringComparison.OrdinalIgnoreCase)
                    .Replace("[action]", method.Name, StringComparison.OrdinalIgnoreCase)
                    .Trim('/');

                if (!_endpoints.TryAdd(
                        endpointPath,
                        new EndpointDefinition(controller, method)))
                {
                    throw new InvalidOperationException(
                        $"Duplicate endpoint found: {endpointPath}");
                }
            }
        }
    }

    public IReadOnlyCollection<string> GetEndpoints()
    {
        return _endpoints.Keys
            .OrderBy(endpoint => endpoint)
            .ToArray();
    }

    public void ExecuteEndpoint(string url)
    {
        if (_endpoints.Count == 0)
        {
            throw new InvalidOperationException(
                "No endpoints found. Call BuildEndpoints() first.");
        }

        if (!Uri.TryCreate(url, UriKind.Absolute, out Uri? uri))
        {
            throw new ArgumentException("The URL is not valid.", nameof(url));
        }

        string path = uri.AbsolutePath.Trim('/');

        if (!_endpoints.TryGetValue(path, out EndpointDefinition? endpoint))
        {
            throw new InvalidOperationException(
                $"Endpoint '{path}' was not found.");
        }

        object controllerInstance =
            Activator.CreateInstance(endpoint.ControllerType)
            ?? throw new InvalidOperationException(
                $"Could not create controller '{endpoint.ControllerType.Name}'.");

        Dictionary<string, string> queryParameters =
            ParseQueryString(uri.Query);

        ParameterInfo[] methodParameters = endpoint.Method.GetParameters();
        object?[] arguments = new object?[methodParameters.Length];

        for (int i = 0; i < methodParameters.Length; i++)
        {
            ParameterInfo parameter = methodParameters[i];

            if (parameter.Name is null ||
                !queryParameters.TryGetValue(parameter.Name, out string? rawValue))
            {
                throw new InvalidOperationException(
                    $"Missing parameter '{parameter.Name}' for endpoint '{path}'.");
            }

            arguments[i] = ConvertValue(rawValue, parameter.ParameterType);
        }

        endpoint.Method.Invoke(controllerInstance, arguments);
    }

    private static Dictionary<string, string> ParseQueryString(string query)
    {
        var result = new Dictionary<string, string>(
            StringComparer.OrdinalIgnoreCase);

        if (string.IsNullOrWhiteSpace(query))
        {
            return result;
        }

        string trimmedQuery = query.TrimStart('?');

        foreach (string part in trimmedQuery.Split(
                     '&',
                     StringSplitOptions.RemoveEmptyEntries))
        {
            string[] pair = part.Split('=', 2);

            string key = Uri.UnescapeDataString(pair[0]);
            string value = pair.Length == 2
                ? Uri.UnescapeDataString(pair[1])
                : string.Empty;

            result[key] = value;
        }

        return result;
    }

    private static object? ConvertValue(string value, Type targetType)
    {
        Type actualType = Nullable.GetUnderlyingType(targetType) ?? targetType;

        if (actualType == typeof(string))
        {
            return value;
        }

        if (actualType.IsEnum)
        {
            return Enum.Parse(actualType, value, ignoreCase: true);
        }

        return Convert.ChangeType(
            value,
            actualType,
            CultureInfo.InvariantCulture);
    }
}
