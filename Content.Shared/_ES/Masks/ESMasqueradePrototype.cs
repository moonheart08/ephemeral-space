using Content.Shared._ES.Masks.Masquerades;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization.TypeSerializers.Implementations.Custom.Prototype.Array;

namespace Content.Shared._ES.Masks;

/// <summary>
/// This is a prototype for a Masquerade, a set of roles to give for given player counts.
/// </summary>
[Prototype]
public sealed class ESMasqueradePrototype : IPrototype
{
    [Dependency] private readonly ILocalizationManager _loc = default!;

    /// <inheritdoc/>
    [IdDataField]
    public string ID { get; } = default!;

    /// <summary>
    ///     The name for this masquerade. Can be overwritten by localization.
    /// </summary>
    [DataField(required: true)]
    public string Name = default!;

    /// <summary>
    ///     The localized name for this masquerade.
    /// </summary>
    public string LocName => _loc.TryGetString($"es-masquerade-name-{ID}", out var value) ? value : Name;

    /// <summary>
    ///     The name for this masquerade. Can be overwritten by localization.
    /// </summary>
    [DataField(required: true)]
    public string Description = default!;

    /// <summary>
    ///     The localized name for this masquerade.
    /// </summary>
    public string LocDescription => _loc.TryGetString($"es-masquerade-desc-{ID}", out var value) ? value : Description;


    /// <summary>
    ///     Setter for serialization because we're manually inlining some fields from MasqueradeRoleSet.
    /// </summary>
    /// <seealso cref="MasqueradeRoleSet.MinPlayers"/>
    [DataField(priority: 1, required: true)]
    private int MinPlayers
    {
        get => Masquerade.MinPlayers;
        set => Masquerade.MinPlayers = value;
    }

    /// <summary>
    ///     Setter for serialization because we're manually inlining some fields from MasqueradeRoleSet.
    /// </summary>
    /// <seealso cref="MasqueradeRoleSet.MaxPlayers"/>
    [DataField(priority: 1)]
    private int? MaxPlayers
    {
        get => Masquerade.MaxPlayers;
        set => Masquerade.MaxPlayers = value;
    }


    // Due to this being shared, we can't rely on GamePresetPrototype... please don't make typos :3
    /// <summary>
    ///     The GamePreset prototype to use for this masquerade.
    ///     This will always be decoy'd to avoid the existing preset system spoiling the masquerade.
    /// </summary>
    [DataField(required: true, serverOnly: true)]
    public string Preset { get; private set; } = default!;

    [DataField(required: true, priority: 0)]
    public MasqueradeKind Masquerade { get; private set; } = default!;

}

/// <summary>
///     Base class for any masquerades. To introduce new ones, make sure you update the custom serializer too.
/// </summary>
public abstract class MasqueradeKind
{
    public virtual int MinPlayers { get; set; }

    public virtual int? MaxPlayers { get; set; }
};

/// <summary>
///     A truly random masquerade. This mimics the pre-masquerades game behavior of using weights on roles.
/// </summary>
[DataDefinition]
public sealed partial class RandomMasquerade : MasqueradeKind;

/// <summary>
///     A masquerade defined by what roles to add/remove at given levels of players, specifying which roles or role sets
///     to introduce to the round.
/// </summary>
[DataDefinition]
public sealed partial class RoleSetMasquerade : MasqueradeKind
{
    [DataField(required: true, priority: 0)]
    public MasqueradeRoleSet Roles = new();
}
