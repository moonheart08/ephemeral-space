namespace Content.IntegrationTests.Tests._Citadel.Attributes;

public interface IGameTestModifier
{
    Task ApplyToTest(GameTest test);
}

