using System.Collections.Generic;
using System.Reflection;
using JetBrains.Annotations;
using NUnit.Framework.Interfaces;
using NUnit.Framework.Internal;
using NUnit.Framework.Internal.Commands;
using Robust.UnitTesting;

namespace Content.IntegrationTests.Tests._Citadel;

public sealed class RunOnSideAttribute : Attribute, ICommandWrapper, IImplyFixture
{
    /// <summary>
    ///     Which side to run the inner test code on, if not the test thread.
    /// </summary>
    public Side RunOnSide { get; set; }

    public RunOnSideAttribute(Side side)
    {
        RunOnSide = side;
    }

    public RunOnSideAttribute(Side side, params object?[] args)
    {
        RunOnSide = side;
    }

    public TestCommand Wrap(TestCommand command)
    {
        return new SidedTestCommand(command, RunOnSide);
    }

    private sealed class SidedTestCommand : DelegatingTestCommand
    {
        private Side _side;

        public SidedTestCommand(TestCommand inner, Side side) : base(inner)
        {
            _side = side;
        }

        public override TestResult Execute(TestExecutionContext context)
        {
            if (innerCommand.Test.Fixture is not GameTest gt)
            {
                throw new NotSupportedException(
                    $"The fixture {innerCommand.Test.Fixture!.GetType()} needs to be a GameTest for SidedTest to work.");
            }

            if (_side is Side.Neither)
                throw new NotSupportedException($"Sided tests need to specify a side. {Test}");

            TestResult res = null!;
            if (_side is Side.Client)
            {
                gt.Client.WaitAssertion(() =>
                    {
                        res = innerCommand.Execute(context);
                    })
                    .Wait();
            }
            else
            {
                gt.Server.WaitAssertion(() =>
                    {
                        res = innerCommand.Execute(context);
                    })
                    .Wait();
            }

            return res!;
        }
    }
}
