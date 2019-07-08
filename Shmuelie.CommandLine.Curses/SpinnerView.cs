using System.CommandLine.Rendering;
using System.CommandLine.Rendering.Views;
using System.Linq;

namespace Shmuelie.CommandLine.Curses
{
    public sealed class SpinnerView : View
    {
        private static readonly string[] figit = (new [] { '-', '/', '|', '\\' }).Select(c => c.ToString()).ToArray();
        private static readonly Size size = new Size(1, 1);

        private int state;

        public override Size Measure(ConsoleRenderer renderer, Size maxSize) => size;

        public void Tick()
        {
            state = (state + 1) % figit.Length;
            OnUpdated();
        }

        public override void Render(ConsoleRenderer renderer, Region region) => renderer.RenderToRegion($"{ForegroundColor}{BackgroundColor}{figit[state]}{BackgroundColorSpan.Reset()}{ForegroundColorSpan.Reset()}", region);

        public ForegroundColorSpan ForegroundColor
        {
            get;
            set;
        } = ForegroundColorSpan.Reset();

        public BackgroundColorSpan BackgroundColor
        {
            get;
            set;
        } = BackgroundColorSpan.Reset();
    }
}
