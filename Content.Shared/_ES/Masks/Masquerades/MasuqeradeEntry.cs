using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text.RegularExpressions;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Markdown;
using Robust.Shared.Serialization.Markdown.Mapping;
using Robust.Shared.Serialization.Markdown.Validation;
using Robust.Shared.Serialization.Markdown.Value;
using Robust.Shared.Serialization.TypeSerializers.Interfaces;
using Robust.Shared.Utility;

namespace Content.Shared._ES.Masks.Masquerades;

[DataDefinition]
public sealed partial class MasqueradeRoleSet : MasqueradeKind
{
    /// <summary>
    ///     All the roles in this masquerade at given population levels, baked into something easy to use by the game.
    /// </summary>
    /// <remarks>
    ///     These will never be subtractive, all cases of that will be resolved before this is generated.
    /// </remarks>
    private List<HashSet<MasqueradeEntry>> _bakedRoles = new();

    public bool TryGetEntriesForPop(int playerCount, [NotNullWhen(true)] out IReadOnlySet<MasqueradeEntry>? entries)
    {
        var index = MinPlayers + playerCount;

        if (_bakedRoles.TryGetValue(index, out var entries2))
        {
            entries = entries2;
            return true;
        }

        entries = null;
        return false;
    }

    [DataField(readOnly: true, required: true)]
    public MasqueradeEntry DefaultMask = default!;

    [DataField("roles", readOnly: true, required: true)]
    private IReadOnlyDictionary<int, HashSet<MasqueradeEntry>> _roles
    {
        get => throw new NotImplementedException();
        set => Init(value); // set up the baked role lists.
    }

    internal void Init(IReadOnlyDictionary<int, HashSet<MasqueradeEntry>> mapping)
    {
        var rollingSet = new HashSet<MasqueradeEntry>();

        var minPlayers = mapping.Keys.Min();

        DebugTools.Assert(minPlayers > 0, "You can't have any roles without players.");
        DebugTools.Assert(minPlayers == MinPlayers, $"Minimum players should match the first specified set of entries (expected {MinPlayers}, found {minPlayers})");


        foreach (var popCount in mapping.Keys.Order())
        {

        }
    }
}

/// <summary>
///     An entry in a <see cref="MasqueradeRoleSet"/>, describing a set of masks, how many times to repeat them,
///     and whether this entry should cancel out some earlier entry.
/// </summary>
/// <remarks>
///     Subtract looks at the value of the entry itself and removes entries that way, it does not attempt to
///     randomly pick masks, it's for removing entries entirely.
///     Subtract is not factored in when comparing entries for equality.
/// </remarks>
public abstract record MasqueradeEntry(int Count, bool Subtract)
{
    /// <summary>
    ///     An entry containing a set of unweighted masks and how many times to repeat them.
    /// </summary>
    public sealed record DirectEntry(IReadOnlySet<ProtoId<ESMaskPrototype>> Masks, int Count, bool Subtract)
        : MasqueradeEntry(Count, Subtract)
    {
        public bool Equals(DirectEntry? other)
        {
            if (other is null)
                return false;
            if (ReferenceEquals(this, other))
                return true;
            return Masks.Equals(other.Masks);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Masks, 271821); // Nothing up my sleeve, just avoiding hash collisions.
        }
    }

    /// <summary>
    ///     An entry pointing to a mask set prototype, and how many times to repeat it.
    /// </summary>
    public sealed record SetEntry(ProtoId<ESMaskSetPrototype> MaskSet, int Count, bool Subtract)
        : MasqueradeEntry(Count, Subtract)
    {
        public bool Equals(SetEntry? other)
        {
            if (other is null)
                return false;
            if (ReferenceEquals(this, other))
                return true;

            return MaskSet.Equals(other.MaskSet);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(MaskSet, 314159); // Nothing up my sleeve, just avoiding hash collisions.
        }
    }

    // Magic regex to match everything in an unresolved entry.
    // Not GeneratedRegex because I don't think that works on client.
    private static Regex _entryRegex =
            new(
            @"^(?'subtractive'-)?(\#(?'maskset'[a-zA-Z0-9]+)|(?'maskn'[a-zA-Z0-9]+)(/(?'maskn'[a-zA-Z0-9]+))*)(\((?'count'[0-9]*)\))?$",
            RegexOptions.Compiled | RegexOptions.IgnoreCase | RegexOptions.ExplicitCapture
            );

    public static bool TryRead(ValueDataNode node, IPrototypeManager? proto, [NotNullWhen(true)] out MasqueradeEntry? action, [NotNullWhen(false)] out string? error)
    {
        var match = _entryRegex.Match(node.Value);

        if (!match.Success)
        {
            action = null;
            error = "Entry didn't match syntax, check for typos?";
            return false;
        }

        var count = 1;
        var subtractive = false;

        if (match.Groups["count"].Captures is [var capture])
        {
            count = int.Parse(capture.ValueSpan);
        }

        if (match.Groups["subtractive"].Length == 1)
        {
            subtractive = true;
        }

        if (match.Groups["maskn"].Length > 0)
        {
            var masks = new HashSet<ProtoId<ESMaskPrototype>>();

            foreach (Capture entry in match.Groups["maskn"].Captures)
            {
                var value = entry.Value; // This, despite being a property, allocates a new string every time.

                if ((!proto?.TryIndex<ESMaskPrototype>(value, out _)) ?? false)
                {
                    action = null;
                    error = $"Mask {value} isn't a valid mask.";
                    return false;
                }

                masks.Add(new(value));
            }

            error = null;
            action = new DirectEntry(masks, count, subtractive);
            return true;
        }
        else if (match.Groups["maskset"].Captures is [{Value: var maskSetCapture}])
        {
            if ((!proto?.TryIndex<ESMaskSetPrototype>(maskSetCapture, out _)) ?? false)
            {
                action = null;
                error = $"Maskset {maskSetCapture} isn't a valid maskset.";
                return false;
            }

            error = null;
            action = new SetEntry(maskSetCapture, count, subtractive);
            return true;
        }
        else
        {
            throw new NotImplementedException();
        }
    }
}
