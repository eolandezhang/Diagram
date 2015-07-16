using QPP.ComponentModel;
using QPP.Layout;
using QPP.Metadata;
using QPP.Modularity;
using QPP.Navigation;
using QPP.Runtime;
using QPP.Utils;
using QPP.Wpf.ComponentModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Navigation;

namespace QPP.Wpf.Navigation
{
    public class Navigator : INavigator
    {
        Stack<Snapshot> m_ForwardStack = new Stack<Snapshot>();
        Stack<Snapshot> m_BackwardStack = new Stack<Snapshot>();

        bool m_SuppressChanged;

        IDockingDocument m_ActivedDocument;

        public Navigator()
        {
        }

        void OnActivedContentChanged(object sender, EventArgs e)
        {
            //var oldContent = m_ActivedDocument;

            //var newContent = m_LayoutManager.ActivedContent as IDockingDocument;
            //if (newContent != null)
            //{
            //    m_ActivedDocument = newContent;

            //    if (!m_SuppressChanged && oldContent != null)
            //    {
            //        var snapshot = new Snapshot(oldContent);
            //        m_BackwardStack.Push(snapshot);
            //        m_ForwardStack.Clear();
            //    }
            //}
        }

        public void Forward()
        {
            //if (!CanForward) return;
            //m_SuppressChanged = true;
            //m_BackwardStack.Push(new Snapshot(m_ActivedDocument));
            //var snapshot = m_ForwardStack.Pop();
            //var content = LayoutManager.Default.Show(new Uri(snapshot.Uri, UriKind.RelativeOrAbsolute), snapshot.Key);
            //var serializationArgs = new SerializationEventArgs(content);
            //content.OnSerializing(serializationArgs);
            //if (serializationArgs.Data != snapshot.Data)
            //{
            //    serializationArgs.Data = snapshot.Data;
            //    content.OnDeserializing(serializationArgs);
            //}
            //m_SuppressChanged = false;
        }

        public bool CanForward
        {
            get { return m_ForwardStack.Count > 0; }
        }

        public void Backward()
        {
            //if (!CanBackward) return;
            //m_SuppressChanged = true;
            //m_ForwardStack.Push(new Snapshot(m_ActivedDocument));
            //var snapshot = m_BackwardStack.Pop();
            //var content = LayoutManager.Default.Show(new Uri(snapshot.Uri, UriKind.RelativeOrAbsolute), snapshot.Key);
            //var serializationArgs = new SerializationEventArgs(content);
            //content.OnSerializing(serializationArgs);
            //if (serializationArgs.Data != snapshot.Data)
            //{
            //    serializationArgs.Data = snapshot.Data;
            //    content.OnDeserializing(serializationArgs);
            //}
            //m_SuppressChanged = false;
        }

        public bool CanBackward
        {
            get { return m_BackwardStack.Count > 0; }
        }

        public void AddHistory(string caption, string uri, string view, string viewType)
        {
            //Action run = new Action(() =>
            //{
            //    try
            //    {
            //        var data = OnSerializing();
            //        var uri = Metadata.Uri + "?data=" + data;
            //        var hashCode = message.Content.GetHashCode().ToString();
            //        RuntimeContext.Service.GetObject<INavigator>().AddModified(Title, hashCode, uri, Metadata.TypeName, ViewType.Wpf);
            //    }
            //    catch (Exception exc)
            //    {
            //        RuntimeContext.Service.Logger.Error(exc);
            //    }
            //});
            //run.BeginInvoke(new AsyncCallback(e => { }), null);
            throw new NotImplementedException();
        }

        public void AddModified(string caption, string hashCode, string uri, string view, ViewType viewType)
        {
            //Action run = new Action(() =>
            //{
            //    try
            //    {
            //        var data = OnSerializing();
            //        var uri = Metadata.Uri + "?data=" + data;
            //        var hashCode = message.Content.GetHashCode().ToString();
            //        RuntimeContext.Service.GetObject<INavigator>().AddModified(Title, hashCode, uri, Metadata.TypeName, ViewType.Wpf);
            //    }
            //    catch (Exception exc)
            //    {
            //        RuntimeContext.Service.Logger.Error(exc);
            //    }
            //});
            //run.BeginInvoke(new AsyncCallback(e => { }), null);
            throw new NotImplementedException();
        }

        public void RemoveModified(string hashCode, string view, ViewType viewType)
        {
            //Action run = new Action(() =>
            //{
            //    try
            //    {
            //        var data = OnSerializing();
            //        var uri = Metadata.Uri + "?data=" + data;
            //        var hashCode = message.Content.GetHashCode().ToString();
            //        RuntimeContext.Service.GetObject<INavigator>().AddModified(Title, hashCode, uri, Metadata.TypeName, ViewType.Wpf);
            //    }
            //    catch (Exception exc)
            //    {
            //        RuntimeContext.Service.Logger.Error(exc);
            //    }
            //});
            //run.BeginInvoke(new AsyncCallback(e => { }), null);
            throw new NotImplementedException();
        }
    }
}
