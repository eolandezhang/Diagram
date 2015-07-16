using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Controls;
using System.Windows;
using System.Windows.Input;
using QPP.Wpf.UI.Controls.Toolkit;

namespace QPP.Wpf.UI.Controls.Pager
{
    [TemplatePart(Name = "PART_GotoButton", Type = typeof(Button))]
    [TemplatePart(Name = "PART_GotoNextButton", Type = typeof(Button))]
    [TemplatePart(Name = "PART_GotoPreviousButton", Type = typeof(Button))]
    [TemplatePart(Name = "PART_PageIndex", Type = typeof(NumericUpDown))]
    public class PagerBar : Control
    {
        private NumericUpDown PART_PageIndex;
        private Button PART_GotoNextButton;
        private Button PART_GotoPreviousButton;

        public readonly static DependencyProperty TotalProperty;
        public readonly static DependencyProperty PageSizeProperty;
        public readonly static DependencyProperty CurrentPageProperty;

        static PagerBar()
        {
            var thisType = typeof(PagerBar);
            DefaultStyleKeyProperty.OverrideMetadata(thisType, new FrameworkPropertyMetadata(thisType));

            TotalProperty = DependencyProperty.Register("Total", typeof(int), thisType, new PropertyMetadata(0, OnPropertyChanged, (d, v) => { return Math.Max(0, (int)v); }));
            PageSizeProperty = DependencyProperty.Register("PageSize", typeof(int), thisType, new FrameworkPropertyMetadata(50, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, OnPropertyChanged, (d, v) => { return Math.Max(1, (int)v); }));
            CurrentPageProperty = DependencyProperty.Register("CurrentPage", typeof(int), thisType, new FrameworkPropertyMetadata(1, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, OnPropertyChanged, (d, v) => { return Math.Max(1, (int)v); }));
        }

        static void OnPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var bar = d as PagerBar;
            if (bar.PART_GotoNextButton != null)
                bar.PART_GotoNextButton.IsEnabled = bar.PageSize * bar.CurrentPage < bar.Total;
            if (bar.PART_GotoPreviousButton != null)
                bar.PART_GotoPreviousButton.IsEnabled = bar.CurrentPage > 1;
        }

        public int Total
        {
            get { return (int)GetValue(TotalProperty); }
            set { SetValue(TotalProperty, value); }
        }

        public int PageSize
        {
            get { return (int)GetValue(PageSizeProperty); }
            set { SetValue(PageSizeProperty, value); }
        }

        public int CurrentPage
        {
            get { return (int)GetValue(CurrentPageProperty); }
            set { SetValue(CurrentPageProperty, value); }
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            PART_PageIndex = GetTemplateChild("PART_PageIndex") as NumericUpDown;
            var gotoButton = GetTemplateChild("PART_GotoButton") as Button;
            gotoButton.Click += new RoutedEventHandler(gotoButton_Click);
            PART_GotoNextButton = GetTemplateChild("PART_GotoNextButton") as Button;
            PART_GotoNextButton.Click += (s, e) => { GoToNextPage(); };
            PART_GotoPreviousButton = GetTemplateChild("PART_GotoPreviousButton") as Button;
            PART_GotoPreviousButton.Click += (s, e) => { GoToPreviousPage(); };
        }

        void GoToNextPage()
        {
            if (PageSize * CurrentPage < Total)
                CurrentPage++;
        }

        void GoToPreviousPage()
        {
            if (CurrentPage > 1)
                CurrentPage--;
        }

        void gotoButton_Click(object sender, RoutedEventArgs e)
        {
            if (PART_PageIndex != null)
            {
                var bind = PART_PageIndex.GetBindingExpression(NumericUpDown.ValueProperty);
                bind.UpdateSource();
            }
        }
    }
}
