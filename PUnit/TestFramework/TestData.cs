namespace PUnit.TestFramework;

public record TestData<T>(
    string Name,
    T Value)
        : ITestData
{
    Type ITestData.TestDataType => typeof(TestData<T>);

    Type ITestData.Type => typeof(T);

    object? ITestData.Value => Value;
}
