using System;
using System.Linq;
using System.CommandLine;
using System.CommandLine.Builder;

namespace Shmuelie.CommandLine.Curses.Runner
{
    internal static class CommandLineBuilderExtensions
    {
        public static CommandLineBuilder AddCommandsInAssembly(this CommandLineBuilder @this)
        {
            foreach (Type commandType in typeof(CommandLineBuilderExtensions).Assembly.GetTypes().Where(t => t.IsSubclassOf(typeof(Command))))
            {
                @this.AddCommand((Command)Activator.CreateInstance(commandType, true));
            }
            return @this;
        }
    }
}
