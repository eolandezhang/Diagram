using System;
using System.Windows;
using System.Windows.Controls;
using QPP.Wpf.UI.Controls.Gantt.Core;

namespace QPP.Wpf.UI.Controls.Gantt.TimespanHeader
{
    public class TimespanHeaderCell : Control
    {
        public static DependencyProperty TextProperty
            = DependencyProperty.Register("Text", typeof(string), typeof(TimespanHeaderCell), new PropertyMetadata("no data"));
        public static DependencyProperty IsWorkingProperty
            = DependencyProperty.Register("IsWorking", typeof(bool), typeof(TimespanHeaderCell), new PropertyMetadata(true));
        public string Text
        {
            get { return (string)GetValue(TextProperty); }
            set { SetValue(TextProperty, value); }
        }

        public bool IsWorking
        {
            get { return (bool)GetValue(IsWorkingProperty); }
            set { SetValue(IsWorkingProperty, value); }
        }

        internal TimespanHeaderRow ParentRow { get; set; }

        private DateTime _DateTime;
        public DateTime DateTime
        {
            get { return _DateTime; }
            set
            {
                if (_DateTime != value)
                {
                    _DateTime = value;
                    UpdateText();
                }
            }
        }

        private string _Format;
        public string Format
        {
            get { return _Format; }
            set
            {
                if (_Format != value)
                {
                    _Format = value;
                    UpdateText();
                }
            }
        }

        private void UpdateText()
        {
            if (Format == "WEEK")
                Text = "Week " + TimeUnitScalar.GetWeekOfYear(this.DateTime).ToString();
            else
                Text = this.DateTime.ToString(this.Format, System.Globalization.CultureInfo.CurrentCulture);
        }

        static TimespanHeaderCell()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(TimespanHeaderCell), new FrameworkPropertyMetadata(typeof(TimespanHeaderCell)));
        }

        public TimespanHeaderCell()
        {
            this.UseLayoutRounding = false;
        }
    }
}
