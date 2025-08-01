using System.Numerics;
using Robust.Shared.Prototypes;

namespace Content.Server._ES.RespawnPoint;

public record struct OnAttemptRespawnPointActivation(EntityUid RespawnPoint, List<SpawnAction> Actions, bool Cancelled);

public record struct SpawnAction(EntProtoId ToSpawn)
{
    public Vector2 Offset = Vector2.Zero;
    public EntProtoId ToSpawn = ToSpawn;
}

/// <summary>
///     Base for the event fired when a respawn point has no manager and would like to find one.
/// </summary>
/// <param name="Manager">The manager entity, if any.</param>
public abstract record ESRespawnPointLocateManager(EntityUid? Manager);

[DataDefinition]
public sealed partial record ESRespawnPointSingularManager(
    EntityUid? Manager,
    EntProtoId ManagerPrototype) : ESRespawnPointLocateManager(Manager)
{
    [DataField] public EntProtoId ManagerPrototype { get; set; } = ManagerPrototype;

    public ESRespawnPointSingularManager() : this(EntityUid.Invalid, new EntProtoId())
    {

    }
}

[DataDefinition]
public sealed partial record ESRespawnPointGridWideManager(
    EntityUid? Manager,
    EntProtoId ManagerPrototype) : ESRespawnPointLocateManager(Manager)
{
    [DataField] public EntProtoId ManagerPrototype { get; set; } = ManagerPrototype;

    public ESRespawnPointGridWideManager() : this(EntityUid.Invalid, new EntProtoId())
    {

    }
}
