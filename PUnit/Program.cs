var builder = await FancyTestApplication.CreateBuilderAsync(args);

using var testApp = await builder.BuildAsync();

testApp.MapTest("PassingTest", () => { });
testApp.MapTest("FailingTest", void () => throw new Exception("oops"));
testApp.MapTest("SkippedTest", void (TestContext test) => test.Skip());

return await testApp.RunAsync();
