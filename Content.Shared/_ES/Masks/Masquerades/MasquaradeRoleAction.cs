using System.Diagnostics.CodeAnalysis;
using System.Text.RegularExpressions;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Markdown;
using Robust.Shared.Serialization.Markdown.Mapping;
using Robust.Shared.Serialization.Markdown.Validation;
using Robust.Shared.Serialization.Markdown.Value;
using Robust.Shared.Serialization.TypeSerializers.Interfaces;

namespace Content.Shared._ES.Masks.Masquerades;

public sealed class MasqueradeRoleSet
{
    /// <summary>
    ///     All the roles in this masquerade at given population levels, baked into something easy to use by the game.
    /// </summary>
    private List<List<MasqueradeEntry>> _bakedRoles = new();
}

/// <summary>
///     A set of masks that can be randomly picked between.
/// </summary>
public sealed record MasqueradeEntry(IReadOnlySet<ProtoId<ESMaskPrototype>> Masks, int Count);

public sealed record UnresolvedMasqueradeEntry(IReadOnlySet<ProtoId<ESMaskPrototype>> Masks, int Count, bool Subtract);

[TypeSerializer]
public sealed class UnresolvedMasqueradeEntrySerializer : ITypeSerializer<UnresolvedMasqueradeEntry, ValueDataNode>
{
    [Dependency] private readonly IPrototypeManager _proto = default!;

    // Magic regex to match everything in an unresolved entry.
    // Not GeneratedRegex because I don't think that works on client.
    private Regex _entryRegex = new(@"^(?'subtractive'-)?(?'maskn'[a-zA-Z0-9]+)(/(?'maskn'[a-zA-Z0-9]+))*(\((?'count'[0-9]*)\))?$",
        RegexOptions.Compiled | RegexOptions.IgnoreCase | RegexOptions.ExplicitCapture);

    public ValidationNode Validate(ISerializationManager serializationManager,
        ValueDataNode node,
        IDependencyCollection dependencies,
        ISerializationContext? context = null)
    {
        if (TryRead(node, true, out _, out var error))
            return new ValidatedValueNode(node);
        else
            return new ErrorNode(node, error);
    }

    public bool TryRead(ValueDataNode node, bool validate, [NotNullWhen(true)] out UnresolvedMasqueradeEntry? action, [NotNullWhen(false)] out string? error)
    {
        var match = _entryRegex.Match(node.Value);

        if (!match.Success)
        {
            action = null;
            error = "Entry didn't match syntax, check for typos?";
            return false;
        }

        var masks = new HashSet<ProtoId<ESMaskPrototype>>();

        foreach (Capture entry in match.Groups["maskn"].Captures)
        {
            if (validate && !_proto.TryIndex<ESMaskPrototype>(entry.Value, out _))
            {
                action = null;
                error = $"Mask {entry.Value} isn't a valid mask.";
                return false;
            }

            masks.Add(new(entry.Value));
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

        error = null;
        action = new(masks, count, subtractive);
        return true;
    }

    public UnresolvedMasqueradeEntry Read(ISerializationManager serializationManager,
        ValueDataNode node,
        IDependencyCollection dependencies,
        SerializationHookContext hookCtx,
        ISerializationContext? context = null,
        ISerializationManager.InstantiationDelegate<UnresolvedMasqueradeEntry>? instanceProvider = null)
    {
        if (!TryRead(node, false, out var entry, out var error))
        {
            throw new Exception(error);
        }

        return entry;
    }

    public DataNode Write(ISerializationManager serializationManager,
        UnresolvedMasqueradeEntry value,
        IDependencyCollection dependencies,
        bool alwaysWrite = false,
        ISerializationContext? context = null)
    {
        throw new NotImplementedException("Masquerade role sets are not serializable at this time.");
    }
}

[TypeSerializer]
public sealed class MasqueradeRoleSetSerializer : ITypeSerializer<MasqueradeRoleSet, MappingDataNode>
{
    [Dependency] private readonly IPrototypeManager _proto = default!;

    public ValidationNode Validate(ISerializationManager serializationManager,
        MappingDataNode node,
        IDependencyCollection dependencies,
        ISerializationContext? context = null)
    {
        throw new NotImplementedException();
    }

    private bool TryRead(MappingDataNode node, out MasqueradeRoleSet? action)
    {
        throw new NotImplementedException();
    }

    public MasqueradeRoleSet Read(ISerializationManager serializationManager,
        MappingDataNode node,
        IDependencyCollection dependencies,
        SerializationHookContext hookCtx,
        ISerializationContext? context = null,
        ISerializationManager.InstantiationDelegate<MasqueradeRoleSet>? instanceProvider = null)
    {
        throw new NotImplementedException();
    }

    public DataNode Write(ISerializationManager serializationManager,
        MasqueradeRoleSet value,
        IDependencyCollection dependencies,
        bool alwaysWrite = false,
        ISerializationContext? context = null)
    {
        throw new NotImplementedException("Masquerade role sets are not serializable at this time.");
    }
}
