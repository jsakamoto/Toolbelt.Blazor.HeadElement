using Microsoft.Extensions.Configuration;

namespace HeadElement.E2ETest.Internals;

public static class ConfigurationExtensions
{
    public static IConfigurationBuilder AddTestParameters(this IConfigurationBuilder builder)
    {
        var parameters = NUnit.Framework.TestContext.Parameters;
        var initialData = parameters.Names.ToDictionary(name => name, name => parameters[name]);
        return builder.AddInMemoryCollection(initialData);
    }
}
