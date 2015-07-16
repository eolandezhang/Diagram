using System;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using QPP.Wpf.UI.Native;
using System.ComponentModel;
using System.Collections;

namespace QPP.Wpf.UI.Controls.Metro
{
    [TemplatePart(Name = PART_TitleBar, Type = typeof(UIElement))]
    [TemplatePart(Name = PART_WindowCommands, Type = typeof(WindowCommands))]
    public class MetroWindow : Window
    {
        private const string PART_TitleBar = "PART_TitleBar";
        private const string PART_WindowCommands = "PART_WindowCommands";
        //bool isLoad;
        bool isDragging;
        ContentPresenter WindowCommandsPresenter;
        UIElement titleBar;

        public static readonly DependencyProperty ShowIconOnTitleBarProperty;
        public static readonly DependencyProperty ShowTitleBarProperty;
        public static readonly DependencyProperty ShowMinButtonProperty;
        public static readonly DependencyProperty ShowCloseButtonProperty;
        public static readonly DependencyProperty ShowMaxRestoreButtonProperty;
        public static readonly DependencyProperty TitlebarHeightProperty;
        public static readonly DependencyProperty TitleCapsProperty;
        public static readonly DependencyProperty SaveWindowPositionProperty;
        public static readonly DependencyProperty WindowPlacementSettingsProperty;
        public static readonly DependencyProperty TitleForegroundProperty;
        public static readonly DependencyProperty IgnoreTaskbarOnMaximizeProperty;
        public static readonly DependencyProperty GlowBrushProperty;
        public static readonly DependencyProperty FlyoutsProperty;
        public static readonly DependencyProperty WindowTransitionsEnabledProperty;

        //public static readonly DependencyProperty LoadEventProperty;
        //public static readonly DependencyProperty SyncLoadEventProperty;
        //public static readonly DependencyProperty ClosingEventProperty;
        //public static readonly DependencyProperty ClosedEventProperty;
        public static readonly DependencyProperty MainMenuItemsProperty;
        static readonly DependencyPropertyKey MainMenuItemsPropertyKey;

        static MetroWindow()
        {
            var thisType = typeof(MetroWindow);
            DefaultStyleKeyProperty.OverrideMetadata(thisType, new FrameworkPropertyMetadata(thisType));
            ShowIconOnTitleBarProperty = DependencyProperty.Register("ShowIconOnTitleBar", typeof(bool), thisType, new PropertyMetadata(true));
            ShowMinButtonProperty = DependencyProperty.Register("ShowMinButton", typeof(bool), thisType, new PropertyMetadata(true));
            ShowCloseButtonProperty = DependencyProperty.Register("ShowCloseButton", typeof(bool), thisType, new PropertyMetadata(true));
            ShowMaxRestoreButtonProperty = DependencyProperty.Register("ShowMaxRestoreButton", typeof(bool), thisType, new PropertyMetadata(true));
            TitlebarHeightProperty = DependencyProperty.Register("TitlebarHeight", typeof(int), thisType, new PropertyMetadata(26));
            TitleCapsProperty = DependencyProperty.Register("TitleCaps", typeof(bool), thisType, new PropertyMetadata(true));
            SaveWindowPositionProperty = DependencyProperty.Register("SaveWindowPosition", typeof(bool), thisType, new PropertyMetadata(false));
            WindowPlacementSettingsProperty = DependencyProperty.Register("WindowPlacementSettings", typeof(IWindowPlacementSettings), thisType, new PropertyMetadata(null));
            TitleForegroundProperty = DependencyProperty.Register("TitleForeground", typeof(Brush), thisType);
            IgnoreTaskbarOnMaximizeProperty = DependencyProperty.Register("IgnoreTaskbarOnMaximize", typeof(bool), thisType, new PropertyMetadata(false));
            GlowBrushProperty = DependencyProperty.Register("GlowBrush", typeof(SolidColorBrush), thisType, new PropertyMetadata(null));
            FlyoutsProperty = DependencyProperty.Register("Flyouts", typeof(FlyoutsControl), thisType, new PropertyMetadata(null));
            WindowTransitionsEnabledProperty = DependencyProperty.Register("WindowTransitionsEnabled", typeof(bool), thisType, new PropertyMetadata(true));

            //LoadEventProperty = DependencyProperty.Register("LoadEvent", typeof(EventHandler), thisType, new FrameworkPropertyMetadata(null));
            //SyncLoadEventProperty = DependencyProperty.Register("SyncLoadEvent", typeof(EventHandler), thisType, new FrameworkPropertyMetadata(null));
            //ClosingEventProperty = DependencyProperty.Register("ClosingEvent", typeof(CancelEventHandler), thisType, new FrameworkPropertyMetadata(null));
            //ClosedEventProperty = DependencyProperty.Register("ClosedEvent", typeof(EventHandler), thisType, new FrameworkPropertyMetadata(null));
            MainMenuItemsPropertyKey = DependencyProperty.RegisterReadOnly("MainMenuItems", typeof(ObservableCollection<object>), thisType, new FrameworkPropertyMetadata(null));
            MainMenuItemsProperty = MainMenuItemsPropertyKey.DependencyProperty;
            if (Util.IsDesignMode)
                ShowTitleBarProperty = DependencyProperty.Register("ShowTitleBar", typeof(bool), thisType, new PropertyMetadata(false));
            else
                ShowTitleBarProperty = DependencyProperty.Register("ShowTitleBar", typeof(bool), thisType, new PropertyMetadata(true));
        }

