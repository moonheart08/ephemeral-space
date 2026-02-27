using Content.Server.Hands.Systems;
using Content.Shared.Hands.Components;
using Robust.Shared.GameObjects;
using Robust.Shared.Prototypes;

namespace Content.IntegrationTests.Tests._Citadel;

public sealed partial class TestPlayer
{
    [System(Side.Server)] private readonly HandsSystem _serverHandsSys = default!;

    /// <summary>
    ///     Spawns an entity and has the player pick it up.
    /// </summary>
    /// <param name="proto"></param>
    public async Task SpawnAndPickUp(EntProtoId proto)
    {
        var target = EntityUid.Invalid;

        await Server.WaitPost(() =>
        {
            target = Test.SSpawnAtPosition(proto, Test.SComp<TransformComponent>(SEntity).Coordinates);
        });

        await PickUp(target);
    }

    /// <summary>
    ///     Has the player pick up the given entity.
    /// </summary>
    /// <param name="serverTarget">The server-side entity to pick up.</param>
    public async Task PickUp(EntityUid serverTarget)
    {
        var hands = Test.SComp<HandsComponent>(SEntity);

        if (hands.ActiveHandId == null)
            throw new NotSupportedException("Can't pick up things without an active hand");

        await Server.WaitPost(() =>
        {
            if (!_serverHandsSys.TryForcePickup(SEntity, serverTarget, hands.ActiveHandId, false, false))
                throw new Exception("Failed to actually pick anything up. Fatal to the test.");
        });

        await Test.RunTicksSync(1);
    }
}
