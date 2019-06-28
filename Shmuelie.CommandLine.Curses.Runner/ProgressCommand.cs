using System;
using System.Collections.Generic;
using System.CommandLine;
using System.CommandLine.Invocation;
using System.CommandLine.Rendering;
using System.CommandLine.Rendering.Views;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Shmuelie.CommandLine.Curses.Runner
{
    internal sealed class ProgressCommand : Command
    {
        public ProgressCommand() : base("progress")
        {
            Handler = CommandHandler.Create(new Action<IConsole>(Invoke));
        }

        private static void Invoke(IConsole console)
        {
            ConsoleRenderer consoleRenderer = new ConsoleRenderer(console, OutputMode.Ansi, false);
            ScreenView screenView = new ScreenView(consoleRenderer, console)
            {
                Child = new StackLayoutView(Orientation.Vertical)
                {
                    new ProgressView()
                    {
                        Value = 0.5
                    },
                    new ProgressView()
                    {
                        Value = 0.75,
                        ShowProgressText = true
                    },
                    new ProgressView()
                    {
                        Value = 0.5,
                        ShowProgressText = true,
                        FillForeground = ForegroundColorSpan.Red(),
                        EmptyBackground = BackgroundColorSpan.Blue()
                    }
                }
            };
            screenView.Render();
            Thread.Sleep(int.MaxValue);
        }
    }
}
