var builder = await FancyTestApplication.CreateBuilderAsync(args);

builder.TestServices.AddService<MyMathService>();

using var testApp = await builder.BuildAsync();

testApp.MapTest("PassingTest", () => { });
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
testApp.MapTest("FailingTest", void () => throw new Exception("oops"));
testApp.MapTest("SkippedTest", void (TestContext test) => test.Skip());

return await testApp.RunAsync();

public class MyMathService
{
    public int Add(int a, int b) => a + b;
    public int UnsafeAdd(int a, int b) => a + b + 1;
}