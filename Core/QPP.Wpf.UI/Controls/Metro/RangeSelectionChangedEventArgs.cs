using System.Windows;

namespace QPP.Wpf.UI.Controls.Metro
{
    public class RangeSelectionChangedEventArgs : RoutedEventArgs
    {
        public long NewRangeStart { get; set; }
        public long NewRangeStop { get; set; }

        internal RangeSelectionChangedEventArgs(long newRangeStart, long newRangeStop)
        {
            NewRangeStart = newRangeStart;
            NewRangeStop = newRangeStop;
        }

        internal RangeSelectionChangedEventArgs(RangeSlider slider)
            : this(slider.RangeStartSelected, slider.RangeStopSelected)
        { }
    }
}