        public ObservableCollection<object> MainMenuItems
        {
            get { return (ObservableCollection<object>)GetValue(MainMenuItemsProperty); }
        }

        ///// <summary>
        ///// 异步加载
        ///// </summary>
        //[Category("Action"), Localizability(LocalizationCategory.NeverLocalize), Bindable(true)]
        //public EventHandler LoadEvent
        //{
        //    get { return (EventHandler)GetValue(LoadEventProperty); }
        //    set { SetValue(LoadEventProperty, value); }
        //}

        ///// <summary>
        ///// 同步加载
        ///// </summary>
        //[Category("Action"), Localizability(LocalizationCategory.NeverLocalize), Bindable(true)]
        //public EventHandler SyncLoadEvent
        //{
        //    get { return (EventHandler)GetValue(SyncLoadEventProperty); }
        //    set { SetValue(SyncLoadEventProperty, value); }
        //}

        //[Category("Action"), Localizability(LocalizationCategory.NeverLocalize), Bindable(true)]
        //public CancelEventHandler ClosingEvent
        //{
        //    get { return (CancelEventHandler)GetValue(ClosingEventProperty); }
        //    set { SetValue(ClosingEventProperty, value); }
        //}

        //[Category("Action"), Localizability(LocalizationCategory.NeverLocalize), Bindable(true)]
        //public EventHandler ClosedEvent
        //{
        //    get { return (EventHandler)GetValue(ClosedEventProperty); }
        //    set { SetValue(ClosedEventProperty, value); }
        //}

        public bool WindowTransitionsEnabled
        {
            get { return (bool)this.GetValue(WindowTransitionsEnabledProperty); }
            set { SetValue(WindowTransitionsEnabledProperty, value); }
        }

        public FlyoutsControl Flyouts
        {
            get { return (FlyoutsControl)GetValue(FlyoutsProperty); }
            set { SetValue(FlyoutsProperty, value); }
        }

        public WindowCommands WindowCommands { get; set; }

        public bool IgnoreTaskbarOnMaximize
        {
            get { return (bool)this.GetValue(IgnoreTaskbarOnMaximizeProperty); }
            set { SetValue(IgnoreTaskbarOnMaximizeProperty, value); }
        }

        public Brush TitleForeground
        {
            get { return (Brush)GetValue(TitleForegroundProperty); }
            set { SetValue(TitleForegroundProperty, value); }
        }

        public bool SaveWindowPosition
        {
            get { return (bool)GetValue(SaveWindowPositionProperty); }
            set { SetValue(SaveWindowPositionProperty, value); }
        }

        public IWindowPlacementSettings WindowPlacementSettings
        {
            get { return (IWindowPlacementSettings)GetValue(WindowPlacementSettingsProperty); }
            set { SetValue(WindowPlacementSettingsProperty, value); }
        }

        public bool ShowIconOnTitleBar
        {
            get { return (bool)GetValue(ShowIconOnTitleBarProperty); }
            set { SetValue(ShowIconOnTitleBarProperty, value); }
        }

        public bool ShowTitleBar
        {
            get { return (bool)GetValue(ShowTitleBarProperty); }
            set { SetValue(ShowTitleBarProperty, value); }
        }

