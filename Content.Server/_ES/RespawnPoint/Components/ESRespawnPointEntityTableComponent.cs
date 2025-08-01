using Content.Shared.EntityTable;
using Content.Shared.EntityTable.EntitySelectors;
using Robust.Shared.Prototypes;

namespace Content.Server._ES.RespawnPoint.Components;

/// <summary>
/// This contains a simple, no strings attached "use an entity table" pool for all respawn points for a manager.
/// </summary>
[RegisterComponent]
public sealed partial class ESRespawnPointEntityTableComponent : Component
{
    [DataField(required: true)]
    public EntityTableSelector EntityTable = new NoneSelector();
}
