using System.Reflection;

namespace PUnit.TestFramework;

public sealed class Test
{
    public required string Uid { get; init; }

    public required string DisplayName { get; init; }

    public required Delegate Action { get; init; }

    public ITestData[] Data { get; init; } = [];

    public Test WithData(IEnumerable<ITestData> data)
    {
        return new()
        {
            Uid = Uid + string.Join("", data.Select(x => "_" + x.Type.Name + "_" + x.Name)),
            DisplayName = DisplayName + string.Join("", data.Select(x => " " + x.Type.Name + " " + x.Name)),
            Action = Action,
            Data = data.ToArray(),
        };
    }

    public IEnumerable<ParameterInfo> GetTestDataParameters()
    {
        return Action.Method
            .GetParameters()
            .Where(x =>
                !x.IsRetval &&
                x.ParameterType.IsGenericType &&
                x.ParameterType.GetGenericTypeDefinition() == typeof(TestData<>));
    }
}
