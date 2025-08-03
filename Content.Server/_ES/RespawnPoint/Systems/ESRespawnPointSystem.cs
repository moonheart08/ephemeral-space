using Content.Server._ES.RespawnPoint.Components;
using Robust.Shared.Map;
using Robust.Shared.Utility;

namespace Content.Server._ES.RespawnPoint.Systems;

/// <summary>
///     Manages respawn points and their managers.
/// </summary>
public sealed partial class ESRespawnPointSystem : EntitySystem
{
    [Dependency] private readonly IMapManager _mapManager = default!;

    private EntityQuery<ESRespawnPointOwnedComponent> OwnedQuery;
    private EntityQuery<ESRespawnPointComponent> RespawnPointQuery;
    private EntityQuery<ESRespawnPointManagerComponent> ManagerQuery;

    public override void Initialize()
    {
        base.Initialize();

        RespawnPointQuery = GetEntityQuery<ESRespawnPointComponent>();
        ManagerQuery = GetEntityQuery<ESRespawnPointManagerComponent>();
        OwnedQuery = GetEntityQuery<ESRespawnPointOwnedComponent>();

        SubscribeLocalEvent<ESRespawnPointComponent, ESRespawnPointSingularManager>(OnSingularManagerLookup);
        SubscribeLocalEvent<ESRespawnPointComponent, ESRespawnPointGridWideManager>(OnGridWideManagerLookup);
        SubscribeLocalEvent<ESRespawnPointComponent, ComponentShutdown>(OnRespawnPointShutdown);
        SubscribeLocalEvent<ESRespawnPointOwnedComponent, ComponentShutdown>(OnOwnedShutdown);
        SubscribeLocalEvent<ESRespawnPointManagerComponent, ComponentShutdown>(OnManagerShutdown);
        SubscribeLocalEvent<ESRespawnPointComponent, MapInitEvent>(OnRespawnPointMapInit);
    }

    private void OnManagerShutdown(Entity<ESRespawnPointManagerComponent> manager, ref ComponentShutdown args)
    {
        foreach (var point in manager.Comp.AttachedRespawnPoints)
        {
            BreakPointFromManager(RespawnPointQuery.Get(point), manager);
        }

        foreach (var owned in manager.Comp.AttachedEntities)
        {
            BreakOwnedFromManager(OwnedQuery.Get(owned), manager);
        }
    }

    private void OnOwnedShutdown(Entity<ESRespawnPointOwnedComponent> ent, ref ComponentShutdown args)
    {
        if (ent.Comp.Manager is {} manager)
            BreakOwnedFromManager(ent, ManagerQuery.Get(manager));
    }

    private void OnRespawnPointMapInit(Entity<ESRespawnPointComponent> ent, ref MapInitEvent args)
    {
        var ev = ent.Comp.ManagerLocator with { }; // Shallow clone it using with syntax.
        RaiseLocalEvent((object)ev); // and raise it dynamically. This'll automatically find the correct, strongly typed event handler.

        if (ev.Manager is not { } manager)
        {
            Log.Error($"Failed to initialize respawn point {ent}, manager not found.");
            return;
        }

        UnionPointWithManager(ent, ManagerQuery.Get(manager));
    }

    private void OnRespawnPointShutdown(Entity<ESRespawnPointComponent> ent, ref ComponentShutdown args)
    {
        if (ent.Comp.Manager is {} manager)
            BreakPointFromManager(ent, ManagerQuery.Get(manager));
    }

    private void OnSingularManagerLookup(Entity<ESRespawnPointComponent> ent, ref ESRespawnPointSingularManager args)
    {
        var manager = Spawn(args.ManagerPrototype, MapCoordinates.Nullspace);

        UnionPointWithManager(ent, ManagerQuery.Get(manager));
    }

    private void OnGridWideManagerLookup(Entity<ESRespawnPointComponent> ent, ref ESRespawnPointGridWideManager args)
    {
        var xform = Transform(ent);

        if (xform.GridUid is not { } grid)
        {
            // no grid wide manager if we're not on a grid, bail.
            return;
        }

        var directory = EnsureComp<ESRespawnPointGridManagerDirectoryComponent>(grid);

        if (directory.Managers.TryGetValue(args.ManagerPrototype, out var manager))
        {
            UnionPointWithManager(ent, ManagerQuery.Get(manager));
        }
        else
        {
            manager = Spawn(args.ManagerPrototype, MapCoordinates.Nullspace);

            directory.Managers[args.ManagerPrototype] = manager;

            UnionPointWithManager(ent, ManagerQuery.Get(manager));
        }
    }

    public override void Update(float frameTime)
    {
        base.Update(frameTime);
    }
}
