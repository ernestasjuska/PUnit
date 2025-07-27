using Microsoft.Testing.Platform.Builder;

namespace PUnit.TestFramework;

public sealed class FancyTestApplication : IDisposable
{
    private readonly ITestApplication _testApplication;

    internal FancyTestApplication(
        ITestApplication testApplication,
        IServiceProvider services)
    {
        _testApplication = testApplication;
        Services = services;
    }

    public IServiceProvider Services { get; private set; }

    public void MapTest(
        string testName,
        Delegate action)
    {
        ((Tests)Services.GetService(typeof(Tests))!).RegisterTest(new Test
        {
            Uid = testName,
            DisplayName = testName,
            Action = action,
            Data = [],
        });
    }

    public Task<int> RunAsync()
    {
        return _testApplication.RunAsync();
    }

    public void Dispose()
    {
        _testApplication.Dispose();
    }

    public static async Task<FancyTestApplicationBuilder> CreateBuilderAsync(
        string[] args,
        TestApplicationOptions? options = null)
    {
        return await FancyTestApplicationBuilder.CreateAsync(args, options);
    }
}
