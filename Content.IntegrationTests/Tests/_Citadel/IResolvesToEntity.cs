using Robust.Shared.GameObjects;

namespace Content.IntegrationTests.Tests._Citadel;

public interface IResolvesToEntity
{
    EntityUid? SEntity { get; }
    EntityUid? CEntity { get; }
}

