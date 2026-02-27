#nullable enable
using Content.Server.CombatMode;
using Content.Shared.CombatMode;
using Robust.Client.GameObjects;
using Robust.Client.Input;
using Robust.Shared.GameObjects;
using Robust.Shared.Input;
using Robust.Shared.Map;
using Robust.Shared.Maths;
using Robust.Shared.Timing;

namespace Content.IntegrationTests.Tests._Citadel;

public sealed partial class TestPlayer
{
    [SidedDependency(Side.Client)] private readonly IInputManager _inputManager = default!;
    [SidedDependency(Side.Client)] private readonly IGameTiming _clientTiming = default!;
    [SidedDependency(Side.Client)] private readonly IEntityManager _clientEntMan = default!;
    [System(Side.Client)] private readonly InputSystem _clientInputSys = default!;
    [System(Side.Server)] private readonly CombatModeSystem _serverCombatMode = default!;
    [System(Side.Server)] private readonly Robust.Server.GameObjects.TransformSystem _serverXformSys = default!;

    /// <summary>
    ///     Make the client press and then release a key. This assumes the key is currently released.
    ///     This will default to using the <see cref="Target"/> entity and <see cref="TargetCoords"/> coordinates.
    /// </summary>
    public async Task PressKey(
        BoundKeyFunction key,
        EntityCoordinates clientCoordinates,
        int ticks = 1,
        EntityUid? clientCursorEntity = null)
    {
        await SetKey(key, BoundKeyState.Down, clientCoordinates, clientCursorEntity);
        await Test.RunTicksSync(ticks);
        await SetKey(key, BoundKeyState.Up, clientCoordinates, clientCursorEntity);
        await Test.RunTicksSync(1);
    }

    /// <summary>
    ///     Make the client press or release a key.
    ///     This will default to using the <see cref="Target"/> entity and <see cref="TargetCoords"/> coordinates.
    /// </summary>
    public async Task SetKey(
        BoundKeyFunction key,
        BoundKeyState state,
        EntityCoordinates clientCoordinates,
        EntityUid? clientCursorEntity = null,
        ScreenCoordinates? screenCoordinates = null)
    {
        var target = clientCursorEntity ?? default;
        var screen = screenCoordinates ?? default;

        var funcId = _inputManager.NetworkBindMap.KeyFunctionID(key);
        var message = new ClientFullInputCmdMessage(_clientTiming.CurTick, _clientTiming.TickFraction, funcId)
        {
            State = state,
            Coordinates = clientCoordinates,
            ScreenCoordinates = screen,
            Uid = target,
        };

        await Client.WaitPost(() => _clientInputSys.HandleInputCommand(Client.Session, key, message));
    }

    /// <summary>
    ///     Variant of <see cref="SetKey"/> for setting movement keys.
    /// </summary>
    public async Task SetMovementKey(DirectionFlag dir, BoundKeyState state)
    {
        if ((dir & DirectionFlag.South) != 0)
            await SetKey(EngineKeyFunctions.MoveDown, state, EntityCoordinates.Invalid);

        if ((dir & DirectionFlag.East) != 0)
            await SetKey(EngineKeyFunctions.MoveRight, state, EntityCoordinates.Invalid);

        if ((dir & DirectionFlag.North) != 0)
            await SetKey(EngineKeyFunctions.MoveUp, state, EntityCoordinates.Invalid);

        if ((dir & DirectionFlag.West) != 0)
            await SetKey(EngineKeyFunctions.MoveLeft, state, EntityCoordinates.Invalid);
    }

    /// <summary>
    ///     Make the client hold the move key in some direction for some amount of time.
    /// </summary>
    public async Task Move(DirectionFlag dir, float seconds)
    {
        await SetMovementKey(dir, BoundKeyState.Down);
        await Test.RunSeconds(seconds);
        await SetMovementKey(dir, BoundKeyState.Up);
        await Test.RunTicksSync(1);
    }

    public async Task SetCombatMode(bool enabled)
    {
        if (!Test.SEntMan.TryGetComponent(SEntity, out CombatModeComponent? combat))
        {
            Assert.Fail($"Entity {Test.SEntMan.ToPrettyString(SEntity)} does not have a CombatModeComponent");
            return;
        }

        await Server.WaitPost(() => _serverCombatMode.SetInCombatMode(SEntity, enabled, combat));
        await Test.RunTicksSync(1);

        Assert.That(combat.IsInCombatMode, Is.EqualTo(enabled), $"Player could not set combat mode to {enabled}");
    }

    public async Task Punch(EntityUid target)
    {
        var clientEnt = Test.ToClientUid(target);

        var xform = Test.CComp<TransformComponent>(clientEnt);

        var clientCoords = xform.Coordinates;

        await SetCombatMode(true);

        await SetKey(EngineKeyFunctions.Use, BoundKeyState.Down, clientCoords, clientEnt);
        await SetKey(EngineKeyFunctions.Use, BoundKeyState.Up, clientCoords, clientEnt);

        await SetCombatMode(false);
    }
}
