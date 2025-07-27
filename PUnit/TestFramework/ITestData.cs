namespace PUnit.TestFramework;

public interface ITestData
{
    string Name { get; }
    Type TestDataType { get; }
    Type Type { get; }
    object? Value { get; }
}
