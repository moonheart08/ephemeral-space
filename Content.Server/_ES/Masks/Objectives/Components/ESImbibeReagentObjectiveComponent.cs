using Content.Server.Objectives.Components;
using Content.Shared.Chemistry.Reagent;
using Content.Shared.FixedPoint;
using Robust.Shared.Prototypes;


namespace Content.Server._ES.Masks.Objectives.Components;

/// <summary>
///     This contains data for the "imbibe reagent" objective, which requires the engager to drink some units of that
///     reagent.
///     The target amount is set by <see cref="NumberObjectiveComponent"/>.
/// </summary>
/// <seealso cref="ESImbibeReagentObjectiveSystem"/>
[RegisterComponent]
[Access(typeof(ESImbibeReagentObjectiveSystem))]
public sealed partial class ESImbibeReagentObjectiveComponent : Component
{
    /// <summary>
    ///     The possible targets we can pick from on startup.
    /// </summary>
    [DataField(required: true)]
    public List<ProtoId<ReagentPrototype>> PossibleConsumeTargets { get; private set; }

    /// <summary>
    ///     The description for this objective, where $reagent will become the reagent name.
    /// </summary>
    [DataField(required: true)]
    public LocId DescriptionLoc { get; private set; }

    /// <summary>
    ///     The target reagent we need to consume.
    /// </summary>
    [DataField]
    public ProtoId<ReagentPrototype> ConsumeTarget { get; set; } = "Water"; // Tests demand a default.

    /// <summary>
    ///     How much of our target we've consumed.
    /// </summary>
    [DataField]
    public FixedPoint2 ConsumedAmount { get; set; }

    /// <summary>
    ///     Whether we count consuming it from food, and not just drinking.
    /// </summary>
    [DataField(required: true)]
    public bool CanBeFromFood  { get; private set; }
}
