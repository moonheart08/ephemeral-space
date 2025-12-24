using Content.Server._ES.Masks.Masquerades;
using Content.Server.GameTicking.Rules;
using Content.Shared._ES.Masks;
using Content.Shared.GameTicking.Components;
using Robust.Shared.Prototypes;

namespace Content.Server._ES.Masks;

/// <summary>
/// This handles masquerade management and how they influence game flow.
/// </summary>
public sealed class ESMasuqeradeSystem : GameRuleSystem<MasqueradeRuleComponent>
{
    [Dependency] private readonly IPrototypeManager _proto = default!;

    // Icky globla state.
    private ProtoId<ESMasqueradePrototype> ActiveMasquerade = "Random";

    private bool Forced = false;

    /// <inheritdoc/>
    public override void Initialize()
    {
        base.Initialize();
    }

    protected override void Added(EntityUid uid, MasqueradeRuleComponent component, GameRuleComponent gameRule, GameRuleAddedEvent args)
    {
        // When we get added, add more rules because we can be re-entrant here.


    }
}
