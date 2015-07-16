using QPP.ComponentModel;
using QPP.Layout;
using QPP.Modularity;
using QPP.Runtime;
using QPP.Wpf.Windows;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;

namespace QPP.Wpf.Runtime
{
    public class RuntimeContainer : IRuntimeContainer
    {
        List<IPresenter> m_Presenters = new List<IPresenter>();
        List<IModule> m_Modules = new List<IModule>();
        List<IApplication> m_Applications = new List<IApplication>();
        ILayoutManager m_LayoutManager;
        Dictionary<string, IView> m_View = new Dictionary<string, IView>();

        public IPresenter MainView { get; set; }

        public void Run(IPresenter mainView)
        {
            MainView = mainView;
            Load(mainView);
        }

        public string Serialize()
        {
            return m_LayoutManager.LayoutSerialize();
        }

        public void Deserialize(string data)
        {
            m_LayoutManager.LayoutDeserialize(data);
            foreach (var content in m_LayoutManager.Contents)
            {
                LoadContent(content);
            }
        }

        #region ILoadableContainer

        public event EventHandler<LoadEventArgs> ItemLoaded;

        public event EventHandler<LoadEventArgs> ItemUnLoaded;

        protected virtual void OnItemLoaded(LoadEventArgs e)
        {
            if (ItemLoaded != null)
                ItemLoaded(this, e);
        }

        protected virtual void OnItemUnLoaded(LoadEventArgs e)
        {
            if (ItemUnLoaded != null)
                ItemUnLoaded(this, e);
        }

        public void Load(ILoadableItem item)
        {
            if (item is IPresenter)
                Load((IPresenter)item);
            else if (item is IModule)
                Load((IModule)item);
            else if (item is IApplication)
                Load((IApplication)item);
            else
                throw new NotSupportedException(item.GetType().FullName);
        }

        public void UnLoad(ILoadableItem item)
        {
            if (item is IPresenter)
                UnLoad((IPresenter)item);
            else if (item is IModule)
                UnLoad((IModule)item);
            else if (item is IApplication)
                UnLoad((IApplication)item);
            else
                throw new NotSupportedException(item.GetType().FullName);
        }

        protected void Load(IModule module)
        {
            if (!m_Modules.Contains(module))
            {
                m_Modules.Add(module);
                OnItemLoaded(new LoadEventArgs(module));
            }
        }

        protected void Load(IApplication app)
        {
            if (!m_Applications.Contains(app))
            {
                app.Initialize();
                m_Applications.Add(app);
                OnItemLoaded(new LoadEventArgs(app));
            }
        }

        protected void Load(IPresenter presenter)
        {
            if (!m_Presenters.Contains(presenter))
            {
                if (presenter.Metadata.Module.AppType != null)
                {
                    IApplication app = GetApplication(presenter);
                    if (app == null)
                    {
                        app = Activator.CreateInstance(presenter.Metadata.Module.AppType) as IApplication;
                        if (app == null)
                            throw new RuntimeException("AppType is not implemented IApplication");
                        Load(app);
                    }
                }
                if (presenter.Metadata.Module.TypeName.IsNotEmpty())
                {
                    IModule module = GetModule(presenter);
                    if (module == null)
                    {
                        module = new Module { Metadata = presenter.Metadata.Module };
                        Load(module);
                    }
                }
                presenter.Closed += (s, e) => { UnLoad(presenter); };
                m_Presenters.Add(presenter);
                OnItemLoaded(new LoadEventArgs(presenter));
            }
        }

        protected void UnLoad(IModule module)
        {
            var presenters = GetLoadedPresenters(module);
            m_Modules.Remove(module);
            foreach (var presenter in presenters)
                UnLoad(presenter);
            OnItemUnLoaded(new LoadEventArgs(module));
        }

        protected void UnLoad(IApplication app)
        {
            var modules = GetLoadedModules(app);
            m_Applications.Remove(app);
            foreach (var module in modules)
                UnLoad(module);
            OnItemUnLoaded(new LoadEventArgs(app));
        }

