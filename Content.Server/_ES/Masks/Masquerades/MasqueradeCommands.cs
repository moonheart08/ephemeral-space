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

    [CommandImplementation]
    public ProtoId<ESMaskPrototype> PickFromMaskSet([CommandArgument] ProtoId<ESMaskSetPrototype> maskSet,
        [CommandArgument] RngSeed seed)
    {
        var rng = seed.IntoRandomizer();

        var set = _proto.Index(maskSet);

        return set.Pick(rng);
    }
}
