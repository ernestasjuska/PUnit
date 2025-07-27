namespace PUnit.TestFramework;

public sealed class TestResponse
{
    public bool Skipped { get; set; }

    public Exception? Exception { get; set; }
}