        protected void UnLoad(IPresenter presenter)
        {
            m_Presenters.Remove(presenter);
            OnItemUnLoaded(new LoadEventArgs(presenter));
            if (!m_Presenters.Any(p => p.Metadata.Module.TypeName == presenter.Metadata.Module.TypeName))
            {
                var module = GetModule(presenter);
                if (module != null)
                {
                    m_Modules.Remove(module);
                    OnItemUnLoaded(new LoadEventArgs(module));
                }
            }
            if (!m_Presenters.Any(p => p.Metadata.Module.AppType == presenter.Metadata.Module.AppType))
            {
                var app = GetApplication(presenter);
                if (app != null)
                {
                    m_Applications.Remove(app);
                    OnItemUnLoaded(new LoadEventArgs(app));
                }
            }
        }

        #endregion

        #region IModularityContainer

        public IEnumerable<IApplication> Applications
        {
            get { return m_Applications; }
        }

        public IEnumerable<IModule> Modules
        {
            get { return m_Modules; }
        }

        public IApplication GetApplication(IPresenter presenter)
        {
            return m_Applications.FirstOrDefault(p => p.GetType() == presenter.Metadata.Module.AppType);
        }

        public IApplication GetApplication(IModule module)
        {
            return m_Applications.FirstOrDefault(p => p.GetType() == module.Metadata.AppType);
        }

        public IModule GetModule(IPresenter presenter)
        {
            return m_Modules.FirstOrDefault(p => p.Metadata.TypeName == presenter.Metadata.Module.TypeName);
        }

        public IEnumerable<IModule> GetLoadedModules(IApplication application)
        {
            return m_Modules.Where(p => p.Metadata.AppType == application.GetType());
        }

        public IEnumerable<IPresenter> GetLoadedPresenters(IModule module)
        {
            return m_Presenters.Where(p => p.Metadata.Module.TypeName == module.Metadata.TypeName);
        }

        public IEnumerable<IPresenter> GetLoadedPresenters(IApplication app)
        {
            return m_Presenters.Where(p => p.Metadata.Module.AppType == app.GetType());
        }

        #endregion

        #region IPresenter

        public event EventHandler<PresenterEventArgs> ActivedPresenterChanged;

        public event EventHandler<PresenterOpeningEventArgs> PresenterOpening;

        public event EventHandler<PresenterEventArgs> PresenterInitialized;

        public event EventHandler<PresenterEventArgs> PresenterLoaded;

        public event EventHandler<PresenterClosingEventArgs> PresenterClosing;

        public event EventHandler<PresenterEventArgs> PresenterClosed;

        protected virtual void OnActivedPresenterChanged(PresenterEventArgs e)
        {
            if (ActivedPresenterChanged != null)
                ActivedPresenterChanged(this, e);
        }

        protected virtual void OnPresenterOpening(PresenterOpeningEventArgs e)
        {
            if (PresenterOpening != null)
                PresenterOpening(this, e);
        }

        protected virtual void OnPresenterInitialized(PresenterEventArgs e)
        {
            if (PresenterInitialized != null)
                PresenterInitialized(this, e);
        }

        protected virtual void OnPresenterLoaded(PresenterEventArgs e)
        {
            if (PresenterLoaded != null)
                PresenterLoaded(this, e);
        }

        protected virtual void OnPresenterClosing(PresenterClosingEventArgs e)
        {
            if (PresenterClosing != null)
                PresenterClosing(this, e);
        }

        protected virtual void OnPresenterClosed(PresenterEventArgs e)
        {
            if (PresenterClosed != null)
                PresenterClosed(this, e);
        }

        public IPresenter ActivedPresenter { get; private set; }

        public IEnumerable<IPresenter> Presenters
        {
            get { return m_Presenters; }
        }

        public IEnumerable<string> PresenterKeys
        {
            get { return m_View.Keys; }
        }