        public bool ShowMinButton
        {
            get { return (bool)GetValue(ShowMinButtonProperty); }
            set { SetValue(ShowMinButtonProperty, value); }
        }

        public bool ShowCloseButton
        {
            get { return (bool)GetValue(ShowCloseButtonProperty); }
            set { SetValue(ShowCloseButtonProperty, value); }
        }

        public int TitlebarHeight
        {
            get { return (int)GetValue(TitlebarHeightProperty); }
            set { SetValue(TitlebarHeightProperty, value); }
        }

        public bool ShowMaxRestoreButton
        {
            get { return (bool)GetValue(ShowMaxRestoreButtonProperty); }
            set { SetValue(ShowMaxRestoreButtonProperty, value); }
        }

        public bool TitleCaps
        {
            get { return (bool)GetValue(TitleCapsProperty); }
            set { SetValue(TitleCapsProperty, value); }
        }

        public SolidColorBrush GlowBrush
        {
            get { return (SolidColorBrush)GetValue(GlowBrushProperty); }
            set { SetValue(GlowBrushProperty, value); }
        }

        public string WindowTitle
        {
            get { return TitleCaps ? Title.ToUpper() : Title; }
        }

        public MetroWindow()
        {
            SetValue(MainMenuItemsPropertyKey, new ObservableCollection<object>());
            Loaded += this.MetroWindow_Loaded;
            //Closing += new CancelEventHandler(MetroWindow_Closing);
            //Closed += new EventHandler(MetroWindow_Closed);
        }

        //void MetroWindow_Closed(object sender, EventArgs e)
        //{
        //    if (ClosedEvent != null)
        //        ClosedEvent(this, e);
        //}

        //void MetroWindow_Closing(object sender, CancelEventArgs e)
        //{
        //    if (ClosingEvent != null)
        //        ClosingEvent(this, e);
        //}

        private void MetroWindow_Loaded(object sender, RoutedEventArgs e)
        {
            VisualStateManager.GoToState(this, "AfterLoaded", true);
            if (!ShowTitleBar)
            {
                //Disables the system menu for reasons other than clicking an invisible titlebar.
                IntPtr handle = new WindowInteropHelper(this).Handle;
                UnsafeNativeMethods.SetWindowLong(handle, UnsafeNativeMethods.GWL_STYLE, UnsafeNativeMethods.GetWindowLong(handle, UnsafeNativeMethods.GWL_STYLE) & ~UnsafeNativeMethods.WS_SYSMENU);
            }

            if (this.Flyouts == null)
            {
                this.Flyouts = new FlyoutsControl();
            }

            //if (!isLoad)
            //{
            //    isLoad = true;
            //    if (LoadEvent != null)
            //        Dispatcher.BeginInvoke(System.Windows.Threading.DispatcherPriority.Loaded,
            //            new Action(() =>
            //            {
            //                LoadEvent(this, e);
            //            }));
            //    if (SyncLoadEvent != null)
            //        SyncLoadEvent(this, e);
            //}
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            if (WindowCommands == null)
                WindowCommands = new WindowCommands();

            titleBar = GetTemplateChild(PART_TitleBar) as UIElement;

            if (titleBar != null && titleBar.Visibility == System.Windows.Visibility.Visible)
            {
                titleBar.MouseDown += TitleBarMouseDown;
                titleBar.MouseUp += TitleBarMouseUp;
                titleBar.MouseMove += TitleBarMouseMove;
            }
            else
            {
                MouseDown += TitleBarMouseDown;
                MouseUp += TitleBarMouseUp;
                MouseMove += TitleBarMouseMove;
            }

            WindowCommandsPresenter = GetTemplateChild("PART_WindowCommands") as ContentPresenter;
        }

        protected override void OnStateChanged(EventArgs e)
        {
            if (WindowCommands != null)
            {
                WindowCommands.RefreshMaximiseIconState();
            }

            base.OnStateChanged(e);
        }

