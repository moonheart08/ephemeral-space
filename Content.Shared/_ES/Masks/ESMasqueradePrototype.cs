using Content.Shared._ES.Masks.Masquerades;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization.TypeSerializers.Implementations.Custom.Prototype.Array;

namespace Content.Shared._ES.Masks;

/// <summary>
/// This is a prototype for a Masquerade, a set of roles to give for given player counts.
/// </summary>
[Prototype]
public sealed class ESMasqueradePrototype : IPrototype, IInheritingPrototype
{
    /// <inheritdoc/>
    [IdDataField]
    public string ID { get; } = default!;

    /// <inheritdoc/>
    [ParentDataField(typeof(AbstractPrototypeIdArraySerializer<ESMasqueradePrototype>))]
    public string[]? Parents { get; }

    /// <inheritdoc/>
    [NeverPushInheritance]
    [AbstractDataField]
    public bool Abstract { get; }

    // [IncludeDataField] / Include doesn't work for this kind of custom serializer, unfortunately.
    [DataField(required: true)]
    [AlwaysPushInheritance]
    public MasqueradeRoleSet Roles = default!;
}
