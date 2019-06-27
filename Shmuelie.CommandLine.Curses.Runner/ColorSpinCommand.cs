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
    internal sealed class ColorSpinCommand : Command
    {
        public ColorSpinCommand() : base("color-spin")
        {
            Handler = CommandHandler.Create(new Action<IConsole>(Invoke));
        }

        private static void Invoke(IConsole console)
        {
            ConsoleRenderer consoleRenderer = new ConsoleRenderer(console, OutputMode.Ansi, false);
            ScreenView screenView = new ScreenView(consoleRenderer, console)
            {
                Child = new StackLayoutView(Orientation.Horizontal)
                {
                    new AutoSpinnerView()
                    {
                        Running = true
                    },
                    new AutoSpinnerView()
                    {
                        BackgroundColor = BackgroundColorSpan.Blue(),
                        ForegroundColor = ForegroundColorSpan.Red(),
                        Running = true
                    },
                    new AutoSpinnerView()
                    {
                        Running = true
                    }
                }
            };
            screenView.Render();
            Thread.Sleep(int.MaxValue);
        }
    }
}
