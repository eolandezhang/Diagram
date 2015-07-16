using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;

namespace QPP.Wpf.UI.Controls
{
    public class WebBrowserHelper: DependencyObject
    {
        public static readonly DependencyProperty HideScrollBarProperty;

        public static readonly DependencyProperty ContentProperty;

        static WebBrowserHelper()
        {
            ContentProperty = DependencyProperty.RegisterAttached("Content", typeof(string), typeof(WebBrowserHelper), new PropertyMetadata(OnContentChanged));
            HideScrollBarProperty = DependencyProperty.RegisterAttached("HideScrollBar", typeof(bool), typeof(WebBrowserHelper), new PropertyMetadata(OnHideScrollBarChanged));
        }

        static void OnHideScrollBarChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var browser = d as WebBrowser;
            if ((bool)e.NewValue)
                browser.LoadCompleted += BrowserLoadCompleted;
            else
                browser.LoadCompleted -= BrowserLoadCompleted;
        }

        static void BrowserLoadCompleted(object sender, System.Windows.Navigation.NavigationEventArgs e)
        {
            string script = "document.body.style.overflow ='hidden'";
            WebBrowser wb = (WebBrowser)sender;
            wb.InvokeScript("execScript", new Object[] { script, "JavaScript" });
        }

        static void OnContentChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var browser = d as WebBrowser;    
            browser.NavigateToString(ConvertExtendedASCII(e.NewValue.ToSafeString()));
        }

        /// <summary>
        /// 转换文本内容,使文本中文在 WebBrowser 中不出现乱码
        /// </summary>
        /// <param name="content">要显示的文本</param>
        /// <returns>转换后的文本</returns>
        static string ConvertExtendedASCII(string content)
        {
            string result = "";
            char[] chars = content.ToCharArray();

            foreach (char c in chars)
            {
                if (Convert.ToInt32(c) > 127)
                    result += "&#" + Convert.ToInt32(c) + ";";
                else
                    result += c;
            }
            return result;
        }

        public static void SetContent(DependencyObject obj, string value)
        {
            obj.SetValue(ContentProperty, value);
        }

        public static string GetContent(DependencyObject obj)
        {
            return (string)obj.GetValue(ContentProperty);
        }



        public static void SetHideScrollBar(DependencyObject obj, bool value)
        {
            obj.SetValue(HideScrollBarProperty, value);
        }

        public static bool GetHideScrollBar(DependencyObject obj)
        {
            return (bool)obj.GetValue(HideScrollBarProperty);
        }
    }
}
