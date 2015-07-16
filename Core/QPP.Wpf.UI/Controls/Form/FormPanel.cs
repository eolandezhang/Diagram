using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;

namespace QPP.Wpf.UI.Controls.Form
{
    public class FormPanel : Panel
    {
        public static readonly DependencyProperty LabelVisibilityProperty;
        public static readonly DependencyProperty LabelProperty;
        public static readonly DependencyProperty LabelWidthProperty;
        public static readonly DependencyProperty LabelHorizontalAlignmentProperty;
        public static readonly DependencyProperty LabelVerticalAlignmentProperty;

        public static readonly DependencyProperty ColumnSpanProperty;
        public static readonly DependencyProperty ColumnWidthProperty;
        public static readonly DependencyProperty ColumnMinWidthProperty;
        public static readonly DependencyProperty ColumnMaxWidthProperty;
        public static readonly DependencyProperty RowSpanProperty;
        public static readonly DependencyProperty RowHeightProperty;

        public static readonly DependencyProperty ColumnsProperty;
        public static readonly DependencyProperty DisplayIndexProperty;
        public static readonly DependencyProperty CellMarginProperty;

        static FormPanel()
        {
            Type thisType = typeof(FormView);
            //LabelProperty = DependencyProperty.RegisterAttached("Label", typeof(object), thisType, new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.AffectsMeasure));
            //LabelVisibilityProperty = DependencyProperty.RegisterAttached("LabelVisibility", typeof(Visibility), thisType, new FrameworkPropertyMetadata(true, FrameworkPropertyMetadataOptions.AffectsMeasure));
            //LabelWidthProperty = DependencyProperty.Register("LabelWidth", typeof(GridLength), thisType, new FrameworkPropertyMetadata(GridLength.Auto, FrameworkPropertyMetadataOptions.AffectsMeasure));
            //LabelHorizontalAlignmentProperty = DependencyProperty.Register("LabelHorizontalAlignment", typeof(HorizontalAlignment), thisType, new FrameworkPropertyMetadata(HorizontalAlignment.Right, FrameworkPropertyMetadataOptions.AffectsArrange));
            //LabelVerticalAlignmentProperty = DependencyProperty.Register("LabelVerticalAlignment", typeof(VerticalAlignment), thisType, new FrameworkPropertyMetadata(VerticalAlignment.Center, FrameworkPropertyMetadataOptions.AffectsArrange));

            //DisplayIndexProperty = DependencyProperty.RegisterAttached("DisplayIndex", typeof(int), thisType, new FrameworkPropertyMetadata(-1, FrameworkPropertyMetadataOptions.AffectsArrange));
            //RowSpanProperty = DependencyProperty.RegisterAttached("RowSpan", typeof(int), thisType, new FrameworkPropertyMetadata(1, FrameworkPropertyMetadataOptions.AffectsArrange, OnCoerceValue));
            //ColumnSpanProperty = DependencyProperty.RegisterAttached("ColumnSpan", typeof(int), thisType, new FrameworkPropertyMetadata(1, FrameworkPropertyMetadataOptions.AffectsArrange, OnCoerceValue));
            //RowHeightProperty = DependencyProperty.RegisterAttached("RowHeight", typeof(GridLength), thisType, new FrameworkPropertyMetadata(new GridLength(-1), FrameworkPropertyMetadataOptions.AffectsArrange));

            //ColumnsProperty = DependencyProperty.Register("Columns", typeof(int), thisType, new FrameworkPropertyMetadata(2, FrameworkPropertyMetadataOptions.AffectsArrange, OnCoerceValue));
            //DefaultRowHeightProperty = DependencyProperty.Register("DefaultRowHeight", typeof(double), thisType, new FrameworkPropertyMetadata(24d, FrameworkPropertyMetadataOptions.AffectsArrange));
            //ControlWidthProperty = DependencyProperty.Register("ControlWidth", typeof(GridLength), thisType, new FrameworkPropertyMetadata(new GridLength(1, GridUnitType.Star), FrameworkPropertyMetadataOptions.AffectsArrange));
            //ControlMaxWidthProperty = DependencyProperty.Register("ControlMaxWidth", typeof(double), thisType, new FrameworkPropertyMetadata(300d, FrameworkPropertyMetadataOptions.AffectsArrange));
            //RowSpacingProperty = DependencyProperty.Register("RowSpacing", typeof(double), thisType, new FrameworkPropertyMetadata(6.0, FrameworkPropertyMetadataOptions.AffectsArrange));
            //ColumnSpacingProperty = DependencyProperty.Register("ColumnSpacing", typeof(double), thisType, new FrameworkPropertyMetadata(15.0, FrameworkPropertyMetadataOptions.AffectsArrange));
        }

