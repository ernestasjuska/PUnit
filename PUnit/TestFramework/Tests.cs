namespace PUnit.TestFramework;

public sealed class Tests
{
    private readonly Dictionary<string, Test> _tests = [];

#pragma warning disable CS1998 // Async method lacks 'await' operators and will run synchronously
    public async IAsyncEnumerable<Test> FindTestsAsync()
#pragma warning restore CS1998 // Async method lacks 'await' operators and will run synchronously
    {
        foreach (var test in _tests.Values)
        {
            yield return test;
        }
    }

    public void RegisterTest(Test test)
    {
        _tests[test.Uid] = test;
    }
}
