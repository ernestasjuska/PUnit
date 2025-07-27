using Microsoft.Testing.Platform.Capabilities.TestFramework;
using Microsoft.Testing.Platform.Extensions.Messages;
using Microsoft.Testing.Platform.Extensions.TestFramework;

var builder = await Microsoft.Testing.Platform.Builder.TestApplication.CreateBuilderAsync(args);

builder.RegisterTestFramework(
    capabilitiesFactory: (services) =>
    {
        return new TestFrameworkCapabilities();
    },
    adapterFactory: (capabilities, services) =>
    {
        return new PUnit();
    });

using var testApp = await builder.BuildAsync();

return await testApp.RunAsync();

public class PUnit : ITestFramework, IDataProducer
{
    public string Uid => "PUnit";

    public string Version => "1.0.0";

    public string DisplayName => "PUnit";

    public string Description => "This is PUnit.";

    public Type[] DataTypesProduced =>
    [
        typeof(TestNodeUpdateMessage)
    ];

    public Task<bool> IsEnabledAsync()
    {
        return Task.FromResult(true);
    }

    public Task<CreateTestSessionResult> CreateTestSessionAsync(CreateTestSessionContext context)
    {
        return Task.FromResult(new CreateTestSessionResult()
        {
            IsSuccess = true,
        });
    }

    public async Task ExecuteRequestAsync(ExecuteRequestContext context)
    {
        await RunTests(context);

        context.Complete();
    }

    public Task<CloseTestSessionResult> CloseTestSessionAsync(CloseTestSessionContext context)
    {
        return Task.FromResult(new CloseTestSessionResult()
        {
            IsSuccess = true,
        });
    }

    private async Task RunTests(ExecuteRequestContext context)
    {
        await RunTest(context, "Passing Test", "Passing Test");
        await RunTest(context, "Failing Test", "Failing Test");
    }

    public async Task RunTest(ExecuteRequestContext context, string testUid, string testDisplayName)
    {
        try
        {
            await context.MessageBus.PublishAsync(
                this,
                new TestNodeUpdateMessage(
                   sessionUid: context.Request.Session.SessionUid,
                   testNode: new TestNode()
                   {
                       Uid = testUid,
                       DisplayName = testDisplayName,
                       Properties = new PropertyBag(new DiscoveredTestNodeStateProperty(Explanation: "Hello!")),
                   }
                )
            );

            if (testUid.Contains("Failing"))
            {
                throw new Exception("oops");
            }

            await context.MessageBus.PublishAsync(
                this,
                new TestNodeUpdateMessage(
                   sessionUid: context.Request.Session.SessionUid,
                   testNode: new TestNode()
                   {
                       Uid = testUid,
                       DisplayName = testDisplayName,
                       Properties = new PropertyBag(new PassedTestNodeStateProperty(Explanation: "Works for me.")),
                   }
                )
            );
        }
        catch (Exception e)
        {
            await context.MessageBus.PublishAsync(
                this,
                new TestNodeUpdateMessage(
                   sessionUid: context.Request.Session.SessionUid,
                   testNode: new TestNode()
                   {
                       Uid = testUid,
                       DisplayName = testDisplayName,
                       Properties = new PropertyBag(new FailedTestNodeStateProperty(e)),
                   }
                )
            );
        }
    }
}
