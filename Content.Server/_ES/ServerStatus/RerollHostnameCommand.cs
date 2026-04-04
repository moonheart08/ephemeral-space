using Robust.Shared.Toolshed;

namespace Content.Server._ES.ServerStatus;

[ToolshedCommand]
public sealed class RerollHostnameCommand : ToolshedCommand
{
    [Dependency] private readonly StatusManager _status = default!;

    [CommandImplementation]
    public void RerollHostname()
    {
        _status.RerollHostname();
    }
}
