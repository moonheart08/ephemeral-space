using Content.IntegrationTests.Tests._Citadel;
using Content.IntegrationTests.Tests._Citadel.Constraints;
using Content.Server._ES.Masks;
using Content.Server.Mind;
using Content.Shared._ES.Masks;
using Content.Shared._ES.Masks.Components;
using Content.Shared.Mind;
using Robust.Shared.GameObjects;

namespace Content.IntegrationTests.Tests._ES.Masks;

[TestFixture]
public sealed class MaskTests : GameTest
{
    public override PoolSettings PoolSettings { get; } = new()
    {
        Dirty = true,
        Connected = true, // We need a guy to mask up.
    };

    public static readonly string[] Masks = PrototypeDataScrounger.PrototypesOfKind<ESMaskPrototype>();

    public override bool AutoCreateTestMap => true;

    [Test]
    [TestCaseSource(nameof(Masks))]
    [Description("Assigns each mask alone with no other players.")]
    public async Task AssignMaskAlone(string maskProto)
    {
        var player = await TestPlayer.CreatePlayer(this, Client, playerProto: "MobHuman");

        await Server.WaitAssertion(() =>
        {
            player.SSetMask(maskProto);

            var mask = player.SGetMask();

            using (Assert.EnterMultipleScope())
            {
                Assert.That(mask, Is.EqualTo(maskProto));

                // Verify a side effect: the mask role entity exists.
                Assert.That(SQueryCount<ESMaskRoleComponent>(), Is.EqualTo(1));
            }
        });
    }
}
