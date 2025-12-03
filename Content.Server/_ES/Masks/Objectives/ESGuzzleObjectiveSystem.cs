using Content.Server._ES.Masks.Objectives.Components;
using Content.Server._ES.Masks.Objectives.Relays;
using Content.Server._ES.Masks.Objectives.Relays.Components;
using Content.Shared.Objectives.Components;
using Robust.Shared.Random;

namespace Content.Server._ES.Masks.Objectives;

/// <summary>
///     This handles a particular kind of objective that requires imbibing X amount of reagents, total.
/// </summary>
/// <seealso cref="ESGuzzleObjectiveComponent"/>
public sealed class ESGuzzleObjectiveSystem : ESBaseObjectiveSystem<ESGuzzleObjectiveComponent>
{
    public override Type[] RelayComponents => new[] { typeof(ESMuncherRelayComponent) };

    /// <inheritdoc/>
    public override void Initialize()
    {
        base.Initialize();

        SubscribeLocalEvent<ESGuzzleObjectiveComponent, BodyIngestingEvent>(OnBodyIngesting);
    }

    protected override void GetObjectiveProgress(Entity<ESGuzzleObjectiveComponent> ent, ref ObjectiveGetProgressEvent args)
    {
        var target = NumberObjectivesSys.GetTarget(ent);

        args.Progress = Math.Clamp(ent.Comp.ReagentsConsumed.Float() / target, 0, 1);
    }

    private void OnBodyIngesting(Entity<ESGuzzleObjectiveComponent> ent, ref BodyIngestingEvent args)
    {
        if (!args.IsDrink)
            return; // We're NOT guzzling.

        // Tally our guzzling.
        ent.Comp.ReagentsConsumed += args.FoodSolution.Volume;
    }
}
