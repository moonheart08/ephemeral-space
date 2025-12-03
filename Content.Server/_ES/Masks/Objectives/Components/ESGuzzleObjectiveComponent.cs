using Content.Server.Objectives.Components;
using Content.Shared.FixedPoint;

namespace Content.Server._ES.Masks.Objectives.Components;

/// <summary>
///     THE GUZZLER WUZ HERE!!!
///     This contains data for a particular kind of objective that requires imbibing X amount of reagents, total.
///     If you're here from the guzzler design doc, this does not handle the specific request reagent, that's in
///     <see cref="ESImbibeReagentObjectiveComponent"/>.
///     The target is set by <see cref="NumberObjectiveComponent"/>.
/// </summary>
/// <seealso cref="ESGuzzleObjectiveSystem"/>
[RegisterComponent]
[Access(typeof(ESGuzzleObjectiveSystem))]
public sealed partial class ESGuzzleObjectiveComponent : Component
{
    /// <summary>
    ///     The amount of reagents currently consumed.
    /// </summary>
    [DataField]
    public FixedPoint2 ReagentsConsumed { get; set; } = FixedPoint2.Zero;
}
