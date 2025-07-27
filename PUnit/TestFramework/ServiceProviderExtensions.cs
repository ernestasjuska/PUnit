using System.ComponentModel.Design;

namespace PUnit.TestFramework;

internal static class DependencyInjectionExtensions
{
    public static void AddService<T>(
        this ServiceContainer services)
            where T : class, new()
    {
        services.AddService(services => new T());
    }

    public static void AddService<T>(
        this ServiceContainer services,
        Func<IServiceProvider, T> factory)
    {
        services.AddService(typeof(T), new ServiceCreatorCallback((s, t) => factory(s)));
    }
}
