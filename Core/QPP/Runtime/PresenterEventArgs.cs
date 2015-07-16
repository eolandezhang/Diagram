using QPP.ComponentModel;
using QPP.Modularity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace QPP.Runtime
{
    /// <summary>
    /// 視圖事件參數
    /// </summary>
    public class PresenterEventArgs: EventArgs
    {
        /// <summary>
        /// 運行時可加載項目
        /// </summary>
        public IPresenter Presenter { get; private set; }

        public PresenterEventArgs(IPresenter presenter)
        {
            Presenter = presenter;
        }
    }
}
