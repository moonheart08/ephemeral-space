using Robust.Shared.GameStates;
using Robust.Shared.Prototypes;

namespace Content.Shared.Tag;

[RegisterComponent, NetworkedComponent, AutoGenerateComponentState, Access(typeof(TagSystem))]
[Obsolete("Marker components, i.e. dedicated components created for a given thing, are always better. Do not add new uses of tags.")]
public sealed partial class TagComponent : Component
{
    [DataField, ViewVariables, AutoNetworkedField]
    public HashSet<ProtoId<TagPrototype>> Tags = new();
}
