using System.Linq;
using Content.Server._ES.Masks.Masquerades;
using Content.Server.GameTicking;
using Content.Server.GameTicking.Rules;
using Content.Shared._ES.Masks;
using Content.Shared.GameTicking.Components;
using Content.Shared.Random.Helpers;
using Robust.Shared.Prototypes;
using Robust.Shared.Random;

namespace Content.Server._ES.Masks;

/// <summary>
/// This handles masquerade management and how they influence game flow.
/// </summary>
public sealed class ESMasuqeradeSystem : GameRuleSystem<ESMasqueradeRuleComponent>
{
    [Dependency] private readonly IPrototypeManager _proto = default!;
    [Dependency] private readonly IRobustRandom _random = default!;
    [Dependency] private readonly GameTicker _gameTicker = default!;

    // Icky globla state.
    private ProtoId<ESMasqueradePrototype>? _forcedMasquerade = null;

    /// <inheritdoc/>
    public override void Initialize()
    {
        base.Initialize();
    }

    protected override void Added(EntityUid uid, ESMasqueradeRuleComponent component, GameRuleComponent gameRule, GameRuleAddedEvent args)
    {
        // When we get added, add more rules because we can be re-entrant here.


    }

    private ESMasqueradePrototype SelectMasquerade()
    {
        if (_forcedMasquerade is { } forced)
        {
            return _proto.Index(forced);
        }
        else
        {
            var weighted = _proto.EnumeratePrototypes<ESMasqueradePrototype>()
                .Where(x => x.Weight is not null)
                .Where(x => x.Masquerade.MinPlayers >= _gameTicker.ReadyPlayerCount() && x.Masquerade.MaxPlayers <= _gameTicker.ReadyPlayerCount())
                .ToDictionary(x => x, x => (float)x.Weight!.Value);

            return _random.Pick(weighted);
        }
    }
}
