using Content.Server._ES.Masks.Objectives.Components;
using Content.Server.KillTracking;
using Content.Server.Mind;
using Content.Server.Objectives.Systems;
using Content.Shared.Objectives.Components;

namespace Content.Server._ES.Masks.Objectives;

/// <summary>
///     This handles the kill troupe objective.
/// </summary>
/// <seealso cref="ESKillTroupeObjectiveComponent"/>
public sealed class ESKillTroupeObjectiveSystem : ESBaseObjectiveSystem<ESKillTroupeObjectiveComponent>
{
    [Dependency] private readonly ESMaskSystem _mask = default!;

    /// <inheritdoc/>
    public override void Initialize()
    {
        base.Initialize();

        SubscribeLocalEvent<KillReportedEvent>(OnKillReported);
    }

    protected override void GetObjectiveProgress(Entity<ESKillTroupeObjectiveComponent> ent, ref ObjectiveGetProgressEvent args)
    {
        var target = NumberObjectivesSys.GetTarget(ent);
        if (target == 0)
            return;
        args.Progress = Math.Clamp((float) ent.Comp.Kills / target, 0, 1);
    }

    private void OnKillReported(ref KillReportedEvent args)
    {
        if (args.Primary is not KillPlayerSource source ||
            !MindSys.TryGetMind(source.PlayerId, out var mind))
            return;

        foreach (var objective in MindSys.ESGetObjectivesComp<ESKillTroupeObjectiveComponent>(mind.Value.AsNullable()))
        {
            if (!_mask.TryGetTroupe(args.Entity, out var troupe))
                return;

            if ((troupe == objective.Comp.Troupe) ^ objective.Comp.Invert)
                objective.Comp.Kills += 1;
        }
    }
}
