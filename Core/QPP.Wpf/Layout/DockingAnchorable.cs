using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Controls;
using System.Windows;
using System.ComponentModel;
using QPP.Layout;

namespace QPP.Wpf.Layout
{
    public class DockingAnchorable : DockingContent, IDockingAnchorable
    {
        public static readonly DependencyProperty AutoHideHeightProperty =
            DependencyProperty.Register("AutoHideHeight", typeof(double), 
            typeof(DockingAnchorable), new FrameworkPropertyMetadata(200d));

        public static readonly DependencyProperty AutoHideWidthProperty =
            DependencyProperty.Register("AutoHideWidth", typeof(double), 
            typeof(DockingAnchorable), new FrameworkPropertyMetadata(200d));
        
        public static readonly DependencyProperty DockAreaProperty =
            DependencyProperty.Register("DockArea", typeof(DockAreas),
            typeof(DockingAnchorable), new FrameworkPropertyMetadata(DockAreas.Document));        

        public double AutoHideHeight
        {
            get { return (double)GetValue(AutoHideHeightProperty); }
            set { SetValue(AutoHideHeightProperty, value); }
        }

        public double AutoHideWidth
        {
            get { return (double)GetValue(AutoHideWidthProperty); }
            set { SetValue(AutoHideWidthProperty, value); }
        }

        public DockAreas DockArea
        {
            get { return (DockAreas)GetValue(DockAreaProperty); }
            set { SetValue(DockAreaProperty, value); }
        }
    }
}
