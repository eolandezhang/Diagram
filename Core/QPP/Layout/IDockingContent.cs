using QPP.Modularity;
using QPP.Runtime.Serialization;
using System;
using System.ComponentModel;

namespace QPP.Layout
{
    /// <summary>
    /// 支持Docking佈局的內容
    /// </summary>
    public interface IDockingContent : IView
    {
        void OnClosing(CancelEventArgs e);

        void OnClosed(EventArgs e);

        //void Serialize(SerializationInfo info);

        //void Deserialize(SerializationInfo info);

        string Title { get; }
    }
}
