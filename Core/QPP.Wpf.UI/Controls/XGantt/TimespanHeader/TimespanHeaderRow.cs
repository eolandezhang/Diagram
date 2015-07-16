using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using QPP.Wpf.UI.Controls.XGantt.Core;

namespace QPP.Wpf.UI.Controls.XGantt.TimespanHeader
{
    [TemplatePart(Name = "CellsPresenter", Type = typeof(TimespanHeaderCellsPresenter))]
    public class TimespanHeaderRow : Control
    {
        public static DependencyProperty CellHorizontalAlignmentProperty;
        public static DependencyProperty CellVerticalAlignmentProperty;
        public static DependencyProperty CellBorderThicknessProperty;
        public static DependencyProperty CellBorderBrushProperty;
        public static DependencyProperty CellBackgroundProperty;
        public static DependencyProperty TimeUnitProperty;
        public static DependencyProperty CurrentTimeProperty;
        public static DependencyProperty CellFormatProperty;

        static TimespanHeaderRow()
        {
            var thisType = typeof(TimespanHeaderRow);
            DefaultStyleKeyProperty.OverrideMetadata(thisType, new FrameworkPropertyMetadata(typeof(TimespanHeaderRow)));
            CellHorizontalAlignmentProperty = DependencyProperty.Register("CellHorizontalAlignment", typeof(HorizontalAlignment), thisType, new PropertyMetadata(HorizontalAlignment.Center));
            CellVerticalAlignmentProperty = DependencyProperty.Register("CellVerticalAlignment", typeof(VerticalAlignment), thisType, new PropertyMetadata(VerticalAlignment.Center));
            CellBorderThicknessProperty = DependencyProperty.Register("CellBorderThickness", typeof(Thickness), thisType, new PropertyMetadata(new Thickness(1)));
            CellBorderBrushProperty = DependencyProperty.Register("CellBorderBrush", typeof(Brush), thisType, new PropertyMetadata(new SolidColorBrush(Colors.Black)));
            CellBackgroundProperty = DependencyProperty.Register("CellBackground", typeof(Brush), thisType, new PropertyMetadata(new SolidColorBrush(Colors.Transparent)));
            TimeUnitProperty = DependencyProperty.Register("TimeUnit", typeof(TimeUnits), thisType, new PropertyMetadata(TimeUnits.Months));
            CurrentTimeProperty = DependencyProperty.Register("CurrentTime", typeof(DateTime), thisType, new PropertyMetadata(DateTime.Now));
            CellFormatProperty = DependencyProperty.Register("CellFormat", typeof(string), thisType, new PropertyMetadata(null));
        }

        public DateTime CurrentTime
        {
            get { return (DateTime)GetValue(CurrentTimeProperty); }
            set {
				if (CurrentTime != value)
				{
					SetValue(CurrentTimeProperty, value);
					CellsValid = false;
					InvalidateMeasure();
				}
            }
        }
        public TimeUnits TimeUnit
        {
            get { return (TimeUnits)GetValue(TimeUnitProperty); }
            set { SetValue(TimeUnitProperty, value); ResetFormatting(); }
        }
        public Thickness CellBorderThickness { get { return (Thickness)this.GetValue(CellBorderThicknessProperty); } set { this.SetValue(CellBorderThicknessProperty, value); } }
        public Brush CellBorderBrush { get { return (Brush)this.GetValue(CellBorderBrushProperty); } set { this.SetValue(CellBorderBrushProperty, value); } }
        public Brush CellBackground { get { return (Brush)this.GetValue(CellBackgroundProperty); } set { this.SetValue(CellBackgroundProperty, value); } }
        public string CellFormat { get { return (string)this.GetValue(CellFormatProperty); } set { this.SetValue(CellFormatProperty, value); } }
        public HorizontalAlignment CellHorizontalAlignment { get { return (HorizontalAlignment)this.GetValue(CellHorizontalAlignmentProperty); } set { this.SetValue(CellHorizontalAlignmentProperty, value); } }
        public VerticalAlignment CellVerticalAlignment { get { return (VerticalAlignment)this.GetValue(CellVerticalAlignmentProperty); } set { this.SetValue(CellVerticalAlignmentProperty, value); } }

