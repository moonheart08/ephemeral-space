using System.Collections.Generic;
using Content.IntegrationTests.Tests._Citadel;
using Content.Server._ES.Masks.Masquerades;
using Content.Server.GameTicking;
using Content.Shared._Citadel.Utilities;
using Content.Shared._ES.Masks;
using Content.Shared._ES.Masks.Masquerades;
using Robust.Shared.GameObjects;
using Robust.Shared.Prototypes;
using Robust.Shared.Random;
using Robust.Shared.Serialization.Manager;

namespace Content.IntegrationTests.Tests._ES.Masquerades;

[TestFixture]
public sealed class MasqueradeTests
{
    [GameTest(RunOnSide = Side.Server)]
    public void TestMasqueradeSerialization(
        [SidedDependency(Side.Server)] ISerializationManager ser,
        [SidedDependency(Side.Server)] IPrototypeManager proto
        )
    {
        // We don't support this yet, so whichever shmuck adds it is gonna need to write tests.
        // My advice is to do a round-trip test where you deserialize entries, then reserialize them and verify they're string equal.
        Assert.Throws<NotImplementedException>(() =>
        {
            ser.WriteValue<MasqueradeEntry>(
                new MasqueradeEntry.DirectEntry(new HashSet<ProtoId<ESMaskPrototype>>() { "Foo", "Bar" }, 1, false));
        });
    }

    [GameTest(RunOnSide = Side.Server, Description = "Ensures that selecting masks from MasqueradeEntry is deterministic.")]
    public void MasqueradeEntryDeterminism(
        [SidedDependency(Side.Server)] IPrototypeManager proto,
        [SidedDependency(Side.Server)] IRobustRandom globalRng
    )
    {
        void TestOnEntry(MasqueradeEntry entry)
        {
            // This can in theory flake so let's try a few times.
            for (var i = 0; i < 100; i++)
            {
                var rngSeed = new RngSeed(globalRng);
                var rng1 = rngSeed.IntoRandomizer();
                var rng2 = rngSeed.IntoRandomizer();

                var masks1 = entry!.PickMasks(rng1, proto);
                var masks2 = entry!.PickMasks(rng2, proto);

                Assert.That(masks1, Is.EqualTo(masks2), $"Expected two calls from the same seed to be identical, and they weren't. Seed is {rngSeed.ToString()}");
            }
        }

        {
            MasqueradeEntry.TryRead("Foo/Bar/Baz", null, out var entry, out var error);

            Assert.That(error, Is.Null);

            TestOnEntry(entry);
        }

        {
            MasqueradeEntry.TryRead("#Freaks", null, out var entry, out var error);

            Assert.That(error, Is.Null);

            TestOnEntry(entry);
        }
    }

    [GameTest(RunOnSide = Side.Server, Description = "Ensures that masquerade mask selection itself is deterministic.")]
    public void MasqueradeDeterminism(
        [SidedDependency(Side.Server)] IPrototypeManager proto,
        [SidedDependency(Side.Server)] IRobustRandom globalRng
    )
    {
        #pragma warning disable RA0033
        var freakshow = proto.Index<ESMasqueradePrototype>("Freakshow");
        #pragma warning restore RA0033

        for (var i = 0; i < 100; i++)
        {
            var rngSeed = new RngSeed(globalRng);
            var rng1 = rngSeed.IntoRandomizer();
            var rng2 = rngSeed.IntoRandomizer();

            var masquerade = (MasqueradeRoleSet)freakshow.Masquerade;

            Assert.That(masquerade.TryGetMasks(30, rng1, proto, out var masks1));
            Assert.That(masquerade.TryGetMasks(30, rng2, proto, out var masks2));

            Assert.That(masks1!, Is.EquivalentTo(masks2!));
        }
    }

    public sealed class MasqueradeTestData : GameTestData
    {
        // We need to get to run a mode.
        public override PoolSettings PoolSettings { get; } = new()
        {
            Dirty = true,
            DummyTicker = false,
            Connected = true,
            InLobby = true,
        };

        [System(Side.Server)] public readonly GameTicker SGameticker = default!;
    }

    [GameTest<MasqueradeTestData>]
    public async Task MasqueradeRuns(MasqueradeTestData data)
    {
        await data.Server.AddDummySessions(10); // A smattering of people.

        await data.Server.WaitAssertion(() =>
        {
            // Force a masquerade.
            data.SGameticker.SetGamePreset("ESMasquerade", true);

            // Ready everyone up.
            data.SGameticker.ToggleReadyAll(true);

            // Start the round.
            data.SGameticker.StartRound();

            Assert.That(data.SQuerySingle(out Entity<ESMasqueradeRuleComponent>? rule), "Masquerade didn't start correctly, no rule was found.");


        });
    }
}
