using System;
using MethodBoundaryAspect.Fody.Attributes;

namespace Shmuelie.CommandLine.Curses
{
    [AttributeUsage(AttributeTargets.Property, Inherited = true)]
    internal sealed class MustBeZeroOrGreaterAttribute : OnMethodBoundaryAspect
    {
        public override void OnEntry(MethodExecutionArgs arg)
        {
            if (arg.Arguments.Length != 1)
            {
                return;
            }
            switch(arg.Arguments[0])
            {
                case double d when d < 0:
                case float f when f < 0:
                case long l when l < 0:
                case int i when i < 0:
                case short s when s < 0:
                case sbyte b when b < 0:
                case decimal dec when dec < 0:
                    throw new ArgumentOutOfRangeException("value", "Value must be zero or greater");
            }
        }
    }
}
