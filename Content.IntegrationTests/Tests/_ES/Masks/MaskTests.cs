using Content.IntegrationTests.Tests._Citadel;
using Content.IntegrationTests.Tests._Citadel.Constraints;
using Content.Server._ES.Masks;
using Content.Server.Mind;
using Content.Shared._ES.Masks;
using Content.Shared._ES.Masks.Components;
using Content.Shared.Mind;
using Robust.Shared.GameObjects;
using Robust.Shared.Prototypes;

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

    public override TestMapMode TestMapSetting => TestMapMode.Arena;

    [Test]
    [TestCaseSource(nameof(Masks))]
    [Description("Assigns each mask alone with no other players.")]
    public async Task AssignMaskAlone(string maskProto)
    {
        var player = await TestPlayer.CreatePlayer(this, playerProto: "MobHuman");

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

    // Very strong, suitable for extreme violence.
    private static readonly EntProtoId Weapon = "MeleeDebug200";

    [Test]
    [TestCaseSource(nameof(Masks))]
    [Description("Has the given mask beat up a crew member, asserting it doesn't fail.")]
    public async Task BeatUpCrewmember(string maskProto)
    {
        var deviant = await TestPlayer.CreatePlayer(this, playerProto: "MobHuman");

        var targetSession = await Server.AddDummySession();

        var target = await AssignPlayerBody(targetSession, playerPrototype: "MobHuman");

        await Server.WaitPost(() => { deviant.SSetMask(maskProto); });

        // Grant them the Power.
        await deviant.SpawnAndPickUp(Weapon);

        // Be violent. Really violent.
        await deviant.Punch(target, waitOutCooldown: true);
        if (!SDeleted(deviant.SEntity) && !SDeleted(target))
            await deviant.Punch(target, waitOutCooldown: true);
        if (!SDeleted(deviant.SEntity) && !SDeleted(target))
            await deviant.Punch(target, waitOutCooldown: true);
        if (!SDeleted(deviant.SEntity) && !SDeleted(target))
            await deviant.Punch(target, waitOutCooldown: true);
        if (!SDeleted(deviant.SEntity) && !SDeleted(target))
            await deviant.Punch(target, waitOutCooldown: true);

        // Few seconds for stuff to settle.
        await RunSeconds(5);
    }
}
