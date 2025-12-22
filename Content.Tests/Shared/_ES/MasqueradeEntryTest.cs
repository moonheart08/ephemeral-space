using System.Collections.Generic;
using Content.Shared._ES.Masks;
using Content.Shared._ES.Masks.Masquerades;
using NUnit.Framework;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization.Markdown.Value;

namespace Content.Tests.Shared._ES;

[TestFixture]
public sealed class MasqueradeEntryTest
{
    [Test]
    [TestCase("Mask1")]
    [TestCase("-Mask1")]
    [TestCase("Mask1/Mask2")]
    [TestCase("-Mask1/Mask2")]
    [TestCase("Mask1(3)")]
    [TestCase("-Mask1(3)")]
    [TestCase("Mask1/Mask2(3)")]
    [TestCase("-Mask1/Mask2(3)")]
    [TestCase("-Mask1/Mask2/Mask3/Mask4/Mask5(1)")]
    public void ParseEntry(string entry)
    {
        // mild jank, only ever call with validation off, or it'll be sad about the lack of prototype manager.
        var parser = new UnresolvedMasqueradeEntrySerializer();

        Assert.That(parser.TryRead(new ValueDataNode(entry), false, out var entryParsed, out _));

        Assert.Multiple(() =>
        {
            Assert.That(entryParsed!.Count, Is.GreaterThan(0));
            Assert.That(entryParsed!.Masks, Is.Not.Empty);
            Assert.That(entryParsed!.Subtract, Is.EqualTo(entry.StartsWith('-')));
        });
    }
}
