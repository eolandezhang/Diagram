using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;

namespace QPP.Wpf.UI.Controls.Toolkit
{
    public class HyperlinkButton : ButtonBase
    {
        static HyperlinkButton()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(HyperlinkButton), new FrameworkPropertyMetadata(typeof(HyperlinkButton)));
        }

        public string NavigateUri
        {
            get { return (string)GetValue(NavigateUriProperty); }
            set { SetValue(NavigateUriProperty, value); }
        }

        public static readonly DependencyProperty NavigateUriProperty =
            DependencyProperty.Register("NavigateUri", typeof(string), typeof(HyperlinkButton), new PropertyMetadata());        
    }
}
