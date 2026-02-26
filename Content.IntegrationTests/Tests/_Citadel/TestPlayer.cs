using System.Threading;
using Content.Shared.Mind;
using Content.Shared.Players;
using Robust.Shared.GameObjects;
using Robust.Shared.Map;
using Robust.Shared.Player;
using Robust.UnitTesting;

namespace Content.IntegrationTests.Tests._Citadel;

public sealed class TestPlayer
{
    private GameTest Test;
    private RobustIntegrationTest.ClientIntegrationInstance Client;
    private RobustIntegrationTest.ServerIntegrationInstance Server;

    public Entity<ActorComponent> SEntity { get; private set; }
    public Entity<ActorComponent> CEntity { get; private set; }
    public NetEntity NetEntity { get; private set; }

    private TestPlayer(GameTest test, RobustIntegrationTest.ClientIntegrationInstance client)
    {
        Test = test;
        // REMARK: We specify the client explicitly for some future where we can have more than one
        //         client attached in a test if we need it.
        Client = client;
        Server = test.Server;
    }

    public static async Task<TestPlayer> CreatePlayer(GameTest test, RobustIntegrationTest.ClientIntegrationInstance client, string playerProto = "InteractionTestMob", EntityCoordinates? location = null)
    {
        if (client != test.Client)
            throw new NotImplementedException("Multi-client TestPlayer not supported yet.");

        var player = new TestPlayer(test, client);
        player.MutualThreadSanity();

        if (test.TestMap is not { } map)
        {
            // Fiddlesticks, we can't *make* a player now.
            throw new NotSupportedException("A test map must be created for GTPlayer to exist within.");
        }

        // TODO: Support multiple clients.
        var session = test.Player;

        if (session is null)
            throw new NotSupportedException("The provided client is not connected to the server.");

        if (session.AttachedEntity is not null)
            throw new NotSupportedException("The provided client is already attached to a player. Detach it first.");

        // By default, put them dead center of the map.
        location ??= map.GridCoords;

        await player.Server.WaitPost(() =>
        {
            var sEntity = test.SSpawnAtPosition(playerProto, location.Value);

            // interactiontest cargo cult
            player.Server.EntMan.System<SharedMindSystem>().WipeMind(session.ContentData()?.Mind);
            player.Server.PlayerMan.SetAttachedEntity(session, sEntity);

            player.SEntity = (sEntity, test.SComp<ActorComponent>(sEntity));
            player.NetEntity = player.Server.EntMan.GetNetEntity(sEntity);
        });

        await test.RunTicksSync(5);

        await player.Client.WaitPost(() =>
        {
            var cEntity = player.Client.EntMan.GetEntity(player.NetEntity);
            player.CEntity = (cEntity, test.CComp<ActorComponent>(cEntity));
        });

        return player;
    }

    /// <summary>
    ///     Destroys the test player, detaching them from the client.
    /// </summary>
    public async Task Destroy()
    {
        await Server.WaitPost(() =>
        {
            Test.SDeleteNow(SEntity);
        });

        await Test.RunTicksSync(5);
    }

    private void MutualThreadSanity()
    {
        // We may await on both the client and server in various methods, so we check we're not there.
        // TODO: Account for multiple clients here..
        if (Thread.CurrentThread == Test.ClientThread || Thread.CurrentThread == Test.ServerThread)
            throw new NotSupportedException("Cannot use this GTPlayer method from the client or server thread, it will deadlock.");
    }
}
