using QPP.Layout;
using QPP.Runtime;
using QPP.Wpf.UI.Controls.AvalonDock;
using QPP.Wpf.UI.Controls.AvalonDock.Layout;
using QPP.Wpf.UI.Controls.AvalonDock.Layout.Serialization;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using System.Windows.Navigation;

namespace QPP.Wpf.Layout
{
    public class DockingWrapper : ILayoutManager
    {
        DockingManager m_DockingManager;

        Window m_Window;

        InputBindingCollection m_DefaultInputBinding = new InputBindingCollection();

        LayoutContentCollection m_Contents = new LayoutContentCollection();

        public event EventHandler<ContentEventArgs> ActivedContentChanged;

        public DockingWrapper(DockingManager manager)
        {
            m_DockingManager = manager;
            m_Window = Window.GetWindow(manager);
            m_DefaultInputBinding.AddRange(m_Window.InputBindings);
            m_DockingManager.ActiveContentChanged += new EventHandler(m_DockingManager_ActiveContentChanged);
        }

        void m_DockingManager_ActiveContentChanged(object sender, EventArgs e)
        {
            //更新快捷鍵
            m_Window.InputBindings.Clear();
            m_Window.InputBindings.AddRange(m_DefaultInputBinding);
            var content = m_DockingManager.ActiveContent as DockingContent;
            if (content != null)
                m_Window.InputBindings.AddRange(content.InputBindings);

            OnActivedContentChanged(new ContentEventArgs(content));
        }

        public IEnumerable<IDockingContent> Contents
        {
            get { return m_Contents.Values.Select(p => p.Content).OfType<IDockingContent>(); }
        }

        public void Show(IDockingContent content)
        {
            ShowWindow(m_Window);
            if (m_Contents.ContainsKey(content.ContentKey))
            {
                var layout = m_Contents[content.ContentKey];
                if (layout is LayoutAnchorable)
                {
                    var anchor = layout as LayoutAnchorable;
                    if (anchor.IsHidden || anchor.IsAutoHidden)
                    {
                        anchor.IsActive = true;
                        anchor.Show();
                    }
                }
                layout.IsActive = true;
            }
            else
            {
                var docking = m_DockingManager.AddToLayout(content);
                m_Contents.Add(content.ContentKey, docking);
            }
        }

        protected virtual void OnActivedContentChanged(ContentEventArgs e)
        {
            if (ActivedContentChanged != null)
                ActivedContentChanged(this, e);
        }

        void ShowWindow(Window window)
        {
            try
            {
                if (window.Visibility != Visibility.Visible)
                    window.Show();
                if (window.WindowState == WindowState.Minimized)
                    window.WindowState = WindowState.Normal;
                window.Activate();
            }
            catch { }
        }

        public void CloseCurrent()
        {
            var doc = m_DockingManager.Layout.ActiveContent as LayoutDocument;
            if (doc != null && doc.CanClose)
                doc.Close();
        }

        public bool CanCloseCurrent()
        {
            var doc = m_DockingManager.Layout.ActiveContent as LayoutDocument;
            return doc != null && doc.CanClose;
        }

        public void CloseAll()
        {
            m_DockingManager.CloseAll();
        }

        public bool CanCloseAll()
        {
            return m_DockingManager.Layout.Descendents().OfType<LayoutDocument>().Any(p => p.CanClose);
        }

        public string LayoutSerialize()
        {
            var serializer = new XmlLayoutSerializer(m_DockingManager);
            using (StringWriter sw = new StringWriter())
            {
                serializer.Serialize(sw);
                return sw.GetStringBuilder().ToString();
            }
        }

        public void LayoutDeserialize(string data)
        {
            var serializer = new XmlLayoutSerializer(m_DockingManager);
            using (var stream = new StringReader(data))
                serializer.Deserialize(stream);
            foreach (var layout in m_DockingManager.Layout.Descendents().OfType<LayoutContent>())
            {
                var content = layout.Content as DockingContent;
                if (content != null)
                {
                    layout.Title = content.Title; 
                    layout.IconSource = content.IconSource;
                    content.PropertyChanged += (s, e) =>
                    {
                        if (e.Property.Name == "Title")
                            layout.Title = content.Title;
                        else if (e.Property.Name == "IconSource")
                            layout.IconSource = content.IconSource;
                    };
                    layout.Closing += (s, e) => { content.OnClosing(e); };
                    layout.Closed += (s, e) => { content.OnClosed(e); };
                    m_Contents.Add(content.ContentKey, layout);
                }
            }
        }

        public void Close(IDockingContent content)
        {
            var layout = m_Contents.Values.FirstOrDefault(p => p.Content == content);
            layout.Close();
        }
    }
}
