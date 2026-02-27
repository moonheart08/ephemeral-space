#nullable enable
using System.Runtime.CompilerServices;
using System.Threading;
using Content.Shared.Mind;
using Content.Shared.Players;
using Robust.Shared.GameObjects;
using Robust.Shared.Map;
using Robust.Shared.Player;
using Robust.UnitTesting;

namespace Content.IntegrationTests.Tests._Citadel;

public sealed partial class TestPlayer : IResolvesToEntity
{
    private GameTest Test;
    private RobustIntegrationTest.ClientIntegrationInstance Client;
    private RobustIntegrationTest.ServerIntegrationInstance Server;

    /// <summary>
    ///     The server-side entity of this player.
    /// </summary>
    public EntityUid SEntity { get; private set; }
    EntityUid? IResolvesToEntity.SEntity => SEntity;

    /// <summary>
    ///     The client-side entity of this player.
    /// </summary>
    public EntityUid CEntity { get; private set; }
    EntityUid? IResolvesToEntity.CEntity => CEntity;

    /// <summary>
    ///     The net-entity of this player.
    /// </summary>
    public NetEntity NetEntity { get; private set; }

    /// <summary>
    ///     The server-side mind of the player.
    /// </summary>
    public Entity<MindComponent> SMindEntity { get; private set; }
    /// <summary>
    ///     The client-side mind of the player.
    /// </summary>
    public Entity<MindComponent> CMindEntity { get; private set; }
    /// <summary>
    ///     The net-entity of the player's mind.
    /// </summary>
    public NetEntity MindNetEntity { get; private set; }

    private TestPlayer(GameTest test)
    {
        Test = test;
        // REMARK: We specify the client explicitly for some future where we can have more than one
        //         client attached in a test if we need it.
        Client = test.Client;
        Server = test.Server;
    }

    /// <summary>
    ///     Creates a new player using the attached client of the test.
    /// </summary>
    /// <param name="test">The test we're creating this player for.</param>
    /// <param name="playerProto">The prototype to use for the player</param>
    /// <param name="location">Where to spawn the player. By default, the TestMap is used.</param>
    /// <returns>The constructed TestPlayer.</returns>
    public static async Task<TestPlayer> CreatePlayer(GameTest test, string playerProto = "MobHuman", EntityCoordinates? location = null)
    {
        var player = new TestPlayer(test);
        player.MutualThreadSanity();

        test.InjectDependencies(player);

        if (test.TestMap is not { } map)
        {
            // Fiddlesticks, we can't *make* a player now.
            throw new NotSupportedException("A test map must be created for GTPlayer to exist within.");
        }

        // TODO: Support multiple clients.
        var session = test.ServerSession;

        if (session is null)
            throw new NotSupportedException("The provided client is not connected to the server.");

        if (session.AttachedEntity is not null)
            throw new NotSupportedException($"The provided client is already attached to a player. Detach it first, possibly with {nameof(Destroy)}()");

        // By default, put them dead center of the map.
        location ??= map.GridCoords;

        await player.Server.WaitPost(() =>
        {
            var sEntity = test.SSpawnAtPosition(playerProto, location.Value);
            var sMindSys = player.Server.EntMan.System<SharedMindSystem>();

            // interactiontest cargo cult
            sMindSys.WipeMind(session.ContentData()?.Mind);

            player.SMindEntity = sMindSys.GetOrCreateMind(session.UserId);
            player.MindNetEntity = player.Server.EntMan.GetNetEntity(player.SMindEntity);

            sMindSys.TransferTo(player.SMindEntity, sEntity, true, false);
            player.Server.PlayerMan.SetAttachedEntity(session, sEntity);

            player.SEntity = sEntity;
            player.NetEntity = player.Server.EntMan.GetNetEntity(sEntity);
        });

        await test.RunTicksSync(5);

        await player.Client.WaitPost(() =>
        {
            var cEntity = player.Client.EntMan.GetEntity(player.NetEntity);
            var cMindEntity = player.Client.EntMan.GetEntity(player.MindNetEntity);

            player.CEntity = cEntity;
            player.CMindEntity = (cMindEntity, test.CComp<MindComponent>(cMindEntity));
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

    private void MutualThreadSanity([CallerMemberName] string caller = "<unknown>")
    {
        // We may await on both the client and server in various methods, so we check we're not there.
        // TODO: Account for multiple clients here..
        if (Thread.CurrentThread == Test.ClientThread || Thread.CurrentThread == Test.ServerThread)
            throw new NotSupportedException($"Cannot use {caller} on the client or server thread, it will deadlock.");
    }

    private void AssertServer([CallerMemberName] string caller = "<unknown>")
    {
        if (Thread.CurrentThread != Test.ServerThread)
            throw new NotSupportedException($"Cannot use {caller} outside of the server thread.");
    }

    private void AssertClient([CallerMemberName] string caller = "<unknown>")
    {
        if (Thread.CurrentThread != Test.ClientThread)
            throw new NotSupportedException($"Cannot use {caller} outside of the server thread.");
    }
}
