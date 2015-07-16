using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Controls;
using System.Windows;
using System.Collections;
using System.ComponentModel;
using System.Windows.Input;
using QPP.Wpf.UI.Models;

namespace QPP.Wpf.UI.Controls
{
    public class Content : ContentControl
    {
        //bool isLoad;

        //public static readonly DependencyProperty LoadEventProperty
        //    = DependencyProperty.Register("LoadEvent", typeof(EventHandler), typeof(Content),
        //    new FrameworkPropertyMetadata(null));


        //[Category("Action"), Localizability(LocalizationCategory.NeverLocalize), Bindable(true)]
        //public EventHandler LoadEvent
        //{
        //    get { return (EventHandler)GetValue(LoadEventProperty); }
        //    set { SetValue(LoadEventProperty, value); }
        //}

        static Content()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(Content), new FrameworkPropertyMetadata(typeof(Content)));
        }

        public Content()
        {
            DataContextChanged += new DependencyPropertyChangedEventHandler(Content_DataContextChanged);
            //Loaded += Content_Loaded;
        }

        //void Content_Loaded(object sender, RoutedEventArgs e)
        //{
        //    if (!isLoad)
        //    {
        //        isLoad = true;
        //        if (LoadEvent != null)
        //            LoadEvent(this, e);
        //    }
        //}

        void Content_DataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            ResourceDictionary rd = new ResourceDictionary();
            rd.Add("dataContext", new DataContextSpy() { DataContext = e.NewValue });

            ApplyResourceDictionary(rd, Resources);
        }

        static void ApplyResourceDictionary(ResourceDictionary newRd, ResourceDictionary oldRd)
        {
            foreach (DictionaryEntry r in newRd)
            {
                if (oldRd.Contains(r.Key))
                    oldRd.Remove(r.Key);

                oldRd.Add(r.Key, r.Value);
            }
        }
    }
}
