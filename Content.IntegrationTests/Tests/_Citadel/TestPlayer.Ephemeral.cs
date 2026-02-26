using Content.Server._ES.Masks;
using Content.Shared._ES.Masks;
using Robust.Shared.Prototypes;

namespace Content.IntegrationTests.Tests._Citadel;

public sealed partial class TestPlayer
{
    /// <summary>
    ///     Sets the player's mask on the server.
    /// </summary>
    public void SSetMask(ProtoId<ESMaskPrototype> mask)
    {
        AssertServer();

        var maskSys = Test.Server.System<ESMaskSystem>();

        maskSys.ApplyMask(SMindEntity, mask);
    }

    public ProtoId<ESMaskPrototype>? SGetMask()
    {
        AssertServer();

        var maskSys = Test.Server.System<ESMaskSystem>();

        return maskSys.GetMaskOrNull(SMindEntity);
    }
}
