/*
 * ********************************
 *  Copyright © 2009. CoderForRent,LLC. All Rights Reserved.  Licensed under the GNU General Public License version 2 (GPLv2) .
 * 
 * */


using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using QPP.Wpf.UI.Controls.XGantt.Core;

namespace QPP.Wpf.UI.Controls.XGantt.TimespanHeader
{
    [TemplatePart(Name = "RowsPresenter", Type = typeof(TimespanHeaderRowsPresenter))]
    public class TimespanHeader : ContentControl, IMouseWheelObserver
    {
        public static readonly DependencyProperty CurrentTimeProperty;
        public static readonly DependencyProperty ZoomFactorProperty;
        public static readonly DependencyProperty TopBarTimeUnitProperty;
        public static readonly DependencyProperty BottomBarTimeUnitProperty;

        static TimespanHeader()
        {
            var thisType = typeof(TimespanHeader);
            DefaultStyleKeyProperty.OverrideMetadata(thisType, new FrameworkPropertyMetadata(typeof(TimespanHeader)));
            CurrentTimeProperty = DependencyProperty.Register("CurrentTime", typeof(DateTime), thisType, new PropertyMetadata(DateTime.Now, OnCurrentTimeChanged));
            ZoomFactorProperty = DependencyProperty.Register("ZoomFactor", typeof(double), thisType, new PropertyMetadata(1d));
            TopBarTimeUnitProperty = DependencyProperty.Register("TopBarTimeUnit", typeof(TimeUnits), thisType, new PropertyMetadata(TimeUnits.Months));
            BottomBarTimeUnitProperty = DependencyProperty.Register("BottomBarTimeUnit", typeof(TimeUnits), thisType, new PropertyMetadata(TimeUnits.Days));
        }

        static void OnCurrentTimeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var x = d as TimespanHeader;
            if (x.IsInitialized)
            {
                x.UpdateRowTimes();
                x.InvalidateRowPresenters();
                x.RaiseCurrentTimeChanged(EventArgs.Empty);
            }
        }

        public event EventHandler CurrentTimeChanged;

        public event EventHandler ZoomFactorChanged;

        protected void RaiseCurrentTimeChanged(EventArgs e)
        {
            if (CurrentTimeChanged != null)
                CurrentTimeChanged(this, e);
        }

        protected void RaiseZoomFactorChange(EventArgs e)
        {
            if (ZoomFactorChanged != null)
                ZoomFactorChanged(this, e);
        }

        public TimeUnits LowerUnit
        {
            get
            {
                if (this.RowsPresenter.Children.Count > 0)
                    return (this.RowsPresenter.Children[this.RowsPresenter.Children.Count - 1] as TimespanHeaderRow).TimeUnit;
                else
                    return TimeUnits.Days;
            }
        }

        public DateTime CurrentTime
        {
            get { return (DateTime)GetValue(CurrentTimeProperty); }
            set { SetValue(CurrentTimeProperty, value); }
        }

        public TimeUnits TopBarTimeUnit
        {
            get { return (TimeUnits)GetValue(TopBarTimeUnitProperty); }
            set { SetValue(TopBarTimeUnitProperty, value); }
        }

        public TimeUnits BottomBarTimeUnit
        {
            get { return (TimeUnits)GetValue(BottomBarTimeUnitProperty); }
            set { SetValue(BottomBarTimeUnitProperty, value); }
        }

        public GanttCalendar Calendar
        {
            get;
            set;
        }

        public double ZoomFactor
        {
            get
            {
                return Zoom.Value;
            }
            set
            {
                if (value != ZoomFactor)
                {
                    Zoom.Value = value;
                    InvalidateRowPresenters();
                    RaiseZoomFactorChange(EventArgs.Empty);
                }
            }
        }

        internal TimespanHeaderRowsPresenter RowsPresenter { get; set; }

        public TimespanHeader()
        {
            this.SizeChanged += this_SizeChanged;
        }

        #region Drag and Drop

        protected override void OnMouseDown(MouseButtonEventArgs e)
        {
            base.OnMouseDown(e);
            _LastMouseDownPosition = e.GetPosition(this);
            IsMouseDown = true;
        }

