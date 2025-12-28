using System.Linq;
using Content.Server.GameTicking;

namespace Content.Server._ES.Masks.Masquerades;

public sealed partial class ESMasqueradeSystem
{
    [Dependency] private readonly ILocalizationManager _loc = default!;

    private void OnRoundEndTextAppend(RoundEndTextAppendEvent ev)
    {
        var rule = EntityQuery<ESMasqueradeRuleComponent>().SingleOrDefault();

        if (rule?.Masquerade is not {} masquerade)
            return;

        ev.AddLine(
            Loc.GetString(
                "es-roundend-masquerade-reveal",
                ("masquerade", masquerade.LocName(_loc)),
                ("description", masquerade.LocDescription(_loc))
                )
            );


    }
}
