using System.Linq;
using Content.Shared.Dataset;
using Robust.Shared.Collections;
using Robust.Shared.Prototypes;
using Robust.Shared.Random;
using Robust.Shared.Serialization;

namespace Content.Server._ES.ServerStatus;

/// <summary>
///     This holds data for the random roleplay levels feature.
/// </summary>
[Prototype("esRoleplayLevels")]
public sealed partial class ESRoleplayLevelsPrototype : IPrototype, ISerializationHooks
{
    /// <inheritdoc/>
    [IdDataField]
    public string ID { get; set; } = default!;

    /// <summary>
    ///     Characters that roleplay levels are not allowed to start with.
    ///     These get validated and will cause validation failures.
    /// </summary>
    [DataField(required: true)]
    public List<char> ForbidCharacters = default!;

    /// <summary>
    ///     Localized datasets we also add in to our list of roleplays.
    /// </summary>
    public List<ProtoId<LocalizedDatasetPrototype>> LocalizedDatasets = new();

    /// <summary>
    ///     The kinds of roleplays in this dataset.
    /// </summary>
    [DataField(required: true)]
    public List<string> Roleplays = default!;

    void ISerializationHooks.AfterDeserialization()
    {
        var badRoleplays = new ValueList<string>();

        foreach (var roleplay in Roleplays)
        {
            if (ForbidCharacters.Any(x => roleplay.StartsWith(x)))
                badRoleplays.Add(roleplay);
        }

        if (badRoleplays.Count > 0)
        {
            throw new Exception(
                $"Some roleplays in {ID} violate the forbidden characters: {string.Join(", ", badRoleplays)}");
        }
    }

    public string GetPossibleRoleplay(ILocalizationManager loc, IPrototypeManager proto, IRobustRandom random)
    {
        var total = 0;

        foreach (var setProtoId in LocalizedDatasets)
        {
            var set = proto.Index(setProtoId);

            total += set.Values.Count;
        }

        total += Roleplays.Count;

        var index = random.Next(total);

        total = 0;

        // Iterate each dataset and see if our random index falls into it.
        foreach (var setProtoId in LocalizedDatasets)
        {
            var set = proto.Index(setProtoId);

            total += set.Values.Count;

            if (total > index)
            {
                index -= (total - set.Values.Count);
                return loc.GetString(set.Values[index]);
            }
        }

        // okay just..
        index -= total;

        return Roleplays[index];
    }
}
