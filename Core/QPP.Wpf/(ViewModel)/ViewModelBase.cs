using QPP.Command;
using QPP.ComponentModel;
using QPP.Layout;
using QPP.Messaging;
using QPP.Metadata;
using QPP.Modularity;
using QPP.Runtime;
using QPP.Runtime.Serialization;
using QPP.Security;
using QPP.Wpf.Command;
using QPP.Wpf.ComponentModel;
using QPP.Wpf.UI.Controls.AvalonDock.Layout.Serialization;
using QPP.Wpf.UI;
using System;
using System.Collections;
using System.ComponentModel;
using System.Windows.Threading;

namespace QPP.Wpf
{
    /// <summary>
    /// ViewModel基类
    /// </summary>
    public class ViewModelBase : ObservableObject, IPresenter
    {
        Hashtable validatorTable = new Hashtable();
        PresenterMetadata metadata;

        public event EventHandler Loaded;
        public event CancelEventHandler Closing;
        public event EventHandler Closed;

        public ICommandContext CommandContext { get; private set; }

        public PresenterMetadata Metadata { get { return metadata; } }

        /// <summary>
        /// 用於查找Window
        /// </summary>
        public object Source { get; private set; }

        /// <summary>
        /// 是否已加載
        /// </summary>
        protected bool IsLoaded { get; set; }

        #region Properties

        /// <summary>
        /// 视图标题
        /// </summary>
        public string Title
        {
            get { return Get<string>("Title"); }
            set { Set("Title", value); }
        }

        public bool CanSerialize
        {
            get { return Get<bool>("CanSerialize", () => true); }
            set { Set("CanSerialize", value); }
        }

        /// <summary>
        /// 視圖狀態
        /// </summary>
        public ViewStatus ViewStatus
        {
            get { return Get<ViewStatus>("ViewStatus", () => new ViewStatus()); }
            set { Set("ViewStatus", value); }
        }

        #endregion

        public ViewModelBase()
        {
            CommandContext = new CommandContext();
            if (Util.IsDesignMode) return;

            metadata = RuntimeContext.Service.GetObject<IMetadataDescriptor>().GetMetadata(GetType());
            RuntimeContext.Service.L10N.PropertyChanged += (s, e) => { RefreshTitle(); };
            //初始化視圖狀態
            ViewStatus.StatusText = RuntimeContext.Service.L10N.GetText("Status.Ready");
            CommandContext.Init(this);
            Initialize();
            RefreshTitle();
        }

        /// <summary>
        /// 初始化
        /// </summary>
        protected virtual void Initialize()
        {

        }

        #region Events

        /// <summary>
        /// 關閉時事件
        /// </summary>
        [EventCommand(EventCommands.Closing)]
        public ICommand ClosingEventCommand
        {
            get { return CommandContext.GetCommand<CancelEventArgs>(() => ClosingEventCommand, OnClosing); }
        }

        public virtual void OnClosing(CancelEventArgs e)
        {
            if (Closing != null)
                Closing(this, e);
        }

        /// <summary>
        /// 關閉事件
        /// </summary>
        [EventCommand(EventCommands.Closed)]
        public ICommand ClosedEventCommand
        {
            get { return CommandContext.GetCommand<EventArgs>(() => ClosedEventCommand, OnClosed); }
        }

        public virtual void OnClosed(EventArgs e)
        {
            RuntimeContext.Service.Messenger.Unregister(this);
            if (Closed != null)
                Closed(this, EventArgs.Empty);
        }

        /// <summary>
        /// 加載事件
        /// </summary>
        [EventCommand(EventCommands.Loaded)]
        public ICommand LoadedEventCommand
        {
            get
            {
                return CommandContext.GetCommand<System.Windows.RoutedEventArgs>(() => LoadedEventCommand, (e) =>
                {
                    if (!IsLoaded && !Util.IsDesignMode)
                    {
                        IsLoaded = true;
                        Source = e.Source;
                        OnLoad();
                    }
                });
            }
        }

        protected virtual void OnLoad()
        {
            if (Loaded != null)
                Loaded(this, EventArgs.Empty);
        }

        #endregion

        protected virtual void RefreshTitle()
        {
            if (Metadata.Module.TypeName.IsNotEmpty())
                Title = RuntimeContext.Service.L10N.GetText("Title." + Metadata.Module.TypeName);
            else
                Title = RuntimeContext.Service.L10N.GetText("Title." + Metadata.TypeName);
        }

        protected virtual bool GetPermission(string commandCode)
        {
            return Authorization.GetPermission(Metadata, commandCode);
        }

        /// <summary>
        /// 異步執行
        /// </summary>
        /// <param name="statusText"></param>
        /// <param name="action">執行方法</param>
        /// <param name="completed">回調方法</param>
        protected void RunAsync(string statusText, Action action, Action completed)
        {
            ViewStatus.IsBusy = true;
            ViewStatus.Cursor = System.Windows.Input.Cursors.Wait;
            ViewStatus.StatusText = statusText;
            RuntimeContext.Service.Messenger.Send(new GenericMessage<ViewStatus>(ViewStatus));
            Exception exception = null;
            Action<IAsyncResult> resultHandler = (asyncResult) =>
            {
                ViewStatus.IsBusy = false;
                ViewStatus.Cursor = System.Windows.Input.Cursors.Arrow;
                ViewStatus.StatusText = RuntimeContext.Service.L10N.GetText("Status.Ready");
                RuntimeContext.Service.Messenger.Send(new GenericMessage<ViewStatus>(ViewStatus));
                if (exception != null)
                    RuntimeContext.Service.ExceptionHandler.Handle(exception);
                if (completed != null)
                    completed();
            };
            var callBack = new AsyncCallback((asyncResult) =>
            {
                System.Windows.Application.Current.Dispatcher.BeginInvoke(DispatcherPriority.Normal, resultHandler, asyncResult);
            });
            Action run = new Action(() =>
            {
                try
                {
                    action.Invoke();
                }
                catch (Exception exc)
                {
                    exception = exc;
                }
            });
            run.BeginInvoke(callBack, null);
        }

        /// <summary>
        /// 設置參數，通常通過URI傳入。如 XXX.xaml?user=ERICLUENG&companyid=QP
        /// </summary>
        /// <param name="queryString"></param>
        public void SetQueryString(string queryString)
        {
            SetArgs(DataArgs.Parse(queryString));
        }

        /// <summary>
        /// 設置參數，子類需要實現對參數的處理
        /// </summary>
        /// <param name="args"></param>
        public virtual void SetArgs(DataArgs args)
        {
        }

        public void Serialize(SerializationInfo info)
        {
            OnSerializing(info);
        }

        public void Deserialize(SerializationInfo info)
        {
            OnDeserializing(info);
        }

        protected virtual void OnDeserializing(SerializationInfo info)
        {
        }

        protected virtual void OnSerializing(SerializationInfo info)
        {
        }
    }
}
