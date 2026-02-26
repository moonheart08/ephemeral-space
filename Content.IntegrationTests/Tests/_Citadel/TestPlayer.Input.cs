#nullable enable
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

    /// <summary>
    ///     Make the client press and then release a key. This assumes the key is currently released.
    ///     This will default to using the <see cref="Target"/> entity and <see cref="TargetCoords"/> coordinates.
    /// </summary>
    public async Task PressKey(
        BoundKeyFunction key,
        NetCoordinates coordinates,
        int ticks = 1,
        NetEntity? cursorEntity = null)
    {
        await SetKey(key, BoundKeyState.Down, coordinates, cursorEntity);
        await Test.RunTicksSync(ticks);
        await SetKey(key, BoundKeyState.Up, coordinates, cursorEntity);
        await Test.RunTicksSync(1);
    }

    /// <summary>
    ///     Make the client press or release a key.
    ///     This will default to using the <see cref="Target"/> entity and <see cref="TargetCoords"/> coordinates.
    /// </summary>
    public async Task SetKey(
        BoundKeyFunction key,
        BoundKeyState state,
        NetCoordinates coordinates,
        NetEntity? cursorEntity = null,
        ScreenCoordinates? screenCoordinates = null)
    {
        var coords = coordinates;
        var target = cursorEntity ?? default;
        var screen = screenCoordinates ?? default;

        var funcId = _inputManager.NetworkBindMap.KeyFunctionID(key);
        var message = new ClientFullInputCmdMessage(_clientTiming.CurTick, _clientTiming.TickFraction, funcId)
        {
            State = state,
            Coordinates = _clientEntMan.GetCoordinates(coords),
            ScreenCoordinates = screen,
            Uid = _clientEntMan.GetEntity(target),
        };

        await Client.WaitPost(() => _clientInputSys.HandleInputCommand(Client.Session, key, message));
    }

    /// <summary>
    ///     Variant of <see cref="SetKey"/> for setting movement keys.
    /// </summary>
    public async Task SetMovementKey(DirectionFlag dir, BoundKeyState state)
    {
        if ((dir & DirectionFlag.South) != 0)
            await SetKey(EngineKeyFunctions.MoveDown, state, NetCoordinates.Invalid);

        if ((dir & DirectionFlag.East) != 0)
            await SetKey(EngineKeyFunctions.MoveRight, state, NetCoordinates.Invalid);

        if ((dir & DirectionFlag.North) != 0)
            await SetKey(EngineKeyFunctions.MoveUp, state, NetCoordinates.Invalid);

        if ((dir & DirectionFlag.West) != 0)
            await SetKey(EngineKeyFunctions.MoveLeft, state, NetCoordinates.Invalid);
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

}