        public bool CellsValid { get; set; }

        public TimespanHeader ParentTimespanHeader { get; set; }

        private TimespanHeaderCellsPresenter _CellsPresenter;

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            _CellsPresenter = (TimespanHeaderCellsPresenter)GetTemplateChild("CellsPresenter");
            _CellsPresenter.ParentRow = this;

        }

        public TimespanHeaderRow()
        {
            this.UseLayoutRounding = false;
        }

        protected override Size ArrangeOverride(Size finalSize)
        {
            GenerateCells();
            return base.ArrangeOverride(finalSize);
        }

        private void GenerateCells()
        {
            if (!CellsValid)
            {
                CellsValid = true;
                _CellsPresenter.Children.Clear();
                
                double location = 0d;
                double totalUnits = ParentTimespanHeader.GetTotalUnits();
                DateTime time = CurrentTime;

                while (location < totalUnits)
                {
                    TimespanHeaderCell cell = GetCell( time);

                    location +=  cell.Width ;
  
                    _CellsPresenter.Children.Add(cell);

                    //if (TimeUnit == TimeUnits.Weeks && TimeUnitScalar.GetWeekOfYear(time) == 52)
                    //{
                    //    time = time.AddDays(2);
                    //    cell = GetCell(time);
                    //    location += cell.Width;

                    //    _CellsPresenter.Children.Add(cell);
                    //}
                    //else
                    //{
                        time = time.AddType(TimeUnit, 1d);
                    //}

                }
            }
        }

        private TimespanHeaderCell GetCell(DateTime time)
        {
            TimespanHeaderCell cell = new TimespanHeaderCell();
            if (ParentTimespanHeader.Calendar != null && TimeUnit == TimeUnits.Days)
            {
                cell.IsWorking = ParentTimespanHeader.Calendar.IsWorking(time);
            }
            cell.ParentRow = this;
            cell.DateTime = time;
            cell.Width = ParentTimespanHeader.GetWidth(time, TimeUnit);
            cell.Format = CellFormat;
            cell.Background = CellBackground;
            cell.BorderBrush = CellBorderBrush;
            cell.BorderThickness = CellBorderThickness;
            cell.HorizontalContentAlignment = CellHorizontalAlignment;
            cell.VerticalContentAlignment = CellVerticalAlignment;
            return cell;
        }


        //internal void IncreaseScope()
        //{
        //    if (this.TimeUnit != TimeUnits.Years)
        //    {
               
        //        if (Visibility == Visibility.Visible)
        //            this.TimeUnit++;

        //        //this.Visibility = Visibility.Visible;
        //        ResetFormatting();
        //    }

        //}

        private void ResetFormatting()
        {
            switch (TimeUnit)
            {
                case TimeUnits.Years:
                    CellFormat = "yyyy";
                    break;
                case TimeUnits.Months:
                    if (ParentTimespanHeader != null && ParentTimespanHeader.RowsPresenter.Children.IndexOf(this) == 0)
                        CellFormat = "MMMM yyyy";
                    else
                        CellFormat = "MMM";

                    break;
                case TimeUnits.Weeks:
                    CellFormat = "WEEK";
                    break;
                case TimeUnits.Days:
                    if (ParentTimespanHeader != null && ParentTimespanHeader.RowsPresenter.Children.IndexOf(this) == 0)
                        CellFormat = "ddd MMM dd, yyyy";
                    else
                        CellFormat = "dd";
                    break;
                case TimeUnits.Hours:
                    CellFormat = "HH";
                    break;
                
            }
        }

        //internal void DecreaseScope()
        //{
        //    if (this.TimeUnit != TimeUnits.Days)
        //    {
               
        //        if(Visibility == Visibility.Visible)
        //            this.TimeUnit--;    

        //        //this.Visibility = Visibility.Visible;
        //        ResetFormatting();
        //    }
        //}
    }
}