        public IPresenter OpenDialog(object owner, string uri, DataArgs args = null)
        {
            if (uri == null)
                throw new ArgumentNullException("uri");
            var contentArgs = args ?? new DataArgs();
            if (uri.IndexOf("?") > 0)
                contentArgs.Combine(DataArgs.Parse(uri));
            var e = new PresenterOpeningEventArgs(uri, null, contentArgs);
            OnPresenterOpening(e);
            var component = RuntimeContext.Service.GetObject<IModuleTypeLoader>()
                .LoadComponent(new Uri(e.Uri, UriKind.RelativeOrAbsolute));
            if (!(component is IView))
                throw new RuntimeException("Not support component type:" + component.GetType());
            var window = component as IWindow;
            if (window == null)
            {
                window = new MetroWin { Content = component };
                window.Presenter = ((IView)component).Presenter;
            }
            window.Presenter.Loaded += (x, y) => { OnPresenterLoaded(new PresenterEventArgs(window.Presenter)); };
            window.Activated += (x, y) =>
            {
                ActivedPresenter = window.Presenter;
                OnActivedPresenterChanged(new PresenterEventArgs(window.Presenter));
            };
            window.Closing += (x, y) =>
            {
                var closingEventArgs = new PresenterClosingEventArgs(window.Presenter, y.Cancel);
                OnPresenterClosing(closingEventArgs);
                y.Cancel = closingEventArgs.Cancel;
            };
            window.Closed += (x, y) =>
            {
                OnPresenterClosed(new PresenterEventArgs(window.Presenter));
            };
            if (owner is IWindow)
                window.Owner = (IWindow)owner;
            else if (owner is DependencyObject)
                window.Owner = (IWindow)Window.GetWindow((DependencyObject)owner);
            if (e.Args != null)
                window.Presenter.SetArgs(e.Args);
            OnPresenterInitialized(new PresenterEventArgs(window.Presenter));
            Load(window.Presenter);
            window.ShowDialog();
            return window.Presenter;
        }

        public IPresenter Open(string uri, DataArgs args = null, string key = null)
        {
            if (uri == null)
                throw new ArgumentNullException("uri");
            var contentKey = key;
            var contentArgs = args ?? new DataArgs();
            if (uri.IndexOf("?") > 0)
                contentArgs.Combine(DataArgs.Parse(uri));
            if (contentKey.IsNullOrEmpty())
            {
                contentKey = contentArgs.Get<string>("ContentKey");
                if (contentKey.IsNullOrEmpty())
                    contentKey = uri;
            }
            var e = new PresenterOpeningEventArgs(uri, contentKey, contentArgs);
            OnPresenterOpening(e);

            if (m_View.ContainsKey(e.Key))
            {
                var view = m_View[e.Key];
                if (view is IWindow)
                    return ShowWindow((IWindow)view, e.Args);
                if (view is IDockingContent)
                    return ShowContent((IDockingContent)view, e.Args);
            }

            return LoadPresenter(e.Uri, e.Args, e.Key);
        }

        IPresenter LoadPresenter(string uri, DataArgs args, string contentKey)
        {
            var component = RuntimeContext.Service.GetObject<IModuleTypeLoader>()
                .LoadComponent(new Uri(uri, UriKind.RelativeOrAbsolute));

            if (component is IWindow)
            {
                var window = (IWindow)component;
                window.ContentKey = contentKey;
                LoadWindow(contentKey, window);
                return ShowWindow(window, args);
            }
            else if (component is IDockingContent)
            {
                var content = (IDockingContent)component;
                content.ContentKey = contentKey;
                LoadContent(content);
                return ShowContent(content, args);
            }
            else if (component is IRunnable)
            {
                ((IRunnable)component).Run(args);
                return null;
            }
            throw new RuntimeException("Not support component type:" + component.GetType());
        }

