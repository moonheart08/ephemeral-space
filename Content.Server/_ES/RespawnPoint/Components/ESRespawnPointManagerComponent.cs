namespace Content.Server._ES.RespawnPoint.Components;

/// <summary>
/// This is used by the "manager" for a respawn point, which handles the actual entity pool among other things.
/// </summary>
[RegisterComponent, AutoGenerateComponentPause]
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

    /// <summary>
    ///     The target number of entities to maintain.
    /// </summary>
    [DataField(required: true)]
    public int TargetEntityCount = 1;

    /// <summary>
    ///     The number of entities to spawn per operation.
    ///     This can cause the target to be overshot by N - 1 entities.
    /// </summary>
    [DataField(required: true)]
    public int ToSpawnPerOperation = 1;

    /// <summary>
    ///     How often an "operation" should occur.
    /// </summary>
    [DataField(required: true)]
    public TimeSpan OperationInterval = TimeSpan.Zero;

    /// <summary>
    ///     At what point in game time did the last operation occur.
    /// </summary>
    [DataField, AutoPausedField]
    public TimeSpan LastOperation = TimeSpan.Zero;
}


