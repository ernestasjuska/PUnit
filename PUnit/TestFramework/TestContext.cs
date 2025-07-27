namespace PUnit.TestFramework;

public sealed class TestContext
{
    private readonly CancellationTokenSource _cancellationTokenSource;

    public TestContext(CancellationTokenSource cancellationTokenSource)
    {
        _cancellationTokenSource = cancellationTokenSource;
    }

    public required TestRequest Request { get; init; }

    public required TestResponse Response { get; init; }

    public required IServiceProvider Services { get; init; }

    public required CancellationToken CancellationToken { get; init; }

    public void Skip()
    {
        Response.Skipped = true;
        _cancellationTokenSource.Cancel();
    }
}
