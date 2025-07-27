namespace PUnit.TestFramework;

public sealed class Tests
{
    private readonly Dictionary<string, Test> _tests = [];
    private readonly Dictionary<Type, List<ITestData>> _testDataByType = [];

#pragma warning disable CS1998 // Async method lacks 'await' operators and will run synchronously
    public async IAsyncEnumerable<Test> FindTestsAsync()
#pragma warning restore CS1998 // Async method lacks 'await' operators and will run synchronously
    {
        foreach (var test in _tests.Values)
        {
            foreach (Test testWithData in GenerateTestsWithData(test))
            {
                yield return testWithData;
            }
        }
    }

    public void RegisterTest(Test test)
    {
        _tests[test.Uid] = test;
    }

    public void RegisterTestData(ITestData testData)
    {
        if (!_testDataByType.TryGetValue(testData.TestDataType, out List<ITestData>? list))
        {
            list = [];
            _testDataByType[testData.TestDataType] = list;
        }

        list.Add(testData);
    }

    private IEnumerable<Test> GenerateTestsWithData(Test test)
    {
        Type[] testDataTypes = test
            .GetTestDataParameters()
            .Select(x => x.ParameterType)
            .Distinct()
            .Where(_testDataByType.ContainsKey)
            .ToArray();

        ITestData[] data = new ITestData[testDataTypes.Length];
        
        int[] lengths = new int[testDataTypes.Length];
        for (int i = 0; i < data.Length; i++)
        {
            Type type = testDataTypes[i];
            lengths[i] = _testDataByType[type].Count;
        }

        int[] indices = new int[testDataTypes.Length];

        int carry = 0;
        do
        {
            for (int i = 0; i < indices.Length; i++)
            {
                Type type = testDataTypes[i];
                int index = indices[i];
                data[i] = _testDataByType[type][index];
            }
            yield return test.WithData(data);

            carry = 1;
            for (int i = data.Length - 1; carry == 1 && i >= 0; i--)
            {
                if (indices[i] + carry < lengths[i])
                {
                    indices[i] += carry;
                    carry = 0;
                }
                else
                {
                    indices[i] = 0;
                }
            }
        }
        while (carry == 0);
    }
}
