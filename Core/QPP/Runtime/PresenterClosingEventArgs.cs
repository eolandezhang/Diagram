using QPP.Modularity;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace QPP.Runtime
{
    public class PresenterClosingEventArgs: CancelEventArgs
    {
        public IPresenter Presenter { get; private set; }

        public PresenterClosingEventArgs(IPresenter presenter)
        {
            Presenter = presenter;
        }

        public PresenterClosingEventArgs(IPresenter presenter, bool cancel)
            : base(cancel)
        {
            Presenter = presenter;
        }
    }
}
