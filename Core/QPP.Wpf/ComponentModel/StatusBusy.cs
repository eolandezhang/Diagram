using System;
using System.Windows.Input;
using QPP.Wpf.UI;
using System.Windows.Threading;
using System.Threading;
using System.Windows;
using QPP.Messaging;

namespace QPP.Wpf.ComponentModel
{
    /// <summary>
    /// 繁忙状态，会把Cursor设置为Cursors.Wait，并提示状态信息
    /// </summary>    
    public class StatusBusy : IDisposable
    {
        Cursor m_Cursor;
        string m_StatusText;
        ViewStatus m_ViewStatus;

        /// <summary>
        /// 繁忙状态
        /// </summary>
        /// <param name="statusText">工作時提供信息</param>
        /// <param name="status">視圖狀態</param>
        /// <param name="endStatusText">工作完成時提供信息</param>
        /// <param name="token"></param>
        public StatusBusy(string statusText, ViewStatus status, string endStatusText = null)
        {
            m_ViewStatus = status;
            m_StatusText = endStatusText;
            m_Cursor = status.Cursor;
            //修改當前ViewModel的狀態，再發消息通知主窗口
            status.StatusText = statusText;
            status.Cursor = Cursors.Wait;
            status.IsBusy = true;
            RuntimeContext.Service.Messenger.Send(new GenericMessage<ViewStatus>(status));
            System.Windows.Application.Current.Dispatcher.DoEvent();
        }

        public void Dispose()
        {
            m_ViewStatus.StatusText = m_StatusText ?? RuntimeContext.Service.L10N.GetText("Status.Ready");
            m_ViewStatus.Cursor = m_Cursor;
            m_ViewStatus.IsBusy = false;
            RuntimeContext.Service.Messenger.Send(new GenericMessage<ViewStatus>(m_ViewStatus));
        }
    }
}
