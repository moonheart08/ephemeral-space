using Content.Shared._ES.Masks;
using Robust.Shared.Prototypes;

namespace Content.Server._ES.Masks.Masquerades;

/// <summary>
/// This is used for...
/// </summary>
[RegisterComponent]
public sealed partial class ESMasqueradeRuleComponent : Component
{
    /// <summary>
    ///     The running masquerade.
    /// </summary>
    public ESMasqueradePrototype? Masquerade;


}
