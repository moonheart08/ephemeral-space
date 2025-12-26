using Content.Server.GameTicking;
using Content.Server.GameTicking.Presets;
using Content.Shared._Citadel.Utilities;
using Content.Shared._ES.Masks;
using Content.Shared._ES.Masks.Masquerades;
using Robust.Shared.Prototypes;
using Robust.Shared.Random;
using Robust.Shared.Toolshed;

namespace Content.Server._ES.Masks.Masquerades;

[ToolshedCommand(Name = "mq")]
public sealed class MasqueradeCommands : ToolshedCommand
{
    [Dependency] private readonly IPrototypeManager _proto = default!;

    public static readonly ProtoId<GamePresetPrototype> MasqueradePreset = "ESMasqueradeManaged";

    [CommandImplementation]
    public ProtoId<ESMaskPrototype> PickFromMaskSet([CommandArgument] ProtoId<ESMaskSetPrototype> maskSet,
        [CommandArgument] RngSeed seed)
    {
        var rng = seed.IntoRandomizer();

        var set = _proto.Index(maskSet);

        return set.Pick(rng);
    }

    [CommandImplementation]
    public void ForceMasquerade([CommandArgument] ProtoId<ESMasqueradePrototype> masquerade)
    {
        var mqSys = Sys<ESMasqueradeSystem>();
        var gameTicker = Sys<GameTicker>();

        mqSys.ForceMasquerade(masquerade);
        gameTicker.SetGamePreset(MasqueradePreset, true);
    }

    // exists due to toolshed and C# limitations around nulls.
    [CommandImplementation]
    public void UnforceMasquerade()
    {
        var mqSys = Sys<ESMasqueradeSystem>();

        mqSys.ForceMasquerade(null);
    }
}
