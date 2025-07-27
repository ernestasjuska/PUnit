using System.ComponentModel.Design;

namespace PUnit.TestFramework;

internal static class DependencyInjectionExtensions
{
    public static void AddService<T>(this ServiceContainer services) where T : class, new()
    {
        services.AddService(typeof(T), new T());
    }
}
