using Robust.Shared.Prototypes;

namespace Content.Server._ES.RespawnPoint.Components;

/// <summary>
///     Contains a directory of respawn point managers for the given grid.
/// </summary>
[RegisterComponent]
public sealed partial class ESRespawnPointGridManagerDirectoryComponent : Component
{
    /// <summary>
    ///     The manager proto id to manager entity mapping.
    /// </summary>
    [DataField]
    public Dictionary<EntProtoId, EntityUid> Managers = new();
}