        protected virtual void TitleBarMouseDown(object sender, MouseButtonEventArgs e)
        {
            var mousePosition = e.GetPosition(this);
            bool isIconClick = ShowIconOnTitleBar && mousePosition.X <= TitlebarHeight && mousePosition.Y <= TitlebarHeight;

            if (e.ChangedButton == MouseButton.Left)
            {
                if (isIconClick)
                {
                    if (e.ClickCount == 2)
                    {
                        Close();
                    }
                    else
                    {
                        ShowSystemMenuPhysicalCoordinates(this, PointToScreen(new Point(0, TitlebarHeight)));
                    }
                }
                else
                {
                    IntPtr windowHandle = new WindowInteropHelper(this).Handle;
                    UnsafeNativeMethods.ReleaseCapture();

                    var mPoint = Mouse.GetPosition(this);
                    var wpfPoint = PointToScreen(mPoint);
                    short x = Convert.ToInt16(wpfPoint.X);
                    short y = Convert.ToInt16(wpfPoint.Y);

                    int lParam = (ushort)x | (y << 16);

                    UnsafeNativeMethods.SendMessage(windowHandle, Constants.WM_NCLBUTTONDOWN, Constants.HT_CAPTION, lParam);
                    if (e.ClickCount == 2 && (ResizeMode == ResizeMode.CanResizeWithGrip || ResizeMode == ResizeMode.CanResize) && mPoint.Y <= TitlebarHeight)
                    {
                        WindowState = WindowState == WindowState.Maximized ? WindowState.Normal : WindowState.Maximized;
                    }
                }
            }
            else if (e.ChangedButton == MouseButton.Right)
            {
                ShowSystemMenuPhysicalCoordinates(this, PointToScreen(mousePosition));
            }
        }

        protected void TitleBarMouseUp(object sender, MouseButtonEventArgs e)
        {
            isDragging = false;
        }

        private void TitleBarMouseMove(object sender, MouseEventArgs e)
        {
            if (e.LeftButton != MouseButtonState.Pressed)
            {
                isDragging = false;
            }

            if (isDragging
                && WindowState == WindowState.Maximized
                && ResizeMode != ResizeMode.NoResize)
            {
                // Calculating correct left coordinate for multi-screen system.
                Point mouseAbsolute = PointToScreen(Mouse.GetPosition(this));
                double width = RestoreBounds.Width;
                double left = mouseAbsolute.X - width / 2;

                // Check if the mouse is at the top of the screen if TitleBar is not visible
                if (!(titleBar.Visibility == System.Windows.Visibility.Visible) && mouseAbsolute.Y > TitlebarHeight)
                    return;

                // Aligning window's position to fit the screen.
                double virtualScreenWidth = SystemParameters.VirtualScreenWidth;
                left = left + width > virtualScreenWidth ? virtualScreenWidth - width : left;

                var mousePosition = e.MouseDevice.GetPosition(this);

                // When dragging the window down at the very top of the border,
                // move the window a bit upwards to avoid showing the resize handle as soon as the mouse button is released
                //Top = mousePosition.Y < 5 ? -5 : mouseAbsolute.Y - mousePosition.Y;
                //Left = left;

                // Restore window to normal state.
                WindowState = WindowState.Normal;
            }
        }

        internal T GetPart<T>(string name) where T : DependencyObject
        {
            return (T)GetTemplateChild(name);
        }

        private static void ShowSystemMenuPhysicalCoordinates(Window window, Point physicalScreenLocation)
        {
            if (window == null) return;

            var hwnd = new WindowInteropHelper(window).Handle;
            if (hwnd == IntPtr.Zero || !UnsafeNativeMethods.IsWindow(hwnd))
                return;

            var hmenu = UnsafeNativeMethods.GetSystemMenu(hwnd, false);

            var cmd = UnsafeNativeMethods.TrackPopupMenuEx(hmenu, Constants.TPM_LEFTBUTTON | Constants.TPM_RETURNCMD, (int)physicalScreenLocation.X, (int)physicalScreenLocation.Y, hwnd, IntPtr.Zero);
            if (0 != cmd)
                UnsafeNativeMethods.PostMessage(hwnd, Constants.SYSCOMMAND, new IntPtr(cmd), IntPtr.Zero);
        }

        internal void HandleFlyoutStatusChange(Flyout flyout)
        {
            if (flyout.Position == Position.Right && flyout.IsOpen)
                WindowCommandsPresenter.SetValue(Panel.ZIndexProperty, 3);
            else
                WindowCommandsPresenter.SetValue(Panel.ZIndexProperty, 1); //in the style, the default is 1
        }
    }
}