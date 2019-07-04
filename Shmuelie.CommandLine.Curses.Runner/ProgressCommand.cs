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
                        Value = 50
                    },
                    new ProgressView()
                    {
                        Value = 75,
                        ShowProgressText = ProgressViewTextPosition.Center
                    },
                    new ProgressView()
                    {
                        Value = 50,
                        ShowProgressText = ProgressViewTextPosition.Start,
                        FillForeground = ForegroundColorSpan.Red(),
                        EmptyBackground = BackgroundColorSpan.Blue()
                    },
                    new ProgressView()
                    {
                        Value = 25,
                        ShowProgressText = ProgressViewTextPosition.End,
                        FillBackground = BackgroundColorSpan.Green(),
                        FillCharacter = ' '
                    }
                }
            };
            screenView.Render();
            Thread.Sleep(int.MaxValue);
        }
    }
}
