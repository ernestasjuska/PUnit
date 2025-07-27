namespace PUnit.TestFramework;

public sealed class Test
{
    public required string Uid { get; init; }

    public required string DisplayName { get; init; }

    public required Delegate Action { get; init; }
}