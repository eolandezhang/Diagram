using QPP.Layout;
using QPP.Modularity;
using QPP.Runtime;
using QPP.Runtime.Serialization;
using QPP.Wpf.Layout.Serialization;
using QPP.Wpf.UI.Controls.AvalonDock.Layout.Serialization;
using QPP.Wpf.UI.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;
using System.Xml.Serialization;

namespace QPP.Wpf.Layout
{
    public abstract class DockingContent : ContentControl, IDockingContent, IXmlSerializeContract
    {
        public event CancelEventHandler Closing;

        public event EventHandler Closed;

        public event PropertyChangedCallback PropertyChanged;

        public static readonly DependencyProperty TitleProperty;
        public static readonly DependencyProperty FloatingHeightProperty;
        public static readonly DependencyProperty FloatingWidthProperty;
        public static readonly DependencyProperty FloatingTopProperty;
        public static readonly DependencyProperty FloatingLeftProperty;
        public static readonly DependencyProperty IconSourceProperty;

        static DockingContent()
        {
            var thisType = typeof(DockingContent);
            DefaultStyleKeyProperty.OverrideMetadata(typeof(DockingContent), new FrameworkPropertyMetadata(thisType));
            TitleProperty = DependencyProperty.Register("Title", typeof(string), thisType, new PropertyMetadata(OnPropertyChanged));
            FloatingHeightProperty = DependencyProperty.Register("FloatingHeight", typeof(double), thisType, new FrameworkPropertyMetadata(600d));
            FloatingWidthProperty = DependencyProperty.Register("FloatingWidth", typeof(double), thisType, new FrameworkPropertyMetadata(800d));
            FloatingTopProperty = DependencyProperty.Register("FloatingTop", typeof(double), thisType, new FrameworkPropertyMetadata(double.NaN));
            FloatingLeftProperty = DependencyProperty.Register("FloatingLeft", typeof(double), thisType, new FrameworkPropertyMetadata(double.NaN));
            IconSourceProperty = DependencyProperty.Register("IconSource", typeof(ImageSource), thisType, new FrameworkPropertyMetadata(OnPropertyChanged));
        }

        public string ContentKey { get; set; }

        public ImageSource IconSource
        {
            get { return (ImageSource)GetValue(IconSourceProperty); }
            set { SetValue(IconSourceProperty, value); }
        }

        public string Title
        {
            get { return GetValue(TitleProperty) as string; }
            set { SetValue(TitleProperty, value); }
        }

        public double FloatingTop
        {
            get { return (double)GetValue(FloatingTopProperty); }
            set { SetValue(FloatingTopProperty, value); }
        }

        public double FloatingLeft
        {
            get { return (double)GetValue(FloatingLeftProperty); }
            set { SetValue(FloatingLeftProperty, value); }
        }

        public double FloatingHeight
        {
            get { return (double)GetValue(FloatingHeightProperty); }
            set { SetValue(FloatingHeightProperty, value); }
        }

        public double FloatingWidth
        {
            get { return (double)GetValue(FloatingWidthProperty); }
            set { SetValue(FloatingWidthProperty, value); }
        }

        static void OnPropertyChanged(DependencyObject dependency, DependencyPropertyChangedEventArgs e)
        {
            var doc = dependency as DockingContent;
            if (doc.PropertyChanged != null)
                doc.PropertyChanged(dependency, e);
        }

        public void OnClosing(CancelEventArgs e)
        {
            if (Closing != null)
                Closing(this, e);
        }

        public void OnClosed(EventArgs e)
        {
            if (Closed != null)
                Closed(this, e);
        }

        public DockingContent()
        {
            DataContextChanged += new DependencyPropertyChangedEventHandler(Content_DataContextChanged);
        }

        void Content_DataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            ResourceDictionary rd = new ResourceDictionary();
            rd.Add("dataContext", new DataContextSpy() { DataContext = e.NewValue });

            QPP.Wpf.UI.Util.ApplyResourceDictionary(rd, Resources);
        }

        public IXmlSerializer CreateSerializer()
        {
            return new ContentXmlSerializer();
        }

        public IPresenter Presenter
        {
            get { return DataContext as IPresenter; }
            set { DataContext = value; }
        }

        public bool CanSerialize
        {
            get { return Presenter.CanSerialize; }
        }
    }
}

