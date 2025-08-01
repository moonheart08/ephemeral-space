namespace Content.Server._ES.RespawnPoint.Components;

/// <summary>
/// This is used by the "manager" for a respawn point, which handles the actual entity pool among other things.
/// </summary>
[RegisterComponent]
public sealed partial class ESRespawnPointManagerComponent : Component
{
    /// <summary>
    ///     The list of entities attached to this respawn point manager.
    /// </summary>
    [DataField]
    public HashSet<EntityUid> AttachedEntities = new();

    /// <summary>
    ///     The list of all respawn points that use this manager.
    /// </summary>
    [DataField]
    public HashSet<EntityUid> AttachedRespawnPoints = new();
}


