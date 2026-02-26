using Content.IntegrationTests.Tests._Citadel.Constraints;

namespace Content.IntegrationTests.Tests._Citadel.TestTests;

public sealed class TestPlayerTests : GameTest
{

    public static readonly string[] TestMobs = ["InteractionTestMob", "MobHuman", "MobObserver", "AdminObserver"];

    [Test]
    [TestCaseSource(nameof(TestMobs))]
    public async Task MakePlayer(string proto)
    {
        await CreateTestMap();

        var player = await TestPlayer.CreatePlayer(this, Client, playerProto: proto);

        using (Assert.EnterMultipleScope())
        {
            Assert.That(player.CEntity, Is.Initialized(Client));
            Assert.That(player.SEntity, Is.MapInitialized(Server));
        }
    }

    [Test]
    public async Task ErroneousNoMap()
    {
        // No map made here.

        Assert.CatchAsync<NotSupportedException>(async () =>
        {
            _ = await TestPlayer.CreatePlayer(this, Client);
        });
    }

    [Test]
    public async Task ErroneousMakeManyPlayers()
    {
        await CreateTestMap();

        _ = await TestPlayer.CreatePlayer(this, Client);

        Assert.CatchAsync<NotSupportedException>(async () =>
        {
            // We didn't detach first...
            _ = await TestPlayer.CreatePlayer(this, Client);
        });
    }

    [Test]
    public async Task MakeManyPlayers()
    {
        await CreateTestMap();

        var player = await TestPlayer.CreatePlayer(this, Client);

        await player.Destroy();

        _ = await TestPlayer.CreatePlayer(this, Client);
    }
}
