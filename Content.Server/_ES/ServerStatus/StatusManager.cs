using Content.Shared._ES.CCVar;
using Robust.Shared;
using Robust.Shared.Configuration;
using Robust.Shared.Prototypes;
using Robust.Shared.Random;

namespace Content.Server._ES.ServerStatus;

/// <summary>
///     This currently just manages the hostname.
/// </summary>
public sealed class StatusManager
{
    [Dependency] private readonly IConfigurationManager _cfg = default!;
    [Dependency] private readonly IPrototypeManager _proto = default!;
    [Dependency] private readonly ILocalizationManager _loc = default!;
    [Dependency] private readonly IRobustRandom _random = default!;

    public string? CurrentRoleplayLevel { get; private set; }

    public string? CurrentRoleplayAbbreviation
        => CurrentRoleplayLevel != null ? $"{char.ToUpperInvariant(CurrentRoleplayLevel[0])}RP" : null;

    public void Initialize()
    {
        _cfg.OnValueChanged(ESCVars.FormattedHostName, OnHostnameChanged, true);
    }

    private void OnHostnameChanged(string newValue, in CVarChangeInfo info)
    {
        var levelSet = _proto.Index<ESRoleplayLevelsPrototype>(_cfg.GetCVar(ESCVars.RoleplayLevels));

        var titleWord = levelSet.GetPossibleRoleplay(_loc, _proto, _random);

        CurrentRoleplayLevel = titleWord.ToUpper();

        _cfg.SetCVar(CVars.GameHostName, string.Format(newValue, CurrentRoleplayAbbreviation, CurrentRoleplayLevel));
    }
}
