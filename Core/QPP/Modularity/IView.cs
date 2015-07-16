using QPP.Runtime.Serialization;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace QPP.Modularity
{
    /// <summary>
    /// 視圖
    /// </summary>
    public interface IView
    {
        IPresenter Presenter { get; set; }

        event CancelEventHandler Closing;

        event EventHandler Closed;

        string ContentKey { get; set; }
    }
}
