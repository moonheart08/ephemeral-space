using System.Reflection;
using Robust.Shared.Configuration;

namespace Content.IntegrationTests.Tests._Citadel.Attributes;

[AttributeUsage(AttributeTargets.Method | AttributeTargets.Class)]
public sealed class EnsureCVarAttribute(Side side, Type definitionType, string fieldName, object value) : Attribute, IGameTestModifier
{
    public Task ApplyToTest(GameTest test)
    {
        var field = definitionType.GetField(fieldName, BindingFlags.Static | BindingFlags.Public);
        var cvar = (CVarDef)field!.GetValue(field);

        // cvars cannot be null
        if (value.GetType() != cvar!.DefaultValue.GetType())
            throw new NotSupportedException($"Cannot set {cvar.Name} to {value}, it's the wrong type.");

        test.PreTestAddOverride(side, cvar!.Name, value);

        return Task.CompletedTask;
    }
}
