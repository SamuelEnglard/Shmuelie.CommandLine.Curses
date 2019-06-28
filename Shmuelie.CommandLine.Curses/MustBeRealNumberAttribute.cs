using System;
using MethodBoundaryAspect.Fody.Attributes;

namespace Shmuelie.CommandLine.Curses
{
    [AttributeUsage(AttributeTargets.Property, Inherited = true)]
    internal sealed class MustBeRealNumberAttribute : OnMethodBoundaryAspect
    {
        public override void OnEntry(MethodExecutionArgs arg)
        {
            if (arg.Arguments.Length != 1)
            {
                return;
            }
            switch (arg.Arguments[0])
            {
                case double d when double.IsInfinity(d) || double.IsNaN(d):
                case float f when float.IsInfinity(f) || float.IsNaN(f):
                    throw new ArgumentException("Value must be a real number");
            }
        }
    }
}