        protected override void OnMouseUp(MouseButtonEventArgs e)
        {
            base.OnMouseUp(e);
            IsMouseDown = false;
        }

        protected override void OnMouseLeave(MouseEventArgs e)
        {
            base.OnMouseLeave(e);
            IsMouseDown = false;
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e);
            if (IsMouseDown && e.LeftButton == MouseButtonState.Pressed)
            {
                Cursor = Cursors.Hand;

                Point p = e.GetPosition(this);
                double dist = (_LastMouseDownPosition.X - p.X);
                if (_MinimumDragDistance <= Math.Abs(dist))
                {
                    _LastMouseDownPosition = p;

                    TimeUnits tu = (RowsPresenter.Children[RowsPresenter.Children.Count - 1] as TimespanHeaderRow).TimeUnit;
                    if (tu == TimeUnits.Hours)
                    {
                        CurrentTime = CurrentTime.AddType(TimeUnits.Hours, dist / GetWidth(CurrentTime, TimeUnits.Hours));
                    }
                    else
                        CurrentTime = CurrentTime.AddType(TimeUnits.Days, ConvertDistanceToDays(dist));
                }
            }
            else
            {
                Cursor = Cursors.Arrow;
            }
        }

        internal bool IsMouseDown { get; private set; }
        private Point _LastMouseDownPosition = new Point(0, 0);
        private double _MinimumDragDistance = 15.0d;

        private double ConvertDistanceToDays(double dist)
        {
            double unitWidth = GetWidth(CurrentTime, TimeUnits.Days);
            return (dist / unitWidth);
        }

        #endregion

        protected void this_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            InvalidateRowPresenters();
        }

        private void InvalidateRowPresenters()
        {
            this.RowsPresenter.InvalidateCells();
        }

        private void UpdateRowTimes()
        {
            this.RowsPresenter.UpdateCurrentTime(CurrentTime);
        }

        protected override Size ArrangeOverride(Size finalSize)
        {
            return base.ArrangeOverride(finalSize);
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            this.RowsPresenter = (TimespanHeaderRowsPresenter)GetTemplateChild("RowsPresenter");
            this.RowsPresenter
                .Children
                .OfType<TimespanHeaderRow>()
                .ToList()
                .ForEach(r => r.ParentTimespanHeader = this);
        }

        internal double GetTotalUnits()
        {
            return this.DesiredSize.Width;
        }

        internal double GetWidth(DateTime time, TimeUnits timeUnit)
        {
            return TimeUnitScalar.GetWidth(CurrentTime, time, timeUnit);
        }

        #region IMouseWheelObserver Members

        public void OnMouseWheel(MouseWheelArgs args)
        {
            double result = ZoomFactor;
            result += args.Delta * 0.2;
            if (result > 0.2 && result < 2.0)
                ZoomFactor = result;
            //else if (result > 2.0)
            //{
            //    ZoomFactor = 1;
            //    foreach (TimespanHeaderRow row in _RowsPresenter.Children)
            //        row.DecreaseScope();

            //}
            //else if (result < 0.2)
            //{
            //    ZoomFactor = 1;
            //    foreach (TimespanHeaderRow row in _RowsPresenter.Children)
            //        row.IncreaseScope();
            //}
        }

        #endregion

        internal double GetPosition(DateTime d)
        {
            return TimeUnitScalar.GetPosition(CurrentTime, d);
        }

        protected override void OnMouseDoubleClick(MouseButtonEventArgs e)
        {
            base.OnMouseDoubleClick(e);

            if (e.LeftButton == MouseButtonState.Pressed)
            {
                if (Keyboard.Modifiers == ModifierKeys.Shift)
                {
                    ZoomFactor -= ZoomFactor / 2;
                    if (ZoomFactor < 0.3)
                        ZoomFactor = 0.3;
                }
                else
                {
                    ZoomFactor += ZoomFactor / 2;
                    if (ZoomFactor > 10)
                        ZoomFactor = 10;
                }
            }
        }
    }
}
