using System;
using System.CommandLine.Rendering;
using System.CommandLine.Rendering.Views;
using System.Threading;
using System.Threading.Tasks;

namespace Shmuelie.CommandLine.Curses
{
    public sealed class AutoSpinnerView : View, IDisposable
    {
        private readonly SpinnerView spinnerView = new SpinnerView();
        private CancellationTokenSource cancellationTokenSource;
        private readonly Action<Task> tick;

        public AutoSpinnerView()
        {
            tick = new Action<Task>(Tick);
        }

        public TimeSpan Interval
        {
            get;
            set;
        } = TimeSpan.FromMilliseconds(100);

        public bool Running
        {
            get
            {
                return cancellationTokenSource != null;
            }
            set
            {
                if (value != Running)
                {
                    if (!Running)
                    {
                        cancellationTokenSource = new CancellationTokenSource();
                        Task.Delay(Interval, cancellationTokenSource.Token).ContinueWith(tick);
                    }
                    else
                    {
                        cancellationTokenSource.Cancel();
                        cancellationTokenSource.Dispose();
                        cancellationTokenSource = null;
                    }
                }
            }
        }

        private void Tick(Task task)
        {
            if (task.IsCanceled || cancellationTokenSource is null || cancellationTokenSource.IsCancellationRequested)
            {
                return;
            }
            spinnerView.Tick();
            OnUpdated();
            Task.Delay(Interval, cancellationTokenSource.Token).ContinueWith(tick);
        }

        public override Size Measure(ConsoleRenderer renderer, Size maxSize) => spinnerView.Measure(renderer, maxSize);

        public override void Render(ConsoleRenderer renderer, Region region) => spinnerView.Render(renderer, region);

        public ForegroundColorSpan ForegroundColor
        {
            get
            {
                return spinnerView.ForegroundColor;
            }
            set
            {
                spinnerView.ForegroundColor = value;
            }
        }

        public BackgroundColorSpan BackgroundColor
        {
            get
            {
                return spinnerView.BackgroundColor;
            }
            set
            {
                spinnerView.BackgroundColor = value;
            }
        }

        public override string ToString() => spinnerView.ToString();

        public void Dispose()
        {
            cancellationTokenSource?.Dispose();
            cancellationTokenSource = null;
        }
    }
}
