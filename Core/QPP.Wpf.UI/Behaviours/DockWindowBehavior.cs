using QPP.Wpf.UI.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows;
using System.Windows.Input;
using System.Windows.Interactivity;
using System.Windows.Media.Animation;
using System.Windows.Threading;

namespace QPP.Wpf.UI.Behaviours
{
    public class DockWindowBehavior : Behavior<Window>
    {
        public static readonly DependencyProperty ShowActionProperty;

        static DockWindowBehavior()
        {
            ShowActionProperty = DependencyProperty.Register("ShowAction", typeof(Action), typeof(DockWindowBehavior), new FrameworkPropertyMetadata(null));
        }

        public Action ShowAction
        {
            get { return (Action)GetValue(ShowActionProperty); }
            set { SetValue(ShowActionProperty, value); }
        }

        enum Location { None, Top, LeftTop, RightTop }

        const int BORDER = 3;
        const int AUTO_HIDE_TIME = 50;
        const int ANIMATION_DURATION = 100;

        Location m_Location;
        bool m_IsHidded;
        bool m_IsAnimating;
        DispatcherTimer m_AutoHideTimer;
        Storyboard m_FadeInStoryboard;
        Storyboard m_FadeOutStoryboard;

        protected override void OnAttached()
        {
            m_AutoHideTimer = new DispatcherTimer();
            m_AutoHideTimer.Interval = TimeSpan.FromMilliseconds(AUTO_HIDE_TIME);
            m_AutoHideTimer.Tick += new EventHandler(AutoHideTimer_Tick);

            AssociatedObject.StateChanged += AssociatedObject_StateChanged;
            AssociatedObject.Deactivated += AssociatedObject_Deactivated;
            AssociatedObject.LocationChanged += AssociatedObject_LocationChanged;
            //AssociatedObject.IsVisibleChanged += AssociatedObject_IsVisibleChanged;
            InitStoryboard();
            ShowAction = () =>
            { 
                if (this.m_IsHidded)
                {
                    ShowWindow();
                    m_Location = Location.None;
                }
                AssociatedObject.Activate();
            };
            base.OnAttached();
        }

        void AssociatedObject_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if ((bool)e.NewValue)
                if (this.m_IsHidded)
                {
                    ShowWindow();
                    m_Location = Location.None;
                }
        }

        void AssociatedObject_LocationChanged(object sender, EventArgs e)
        {
            UpdateLocation();
        }

        void AssociatedObject_Deactivated(object sender, EventArgs e)
        {
            UpdateLocation();
        }

        void UpdateLocation()
        {
            //System.Diagnostics.Debug.WriteLine("UpdateLocation[Top:{0},Left:{1}]".FormatArgs(Top, Left));
            if (m_IsAnimating) return;
            if (!m_IsHidded)
            {
                if (AssociatedObject.Top <= 0 && AssociatedObject.Left <= 0)
                {
                    m_Location = Location.LeftTop;
                    AssociatedObject.Topmost = true;
                    HideWindow();
                }
                else if (AssociatedObject.Top <= 0 && AssociatedObject.Left >= SystemParameters.VirtualScreenWidth - AssociatedObject.ActualWidth)
                {
                    m_Location = Location.RightTop;
                    AssociatedObject.Topmost = true;
                    HideWindow();
                }
                else if (AssociatedObject.Top <= 0)
                {
                    m_Location = Location.Top;
                    AssociatedObject.Topmost = true;
                    HideWindow();
                }
                else
                {
                    AssociatedObject.Topmost = false;
                    m_Location = Location.None;
                }
                //System.Diagnostics.Debug.WriteLine("UpdateLocation[Location:{0}]".FormatArgs(m_Location.ToString()));
            }
        }

