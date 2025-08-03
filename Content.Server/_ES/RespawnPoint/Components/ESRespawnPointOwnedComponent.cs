namespace Content.Server._ES.RespawnPoint.Components;

/// <summary>
/// This is used for objects managed by a respawn point, to track their status.
/// </summary>
/// <remarks>Not named ESRespawnPointManagedComponent to avoid typos and confusion, even if that's more accurate.</remarks>
[RegisterComponent]
public sealed partial class ESRespawnPointOwnedComponent : Component
{
    public EntityUid? Manager;
}
