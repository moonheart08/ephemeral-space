using Robust.Shared.Prototypes;
using Robust.Shared.Random;
using Robust.Shared.Serialization.TypeSerializers.Implementations.Custom.Prototype.Array;

namespace Content.Shared._ES.Masks.Masquerades;

/// <summary>
///     A weighted collection of masks for use by Masquerades.
/// </summary>
/// <seealso cref="MasqueradeEntry"/>
[Prototype]
public sealed class ESMaskSetPrototype : IPrototype, IInheritingPrototype
{
    /// <inheritdoc/>
    [IdDataField]
    public string ID { get; } = default!;

    /// <inheritdoc/>
    [ParentDataField(typeof(AbstractPrototypeIdArraySerializer<ESMaskSetPrototype>))]
    public string[]? Parents { get; }

    /// <inheritdoc/>
    [NeverPushInheritance]
    [AbstractDataField]
    public bool Abstract { get; }

    // TODO: Weighted set.
    public IReadOnlySet<ProtoId<ESMaskPrototype>> Masks => _masks;

    [DataField("masks", required: true)]
    private HashSet<ProtoId<ESMaskPrototype>> _masks = default!;
}
