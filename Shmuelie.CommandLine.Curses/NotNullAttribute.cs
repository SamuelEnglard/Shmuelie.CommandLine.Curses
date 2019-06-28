using System;
using MethodBoundaryAspect.Fody.Attributes;

namespace Shmuelie.CommandLine.Curses
{
    [AttributeUsage(AttributeTargets.Property, Inherited = true)]
    internal sealed class NotNullAttribute : OnMethodBoundaryAspect
    {
        public override void OnEntry(MethodExecutionArgs arg)
        {
            if (arg.Arguments.Length != 1)
            {
                return;
            }
            if (arg.Arguments[0] is null)
            {
                throw new ArgumentNullException("value");
            }
        }
    }
}
