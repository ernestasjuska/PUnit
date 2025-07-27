using Microsoft.Testing.Platform.Builder;
using Microsoft.Testing.Platform.Capabilities.TestFramework;
using Microsoft.Testing.Platform.CommandLine;
using Microsoft.Testing.Platform.Configurations;
using Microsoft.Testing.Platform.Logging;
using Microsoft.Testing.Platform.TestHost;
using Microsoft.Testing.Platform.TestHostControllers;
using System.ComponentModel.Design;

namespace PUnit.TestFramework;

public sealed class FancyTestApplicationBuilder
{
    private readonly ITestApplicationBuilder _builder;
    private readonly ServiceContainer _testServices;
    private readonly Tests _tests;

    private FancyTestApplicationBuilder(
        ITestApplicationBuilder builder,
        ServiceContainer testServices,
        Tests tests)
    {
        _builder = builder;
        _testServices = testServices;
        _tests = tests;
    }

    public ITestHostManager TestHost => _builder.TestHost;

    public ITestHostControllersManager TestHostControllers => _builder.TestHostControllers;

    public ICommandLineManager CommandLine => _builder.CommandLine;

#pragma warning disable TPEXP // Type is for evaluation purposes only and is subject to change or removal in future updates. Suppress this diagnostic to proceed.
    public IConfigurationManager Configuration => _builder.Configuration;
#pragma warning restore TPEXP // Type is for evaluation purposes only and is subject to change or removal in future updates. Suppress this diagnostic to proceed.

#pragma warning disable TPEXP // Type is for evaluation purposes only and is subject to change or removal in future updates. Suppress this diagnostic to proceed.
    public ILoggingManager Logging => _builder.Logging;
#pragma warning restore TPEXP // Type is for evaluation purposes only and is subject to change or removal in future updates. Suppress this diagnostic to proceed.

    public ServiceContainer TestServices => _testServices;

    public async Task<FancyTestApplication> BuildAsync() =>
        new FancyTestApplication(await _builder.BuildAsync(), _testServices);

    internal static async Task<FancyTestApplicationBuilder> CreateAsync(
        string[] args,
        TestApplicationOptions? options)
    {
        var basicBuilder = await TestApplication.CreateBuilderAsync(args, options);

        ServiceContainer testServices = new();

        Tests tests = new();
        testServices.AddService(typeof(Tests), tests);

        basicBuilder.RegisterTestFramework(
            capabilitiesFactory: (services) =>
            {
                return new TestFrameworkCapabilities();
            },
            adapterFactory: (capabilities, services) =>
            {
                return new PUnitTestFramework(testServices);
            });

        FancyTestApplicationBuilder builder = new(basicBuilder, testServices, tests);

        return builder;
    }

    internal void AddTestData<T>(string name, T value)
    {
        _tests.RegisterTestData(new TestData<T>(name, value));
    }
}