        void LoadWindow(string contentKey, IWindow window)
        {
            window.Presenter.Loaded += (x, y) => { OnPresenterLoaded(new PresenterEventArgs(window.Presenter)); };
            window.Activated += (x, y) =>
            {
                ActivedPresenter = window.Presenter;
                OnActivedPresenterChanged(new PresenterEventArgs(window.Presenter));
            };
            window.Closing += (x, y) =>
            {
                var closingEventArgs = new PresenterClosingEventArgs(window.Presenter, y.Cancel);
                OnPresenterClosing(closingEventArgs);
                y.Cancel = closingEventArgs.Cancel;
            };
            window.Closed += (x, y) =>
            {
                m_View.Remove(contentKey);
                OnPresenterClosed(new PresenterEventArgs(window.Presenter));
            };
            m_View.Add(contentKey, window);
            Load(window.Presenter);
            OnPresenterInitialized(new PresenterEventArgs(window.Presenter));
        }

        void LoadContent(IDockingContent content)
        {
            content.Presenter.Loaded += (x, y) => { OnPresenterLoaded(new PresenterEventArgs(content.Presenter)); };
            content.Closing += (x, y) =>
            {
                var closingEventArgs = new PresenterClosingEventArgs(content.Presenter, y.Cancel);
                OnPresenterClosing(closingEventArgs);
                y.Cancel = closingEventArgs.Cancel;
            };
            content.Closed += (x, y) =>
            {
                m_View.Remove(content.ContentKey);
                OnPresenterClosed(new PresenterEventArgs(content.Presenter));
            };
            m_View.Add(content.ContentKey, content);
            Load(content.Presenter);
            OnPresenterInitialized(new PresenterEventArgs(content.Presenter));
        }

        IPresenter ShowWindow(IWindow window, DataArgs args)
        {
            if (args != null)
                window.Presenter.SetArgs(args);
            window.Show();
            window.Activate();
            ExecuteCommand(window.Presenter, args);
            return window.Presenter;
        }

        void ExecuteCommand(IPresenter presenter, DataArgs args)
        {
            var cmd = args.Get<string>("cmd");
            if (cmd.IsNotEmpty())
            {
                foreach (var c in cmd.Split(','))
                {
                    if (presenter.CommandContext.Commands.ContainsName(c))
                        presenter.CommandContext.Commands[c].Command.Execute(null);
                }
            }
        }

        IPresenter ShowContent(IDockingContent content, DataArgs args)
        {
            if (args != null)
                content.Presenter.SetArgs(args);
            m_LayoutManager.Show(content);
            ExecuteCommand(content.Presenter, args);
            return content.Presenter;
        }

        public void Show(IPresenter presenter)
        {
            var views = m_View.Values.Where(p => p.Presenter == presenter);
            foreach (var view in views)
            {
                if (view is IWindow)
                    ((IWindow)view).Show();
                else if (view is IDockingContent)
                    m_LayoutManager.Show((IDockingContent)view);
            }
        }

        public void Close(IPresenter presenter)
        {
            var views = m_View.Values.Where(p => p.Presenter == presenter).ToList();
            foreach (var view in views)
            {
                if (view is IWindow)
                    ((IWindow)view).Close();
                else if (view is IDockingContent)
                    m_LayoutManager.Close((IDockingContent)view);
            }
        }

        public bool ContainsPresenter(string key)
        {
            return m_View.ContainsKey(key);
        }

        public IPresenter GetPresenter(string key)
        {
            if (m_View.ContainsKey(key))
                return m_View[key].Presenter;
            return null;
        }

        /// <summary>
        /// 
        /// </summary>
        public IView GetView(IPresenter presenter)
        {
            return m_View.Values.FirstOrDefault(p => p.Presenter == presenter);
        }

        #endregion

        public void SetLayoutManager(ILayoutManager manager)
        {
            if (m_LayoutManager != null)
                m_LayoutManager.ActivedContentChanged -= OnLayoutActivedContentChanged;
            m_LayoutManager = manager;
            if (m_LayoutManager != null)
                m_LayoutManager.ActivedContentChanged += OnLayoutActivedContentChanged;
        }

        void OnLayoutActivedContentChanged(object sender, ContentEventArgs e)
        {
            if (e.Content != null)
                ActivedPresenter = e.Content.Presenter as IPresenter;
            OnActivedPresenterChanged(new PresenterEventArgs(ActivedPresenter));
        }
    }
}
