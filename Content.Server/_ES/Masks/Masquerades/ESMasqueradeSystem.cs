using System.Linq;
using Content.Server._ES.Masks.Masquerades;
using Content.Server.Chat.Managers;
using Content.Server.GameTicking;
using Content.Server.GameTicking.Rules;
using Content.Shared._Citadel.Utilities;
using Content.Shared._ES.Masks;
using Content.Shared.GameTicking.Components;
using Content.Shared.Random.Helpers;
using Robust.Shared.Prototypes;
using Robust.Shared.Random;

namespace Content.Server._ES.Masks.Masquerades;

/// <summary>
///     This handles masquerade management and how they influence game flow.
/// </summary>
public sealed class ESMasqueradeSystem : GameRuleSystem<ESMasqueradeRuleComponent>
{
    [Dependency] private readonly IChatManager _chat = default!;
    [Dependency] private readonly IPrototypeManager _proto = default!;
    [Dependency] private readonly IRobustRandom _random = default!;
    [Dependency] private readonly GameTicker _gameTicker = default!;


    // Icky globla state.
    private ProtoId<ESMasqueradePrototype>? _forcedMasquerade = null;

    /// <inheritdoc/>
    public override void Initialize()
    {
        base.Initialize();

        SubscribeLocalEvent<AssignLatejoinerToTroupeEvent>(OnAssignLatejoiner);
    }

    private void OnAssignLatejoiner(ref AssignLatejoinerToTroupeEvent ev)
    {
        var rule = EntityQuery<ESMasqueradeRuleComponent>().SingleOrDefault();

        if (rule is null)
            return;

        var mask = rule.Masquerade!.Masquerade.DefaultMask.PickMasks(rule.Rng, _proto).Single();


    }

    /// <summary>
    ///     Force the given masquerade, or clear it if null.
    /// </summary>
    /// <param name="proto"></param>
    public void ForceMasquerade(ProtoId<ESMasqueradePrototype>? proto)
    {
        _forcedMasquerade = proto;
    }

    protected override void Started(EntityUid uid, ESMasqueradeRuleComponent component, GameRuleComponent gameRule, GameRuleStartedEvent args)
    {
        component.Seed = new RngSeed(_random);
        component.Rng = component.Seed.IntoRandomizer();
        component.Masquerade = SelectMasquerade(_gameTicker.ReadyPlayerCount());

        _chat.SendAdminAlert($"Upcoming masquerade is {component.Masquerade.ID}.");

        foreach (var rule in component.Masquerade.GameRules)
        {
            _gameTicker.StartGameRule(rule);
        }
    }

    private ESMasqueradePrototype SelectMasquerade(int players)
    {
        if (_forcedMasquerade is { } forced)
        {
            return _proto.Index(forced);
        }
        else
        {
            var weighted = _proto.EnumeratePrototypes<ESMasqueradePrototype>()
                .Where(x => x.Weight is not null)
                .Where(x => x.Masquerade.MinPlayers >= players && x.Masquerade.MaxPlayers <= players)
                .ToDictionary(x => x, x => (float)x.Weight!.Value);

            return _random.Pick(weighted);
        }
    }
}
