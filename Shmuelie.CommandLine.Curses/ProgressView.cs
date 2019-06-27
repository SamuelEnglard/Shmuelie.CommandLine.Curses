using System;
using System.Collections.Generic;
using System.CommandLine.Rendering;
using System.CommandLine.Rendering.Views;
using System.ComponentModel;

namespace Shmuelie.CommandLine.Curses
{
    public sealed class ProgressView : View, INotifyPropertyChanged
    {
        private double progress;
        private int? width;

        public double Progress
        {
            get
            {
                return progress;
            }
            set
            {
                if (value > 1 || value < 0)
                {
                    throw new ArgumentOutOfRangeException(nameof(value), "Progress must be between 0 and 1, inclusive.");
                }
                if (value != progress)
                {
                    progress = value;
                    OnUpdated();
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Progress)));
                }
            }
        }

        public char FillCharacter
        {
            get;
            set;
        } = '=';

        public ForegroundColorSpan FillForeground
        {
            get;
            set;
        } = ForegroundColorSpan.Reset();

        public BackgroundColorSpan FillBackground
        {
            get;
            set;
        } = BackgroundColorSpan.Reset();

        public char EmptyCharacter
        {
            get;
            set;
        } = ' ';

        public ForegroundColorSpan EmptyForeground
        {
            get;
            set;
        } = ForegroundColorSpan.Reset();

        public BackgroundColorSpan EmptyBackground
        {
            get;
            set;
        } = BackgroundColorSpan.Reset();

        public int? Width
        {
            get
            {
                return width;
            }
            set
            {
                if (value != null && value < 1)
                {
                    throw new ArgumentOutOfRangeException(nameof(value), "Width must be greater than zero");
                }
                if (value != width)
                {
                    width = value;
                    OnUpdated();
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Width)));
                }
            }
        }

        public char? PrefixCharacter
        {
            get;
            set;
        } = '[';

        public ForegroundColorSpan PrefixForeground
        {
            get;
            set;
        } = ForegroundColorSpan.Reset();

        public BackgroundColorSpan PrefixBackground
        {
            get;
            set;
        } = BackgroundColorSpan.Reset();

        public char? SuffixCharacter
        {
            get;
            set;
        } = ']';

        public ForegroundColorSpan SuffixForeground
        {
            get;
            set;
        } = ForegroundColorSpan.Reset();

        public BackgroundColorSpan SuffixBackground
        {
            get;
            set;
        } = BackgroundColorSpan.Reset();

        public bool ShowProgressText
        {
            get;
            set;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public override Size Measure(ConsoleRenderer renderer, Size maxSize)
        {
            if (width is null)
            {
                return new Size(maxSize.Width, 1);
            }
            return new Size(Math.Min(width.Value, maxSize.Width), 1);
        }

        public override void Render(ConsoleRenderer renderer, Region region)
        {
            int minWidth = 0;
            List<Span> spans = new List<Span>(19);
            if (PrefixCharacter.HasValue)
            {
                spans.Add(PrefixForeground);
                spans.Add(PrefixBackground);
                spans.Add(new ContentSpan(PrefixCharacter.Value.ToString()));
                spans.Add(BackgroundColorSpan.Reset());
                spans.Add(ForegroundColorSpan.Reset());
                minWidth++;
            }
            if (SuffixCharacter.HasValue)
            {
                minWidth++;
            }
            int barInnerWidth = Math.Max(0, (width ?? region.Width) - minWidth);
            int filledWidth = (int)(barInnerWidth * progress);
            spans.Add(FillForeground);
            spans.Add(FillBackground);
            if (barInnerWidth >= 7 && ShowProgressText)
            {
                int textLocation = barInnerWidth / 2 - 3;
                string percentText = ToString();
                if (filledWidth > textLocation)
                {
                    spans.Add(new ContentSpan(new string(FillCharacter, textLocation)));
                    int filledText = filledWidth - textLocation;
                    if (filledText <= 7)
                    {
                        spans.Add(new ContentSpan(percentText.Substring(0, filledText)));
                        spans.Add(EmptyForeground);
                        spans.Add(EmptyBackground);
                        spans.Add(new ContentSpan(percentText.Substring(filledText) + new string(EmptyCharacter, barInnerWidth - (filledWidth + (7 - filledText)))));
                    }
                    else
                    {
                        spans.Add(new ContentSpan(percentText + new string(FillCharacter, filledWidth - (textLocation + 7))));
                        spans.Add(EmptyForeground);
                        spans.Add(EmptyBackground);
                        spans.Add(new ContentSpan(new string(EmptyCharacter, barInnerWidth - filledWidth)));
                    }
                }
                else
                {
                    spans.Add(new ContentSpan(new string(FillCharacter, filledWidth)));
                    spans.Add(EmptyForeground);
                    spans.Add(EmptyBackground);
                    spans.Add(new ContentSpan(new string(EmptyCharacter, textLocation - filledWidth) + percentText + new string(EmptyCharacter, barInnerWidth - (textLocation + 7))));
                }
            }
            else
            {
                spans.Add(new ContentSpan(new string(FillCharacter, filledWidth)));
                spans.Add(EmptyForeground);
                spans.Add(EmptyBackground);
                spans.Add(new ContentSpan(new string(EmptyCharacter, barInnerWidth - filledWidth)));
            }
            spans.Add(BackgroundColorSpan.Reset());
            spans.Add(ForegroundColorSpan.Reset());
            if (SuffixCharacter.HasValue)
            {
                spans.Add(SuffixForeground);
                spans.Add(SuffixBackground);
                spans.Add(new ContentSpan(SuffixCharacter.Value.ToString()));
                spans.Add(BackgroundColorSpan.Reset());
                spans.Add(ForegroundColorSpan.Reset());
            }
            renderer.RenderToRegion(new ContainerSpan(spans.ToArray()), region);
        }

        public override string ToString() => $"{progress,7:P}";
    }
}
