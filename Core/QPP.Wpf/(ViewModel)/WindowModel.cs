using QPP.ComponentModel;
using System;
using System.Windows;

namespace QPP.Wpf
{
    /// <summary>
    /// 窗口模型
    /// </summary>
    public class WindowModel : ObservableObject
    {
        public Action ActivateAction { get; set; }

        public Action CloseAction { get; set; }

        public WindowModel()
        {
        }

        public Visibility Visibility
        {
            get { return Get<Visibility>("Visibility"); }
            set { Set("Visibility", value); }
        }

        public WindowState WindowState
        {
            get { return Get<WindowState>("WindowState"); }
            set { Set("WindowState", value); }
        }

        public void Activate()
        {
            if (ActivateAction != null)
                ActivateAction();
        }

        public void Close()
        {
            if (CloseAction != null)
                CloseAction();
        }
    }
}
