using Content.Server._ES.Masks.Objectives.Components;
using Content.Server._ES.Masks.Objectives.Relays;
using Content.Server._ES.Masks.Objectives.Relays.Components;
using Content.Shared.Objectives.Components;

namespace Content.Server._ES.Masks.Objectives;

/// <summary>
///     This handles the imbibe unique reagents objective, for consuming N reagents.
/// </summary>
/// <seealso cref="ESImbibeUniqueReagentsObjectiveComponent"/>
public sealed class ESImbibeUniqueReagentsObjectiveSystem : ESBaseObjectiveSystem<ESImbibeUniqueReagentsObjectiveComponent>
{
    public override Type[] RelayComponents => new[] { typeof(ESMuncherRelayComponent) };

    /// <inheritdoc/>
    public override void Initialize()
    {
        base.Initialize();

        SubscribeLocalEvent<ESImbibeUniqueReagentsObjectiveComponent, BodyIngestingEvent>(OnBodyIngesting);
    }

    private void OnBodyIngesting(Entity<ESImbibeUniqueReagentsObjectiveComponent> ent, ref BodyIngestingEvent args)
    {
        if (!ent.Comp.CanBeFromFood && !args.IsDrink)
            return;

        foreach (var reagent in args.FoodSolution)
        {
            ent.Comp.SeenReagents.Add(reagent.Reagent);
        }
    }

    protected override void GetObjectiveProgress(Entity<ESImbibeUniqueReagentsObjectiveComponent> ent, ref ObjectiveGetProgressEvent args)
    {
        var target = NumberObjectivesSys.GetTarget(ent);

        args.Progress = Math.Clamp(((float)ent.Comp.SeenReagents.Count) / (float)target, 0, 1);
    }
}
