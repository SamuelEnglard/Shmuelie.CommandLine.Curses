using System;
using System.Collections.Generic;
using System.CommandLine.Rendering;
using System.CommandLine.Rendering.Views;
using System.ComponentModel;
using System.Globalization;
using PropertyChanged;

namespace Shmuelie.CommandLine.Curses
{
    public sealed class ProgressView : View, INotifyPropertyChanged
    {
        public ProgressView()
        {
            PropertyChanged += HandlePropertyChange;
        }

        private static void HandlePropertyChange(object sender, PropertyChangedEventArgs eventArgs)
        {
            if (sender is ProgressView progressView && eventArgs.PropertyName != nameof(Percent))
            {
                progressView.OnUpdated();
            }
        }

        [MustBeRealNumber]
        public double Value
        {
            get;
            set;
        } = 0;

        [MustBeRealNumber]
        public double MinValue
        {
            get;
            set;
        } = 0;

        [MustBeRealNumber]
        public double MaxValue
        {
            get;
            set;
        } = 100;

        [DependsOn(nameof(Value), nameof(MinValue), nameof(MaxValue))]
        public double Percent => Value / (MaxValue - MinValue);

        public char FillCharacter
        {
            get;
            set;
        } = '=';

        [NotNull]
        public ForegroundColorSpan FillForeground
        {
            get;
            set;
        } = ForegroundColorSpan.Reset();

        [NotNull]
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

        [NotNull]
        public ForegroundColorSpan EmptyForeground
        {
            get;
            set;
        } = ForegroundColorSpan.Reset();

        [NotNull]
        public BackgroundColorSpan EmptyBackground
        {
            get;
            set;
        } = BackgroundColorSpan.Reset();

        [MustBeZeroOrGreater]
        public int? Width
        {
            get;
            set;
        } = null;

        public char? PrefixCharacter
        {
            get;
            set;
        } = '[';

        [NotNull]
        public ForegroundColorSpan PrefixForeground
        {
            get;
            set;
        } = ForegroundColorSpan.Reset();

        [NotNull]
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

        [NotNull]
        public ForegroundColorSpan SuffixForeground
        {
            get;
            set;
        } = ForegroundColorSpan.Reset();

        [NotNull]
        public BackgroundColorSpan SuffixBackground
        {
            get;
            set;
        } = BackgroundColorSpan.Reset();

        public ProgressViewTextPosition ShowProgressText
        {
            get;
            set;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        [NotNull]
        public IFormatProvider FormatProvider
        {
            get;
            set;
        } = CultureInfo.CurrentCulture;

        [NotNull]
        public string Format
        {
            get;
            set;
        } = "P0";

        public override Size Measure(ConsoleRenderer renderer, Size maxSize) => new Size(Math.Max(MinWidth, maxSize.Width), 1);

        private int MinWidth
        {
            get
            {
                int minWidth = 0;
                if (PrefixCharacter.HasValue)
                {
                    minWidth++;
                }
                if (SuffixCharacter.HasValue)
                {
                    minWidth++;
                }
                if (ShowProgressText != ProgressViewTextPosition.None)
                {
                    minWidth += ToString().Length;
                }
                if (Width.HasValue && Width.Value > minWidth)
                {
                    minWidth = Width.Value;
                }
                return minWidth;
            }
        }

        public override void Render(ConsoleRenderer renderer, Region region)
        {
            int outerWidth = 0;
            List<Span> spans = new List<Span>(19);
            if (PrefixCharacter.HasValue)
            {
                spans.Add(PrefixForeground);
                spans.Add(PrefixBackground);
                spans.Add(new ContentSpan(PrefixCharacter.Value.ToString()));
                spans.Add(BackgroundColorSpan.Reset());
                spans.Add(ForegroundColorSpan.Reset());
                outerWidth++;
            }
            if (SuffixCharacter.HasValue)
            {
                outerWidth++;
            }
            int barInnerWidth = Math.Max(0, region.Width - outerWidth);
            int filledWidth = (int)(barInnerWidth * Percent);
            spans.Add(FillForeground);
            spans.Add(FillBackground);
            string percentText = ToString();
            if (barInnerWidth >= percentText.Length && ShowProgressText != ProgressViewTextPosition.None)
            {
                int textLocation = 0 ;
                switch (ShowProgressText)
                {
                    case ProgressViewTextPosition.Center:
                        textLocation = barInnerWidth / 2 - (percentText.Length / 2);
                        break;
                    case ProgressViewTextPosition.End:
                        textLocation = barInnerWidth - percentText.Length;
                        break;
                }
                if (filledWidth > textLocation)
                {
                    spans.Add(new ContentSpan(new string(FillCharacter, textLocation)));
                    int filledText = filledWidth - textLocation;
                    if (filledText <= percentText.Length)
                    {
                        spans.Add(new ContentSpan(percentText.Substring(0, filledText)));
                        spans.Add(EmptyForeground);
                        spans.Add(EmptyBackground);
                        spans.Add(new ContentSpan(percentText.Substring(filledText) + new string(EmptyCharacter, barInnerWidth - (filledWidth + (percentText.Length - filledText)))));
                    }
                    else
                    {
                        spans.Add(new ContentSpan(percentText + new string(FillCharacter, filledWidth - (textLocation + percentText.Length))));
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
                    spans.Add(new ContentSpan(new string(EmptyCharacter, textLocation - filledWidth) + percentText + new string(EmptyCharacter, barInnerWidth - (textLocation + percentText.Length))));
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

        public override string ToString() => Percent.ToString(Format, FormatProvider);
    }
}
