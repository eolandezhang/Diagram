using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.Windows;
using System.Windows.Threading;

namespace QPP.Wpf
{
    public static class DispatcherExpension
    {
        /// <summary>
        /// 异步刷新UI
        /// </summary>
        /// <param name="app"></param>
        public static void DoEvent(this Dispatcher dispatcher)
        {
            try
            {
                DispatcherFrame frame = new DispatcherFrame();
                dispatcher.BeginInvoke(DispatcherPriority.Background, new DispatcherOperationCallback(ExitFrame), frame);
                Dispatcher.PushFrame(frame);
            }
            catch { }
        }

        static object ExitFrame(object f)
        {
            try
            {
                ((DispatcherFrame)f).Continue = false;
                return null;
            }
            catch { return null; }
        }
    }
}