        void InitStoryboard()
        {
            m_FadeInStoryboard = new Storyboard();
            m_FadeOutStoryboard = new Storyboard();
            m_FadeInStoryboard.Completed += (s, e) =>
            {
                m_FadeInStoryboard.Remove();
                AssociatedObject.Top = 0;
                m_IsHidded = false;
                AssociatedObject.UpdateLayout();
                m_IsAnimating = false;
                m_AutoHideTimer.Start();
            };
            m_FadeOutStoryboard.Completed += (s, e) =>
            {
                m_FadeOutStoryboard.Remove();
                AssociatedObject.Top = BORDER - AssociatedObject.ActualHeight;
                m_IsHidded = true;
                m_IsAnimating = false;
                m_AutoHideTimer.Start();
            };
        }

        void AssociatedObject_StateChanged(object sender, EventArgs e)
        {
            if (AssociatedObject.WindowState == System.Windows.WindowState.Normal)
            {
                if (this.m_IsHidded)
                    ShowWindow();
            }
            //if (AssociatedObject.WindowState == System.Windows.WindowState.Maximized)
            //{
            //    AssociatedObject.WindowState = System.Windows.WindowState.Normal;
            //}
            //if (AssociatedObject.WindowState == System.Windows.WindowState.Minimized)
            //{
            //    AssociatedObject.WindowState = System.Windows.WindowState.Normal;
            //    AssociatedObject.Hide();
            //}
        }

        void AutoHideTimer_Tick(object sender, EventArgs e)
        {
            POINT p;
            if (!GetCursorPos(out p)) return;

            if (p.x >= AssociatedObject.Left && p.x <= (AssociatedObject.Left + AssociatedObject.ActualWidth)
                && p.y >= AssociatedObject.Top && p.y <= (AssociatedObject.Top + AssociatedObject.ActualHeight))
            {
                ShowWindow();
            }
            else
            {
                HideWindow();
            }
        }

        void FadeIn()
        {
            m_IsAnimating = true;
            m_AutoHideTimer.Stop();
            m_FadeInStoryboard.Children.Clear();
            var animation = new DoubleAnimation(AssociatedObject.Top, 0, new Duration(TimeSpan.FromMilliseconds(ANIMATION_DURATION)));
            animation.SetValue(Storyboard.TargetProperty, AssociatedObject);
            animation.SetValue(Storyboard.TargetPropertyProperty, new PropertyPath("Top"));
            m_FadeInStoryboard.Children.Add(animation);
            m_FadeInStoryboard.Begin();
        }

        void FadeOut()
        {
            m_IsAnimating = true;
            m_AutoHideTimer.Stop();
            var animation = new DoubleAnimation(AssociatedObject.Top, BORDER - AssociatedObject.ActualHeight, new Duration(TimeSpan.FromMilliseconds(ANIMATION_DURATION)));
            animation.SetValue(Storyboard.TargetProperty, AssociatedObject);
            animation.SetValue(Storyboard.TargetPropertyProperty, new PropertyPath("Top"));
            m_FadeOutStoryboard.Children.Add(animation);
            m_FadeOutStoryboard.Begin();
        }

        void ShowWindow()
        {
            if (m_IsAnimating) return;
            if (m_IsHidded)
            {
                switch (this.m_Location)
                {
                    case Location.Top:
                    case Location.LeftTop:
                    case Location.RightTop:
                        FadeIn();
                        break;
                    case Location.None:
                        break;
                }
            }
        }

        void HideWindow()
        {
            if (m_IsAnimating) return;
            if (!m_IsHidded && Mouse.LeftButton == MouseButtonState.Released)
            {
                switch (this.m_Location)
                {
                    case Location.Top:
                        FadeOut();
                        break;
                    case Location.LeftTop:
                        AssociatedObject.Left = 0;
                        FadeOut();
                        break;
                    case Location.RightTop:
                        AssociatedObject.Left = SystemParameters.VirtualScreenWidth - AssociatedObject.ActualWidth;
                        FadeOut();
                        break;
                    case Location.None:
                        break;
                }
            }
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct POINT
        {
            public int x;
            public int y;
            public POINT(int x, int y)
            {
                this.x = x;
                this.y = y;
            }
        }

        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        public static extern bool GetCursorPos(out POINT pt);
    }
}
