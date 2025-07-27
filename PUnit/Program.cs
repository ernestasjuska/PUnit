var builder = await FancyTestApplication.CreateBuilderAsync(args);

builder.TestServices.AddService<MyMathService>();
builder.AddTestData("1 + 2 = 3", new MyTestData(1, 2, 3));
builder.AddTestData("999 + 1 = 1001", new MyTestData(999, 1, 1001));

using var testApp = await builder.BuildAsync();

testApp.MapTest("PassingTest", () => { });
testApp.MapTest("FailingTest", void () => throw new Exception("oops"));
testApp.MapTest("SkippedTest", void (TestContext test) => test.Skip());
testApp.MapTest("MathIsOk", (MyMathService math) =>
{
    if (math.Add(1, 2) != 3)
    {
        throw new Exception("world ending event");
    }
});
testApp.MapTest("MathIsNotOk", (MyMathService math) =>
{
    if (math.UnsafeAdd(1, 2) != 3)
    {
        throw new Exception("world ending event");
    }
});
testApp.MapTest("MathWithMultipleTestData", (MyMathService math, TestData<MyTestData> testData) =>
{
    if (math.UnsafeAdd(testData.Value.A, testData.Value.B) != testData.Value.Sum)
    {
        throw new Exception("world ending event");
    }
});

return await testApp.RunAsync();

public class MyMathService
{
    public int Add(int a, int b) => a + b;
    public int UnsafeAdd(int a, int b) => a + b + 1;
}

public record MyTestData(int A, int B, int Sum);
