using Content.Server._ES.RespawnPoint.Components;
using Robust.Shared.Utility;

namespace Content.Server._ES.RespawnPoint.Systems;

public sealed partial class ESRespawnPointSystem
{
    /// <summary>
    ///     Assigns a respawn point to a manager.
    /// </summary>
    private void UnionPointWithManager(Entity<ESRespawnPointComponent> point,
        Entity<ESRespawnPointManagerComponent> manager)
    {
        manager.Comp.AttachedRespawnPoints.Add(point);
        point.Comp.Manager = manager;
    }

    /// <summary>
    ///     Assigns an arbitrary entity to a manager, as something it spawned.
    /// </summary>
    /// <param name="ent"></param>
    private void UnionOwnedWithManager(EntityUid ent, Entity<ESRespawnPointManagerComponent> manager)
    {
        var owned = AddComp<ESRespawnPointOwnedComponent>(ent);

        owned.Manager = manager;
        manager.Comp.AttachedEntities.Add(ent);
    }

    /// <summary>
    ///     Removes an arbitrary entity from a manager, as something it spawned.
    /// </summary>
    /// <remarks>
    ///     This does not handle respawn points, only things the respawn points created.
    /// </remarks>
    private void BreakOwnedFromManager(Entity<ESRespawnPointOwnedComponent> ent,
        Entity<ESRespawnPointManagerComponent> manager)
    {
        var success = manager.Comp.AttachedEntities.Remove(ent);
        DebugTools.Assert(success,
            "Removing an entity from a manager should never be called with the wrong manager or entity.");

        ent.Comp.Manager = null;
    }

    /// <summary>
    ///     Removes a respawn point from a manager.
    /// </summary>
    /// <remarks>
    ///     The respawn point should be destroyed soon after this is called.
    /// </remarks>
    private void BreakPointFromManager(Entity<ESRespawnPointComponent> point,
        Entity<ESRespawnPointManagerComponent> manager)
    {
        var success = manager.Comp.AttachedRespawnPoints.Remove(point);
        DebugTools.Assert(success,
            "Removing a respawn point from a manager should never be called with the wrong manager or point.");

        point.Comp.Manager = null;
    }
}
