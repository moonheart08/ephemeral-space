using NUnit.Framework.Interfaces;
using NUnit.Framework.Internal.Commands;

namespace Content.IntegrationTests.Tests._Citadel;

[AttributeUsage(AttributeTargets.Method | AttributeTargets.Class)]
public sealed class TestMapAttribute(TestMapMode testMapMode) : Attribute, ITestAction
{
    public TestMapMode TestMapMode { get; } = testMapMode;

    public void BeforeTest(ITest test)
    {
        if (test.Fixture is not GameTest gt)
        {
            throw new NotSupportedException(
                $"The fixture {test.Fixture?.GetType()} needs to be a GameTest for SidedTest to work.");
        }

        Task.WaitAll(Task.Run(async () =>
        {
            await gt.CreateTestMap(TestMapMode);
        }));
    }

    public void AfterTest(ITest test)
    {
        // nothin
    }

    public ActionTargets Targets => ActionTargets.Default;
}
