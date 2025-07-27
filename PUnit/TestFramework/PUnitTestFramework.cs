using Microsoft.Extensions.DependencyInjection;
using Microsoft.Testing.Platform.Extensions.Messages;
using Microsoft.Testing.Platform.Extensions.TestFramework;
using Microsoft.Testing.Platform.Services;
using System;
using System.Diagnostics;
using System.Runtime.ExceptionServices;

namespace PUnit.TestFramework;

public class PUnitTestFramework(IServiceProvider services) : ITestFramework, IDataProducer
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
        await RunTestsAsync(context);
        context.Complete();
    }

    public Task<CloseTestSessionResult> CloseTestSessionAsync(CloseTestSessionContext context)
    {
        return Task.FromResult(new CloseTestSessionResult()
        {
            IsSuccess = true,
        });
    }

    private async Task RunTestsAsync(ExecuteRequestContext context)
    {
        //await RunTest(context, "Passing Test", "Passing Test");
        //await RunTest(context, "Failing Test", "Failing Test");

        var tests = ServiceProviderServiceExtensions.GetService<Tests>(services)!;
        await foreach (var test in tests.FindTestsAsync())
        {
            await RunTestAsync(context, test);
        }
    }

    [StackTraceHidden]
    public async Task RunTestAsync(ExecuteRequestContext context, Test test)
    {
        // TODO: This should come from somewhere else.
        CancellationTokenSource cancellationTokenSource = new();

        // TODO: Scoped services.

        TestContext testContext = new(cancellationTokenSource)
        {
            Request = new()
            {
                Test = test,
            },
            Response = new(),
            Services = services,
            CancellationToken = cancellationTokenSource.Token,
        };

        StackTrace stackTrace = new(1);

        try
        {
            await context.MessageBus.PublishAsync(
                this,
                new TestNodeUpdateMessage(
                   sessionUid: context.Request.Session.SessionUid,
                   testNode: new TestNode()
                   {
                       Uid = test.Uid,
                       DisplayName = test.DisplayName,
                       Properties = new PropertyBag(new DiscoveredTestNodeStateProperty(Explanation: "Hello!")),
                   }
                )
            );

            await RunTestActionAsync(testContext);

            var properties = new PropertyBag();
            if (testContext.Response.Skipped)
            {
                properties.Add(new SkippedTestNodeStateProperty(Explanation: "Skipped."));
            }
            else
            {
                properties.Add(new PassedTestNodeStateProperty(Explanation: "Works for me."));
            }

            await context.MessageBus.PublishAsync(
                this,
                new TestNodeUpdateMessage(
                   sessionUid: context.Request.Session.SessionUid,
                   testNode: new TestNode()
                   {
                       Uid = test.Uid,
                       DisplayName = test.DisplayName,
                       Properties = properties,
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
                       Uid = test.Uid,
                       DisplayName = test.DisplayName,
                       Properties = new PropertyBag(new FailedTestNodeStateProperty(e)),
                   }
                )
            );
        }
    }

    private async Task RunTestActionAsync(
        TestContext testContext)
    {
        // TODO: Ew. This is too hacky.

        Delegate action = testContext.Request.Test.Action;

        var parameters = action.Method.GetParameters();

        object?[] parameterValues = parameters
            .Where(x => !x.IsRetval)
            .Select(x =>
            {
                if (x.ParameterType == typeof(TestContext))
                {
                    return testContext;
                }

                return testContext.Services.GetService(x.ParameterType);
            })
            .ToArray();

        object? result = action.DynamicInvoke(parameterValues);
        if (result is Task<object?> objectTask)
        {
            result = await objectTask;
        }
        else if (result is Task voidTask)
        {
            await voidTask;
            result = null;
        }
    }
}
