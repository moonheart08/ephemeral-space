#nullable enable
using NUnit.Framework.Constraints;
using Robust.Shared.GameObjects;
using Robust.UnitTesting;

namespace Content.IntegrationTests.Tests._Citadel.Constraints;

public sealed class CompConstraint(Type component, IIntegrationInstance instance) : GameConstraint(instance)
{
    public override ConstraintResult ApplyTo<TActual>(TActual actual)
    {
        if (!TryActualAsEnt(actual, out var ent, out var error))
        {
            if (error)
            {
                throw new NotImplementedException(
                    $"The input type {typeof(TActual)} to {nameof(CompConstraint)} is not a supported entity id.");
            }

            return new ConstraintResult(this, actual, ConstraintStatus.Failure);
        }

        return new ConstraintResult(this, actual, instance.EntMan.HasComponent(ent, component));
    }

    public override string Description => $"has the component {component.Name}.";
}

public static class CompConstraintExtensions
{
    extension(Has)
    {
        public static CompConstraint Comp<T>(IIntegrationInstance instance)
            where T: IComponent
        {
            return new CompConstraint(typeof(T), instance);
        }

        public static CompConstraint Comp(Type t, IIntegrationInstance instance)
        {
            return new CompConstraint(t, instance);
        }
    }

    extension(ConstraintExpression expr)
    {
        public CompConstraint Comp<T>(IIntegrationInstance instance)
            where T: IComponent
        {
            var c = new CompConstraint(typeof(T), instance);

            expr.Append(c);
            return c;
        }

        public CompConstraint Comp(Type t, IIntegrationInstance instance)
        {
            var c = new CompConstraint(t, instance);

            expr.Append(c);
            return c;
        }
    }
}
