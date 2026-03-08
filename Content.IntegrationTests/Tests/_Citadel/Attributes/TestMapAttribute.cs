namespace Content.IntegrationTests.Tests._Citadel.Attributes;

[AttributeUsage(AttributeTargets.Method | AttributeTargets.Class)]
public sealed class TestMapAttribute(TestMapMode testMapMode) : Attribute, IGameTestModifier
{
    public async Task ApplyToTest(GameTest test)
    {
        await test.CreateTestMap(testMapMode);
    }
}