        public static void SetLabel(DependencyObject obj, object value)
        {
            obj.SetValue(LabelProperty, value);
        }

        public static object GetLabel(DependencyObject obj)
        {
            return obj.GetValue(LabelProperty);
        }

        public static void SetDisplayIndex(DependencyObject obj, int value)
        {
            obj.SetValue(DisplayIndexProperty, value);
        }

        public static int GetDisplayIndex(DependencyObject obj)
        {
            return (int)obj.GetValue(DisplayIndexProperty);
        }

        public static void SetRowSpan(DependencyObject obj, int value)
        {
            obj.SetValue(RowSpanProperty, value);
        }

        public static int GetRowSpan(DependencyObject obj)
        {
            return (int)obj.GetValue(RowSpanProperty);
        }

        //public static void SetShowHeader(DependencyObject obj, bool value)
        //{
        //    obj.SetValue(ShowHeaderProperty, value);
        //}

        //public static bool GetShowHeader(DependencyObject obj)
        //{
        //    return (bool)obj.GetValue(ShowHeaderProperty);
        //}

        public static void SetColumnSpan(DependencyObject obj, int value)
        {
            obj.SetValue(ColumnSpanProperty, value);
        }

        public static int GetColumnSpan(DependencyObject obj)
        {
            return (int)obj.GetValue(ColumnSpanProperty);
        }

        public static void SetRowHeight(DependencyObject obj, GridLength value)
        {
            obj.SetValue(RowHeightProperty, value);
        }

        public static GridLength GetRowHeight(DependencyObject obj)
        {
            return (GridLength)obj.GetValue(RowHeightProperty);
        }

        public int Columns
        {
            get { return (int)GetValue(ColumnsProperty); }
            set { SetValue(ColumnsProperty, value); }
        }

        public GridLength LabelWidth
        {
            get { return (GridLength)GetValue(LabelWidthProperty); }
            set { SetValue(LabelWidthProperty, value); }
        }

        //public GridLength ControlWidth
        //{
        //    get { return (GridLength)GetValue(ControlWidthProperty); }
        //    set { SetValue(ControlWidthProperty, value); }
        //}

        //public double ControlMaxWidth
        //{
        //    get { return (double)GetValue(ControlMaxWidthProperty); }
        //    set { SetValue(ControlMaxWidthProperty, value); }
        //}

        //public double DefaultRowHeight
        //{
        //    get { return (double)GetValue(DefaultRowHeightProperty); }
        //    set { SetValue(DefaultRowHeightProperty, value); }
        //}

        //public double ColumnSpacing
        //{
        //    get { return (double)GetValue(ColumnSpacingProperty); }
        //    set { SetValue(ColumnSpacingProperty, value); }
        //}

        //public double RowSpacing
        //{
        //    get { return (double)GetValue(RowSpacingProperty); }
        //    set { SetValue(RowSpacingProperty, value); }
        //}

        public HorizontalAlignment LabelHorizontalAlignment
        {
            get { return (HorizontalAlignment)GetValue(LabelHorizontalAlignmentProperty); }
            set { SetValue(LabelHorizontalAlignmentProperty, value); }
        }

        public VerticalAlignment LabelVerticalAlignment
        {
            get { return (VerticalAlignment)GetValue(LabelVerticalAlignmentProperty); }
            set { SetValue(LabelVerticalAlignmentProperty, value); }
        }

        static object OnCoerceValue(DependencyObject d, object baseValue)
        {
            var value = (int)baseValue;
            return Math.Max(1, value);
        }
    }
}
