#nullable enable
using System.Diagnostics.CodeAnalysis;
using System.Linq.Expressions;
using NUnit.Framework.Constraints;
using Robust.Shared.GameObjects;
using Robust.Shared.Toolshed.TypeParsers;
using Robust.UnitTesting;

namespace Content.IntegrationTests.Tests._Citadel.Constraints;

public abstract class GameConstraint(IIntegrationInstance instance) : Constraint
{
    /// <summary>
    ///     A constraint implementation helper to convert TActual into an entityuid.
    /// </summary>
    /// <param name="t">The input value to try to get an entity uid from.</param>
    /// <param name="ent">The resulting entity uid.</param>
    /// <param name="error">Whether TActual is recognized to begin with.</param>
    /// <typeparam name="TActual">The type to cast out of.</typeparam>
    /// <returns></returns>
    protected bool TryActualAsEnt<TActual>(TActual t, [NotNullWhen(true)] out EntityUid? ent, out bool error)
    {
        if (t is EntityUid u)
        {
            ent = u;
            error = false;
            return true;
        }

        if (t is IAsType<EntityUid> asTy)
        {
            ent = asTy.AsType();
            error = false;
            return true;
        }

        if (t is null)
        {
            ent = null;
            error = false;
            return false;
        }

        ent = null;
        error = true; // Dunno what this type is!
        return false;
    }
}
