using Content.Shared._Citadel.Utilities;
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

    /// <summary>
    ///     The seed used for this masquerade.
    /// </summary>
    public RngSeed Seed;

    /// <summary>
    ///     The randomizer created from the initial seed and used for all role selection and latejoins.
    /// </summary>
    public SmallRandom Rng = default!;
}
