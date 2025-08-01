using Content.Server._ES.RespawnPoint.Systems;

namespace Content.Server._ES.RespawnPoint.Components;

/// <summary>
/// Controls individual respawn point behavior
/// </summary>
[RegisterComponent]
public sealed partial class ESRespawnPointComponent : Component
{
    /// <summary>
    ///     Whether this respawn point can activate if it is being observed (nearby player.)
    ///     When true, this respawn point can activate at any time.
    /// </summary>
    [DataField]
    public bool AllowRespawnWhenObserved = false;

    /// <summary>
    ///     The respawn point manager for this respawn point, if any.
    ///     This must be set by mapinit at latest.
    /// </summary>
    [DataField, Access(friends: typeof(ESRespawnPointSystem))]
    public EntityUid? Manager;

    /// <summary>
    ///     Controls how to locate the manager for this respawn point.
    /// </summary>
    [DataField(required: true)]
    public ESRespawnPointLocateManager ManagerLocator = null!;
}
