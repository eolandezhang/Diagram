using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace QPP.Modularity
{
    public class ModuleDownloadProgressChangedEventArgs : ProgressChangedEventArgs
    {
        public ModuleDownloadProgressChangedEventArgs(int progress)
            :base(progress, null)
        {

        }

        public ModuleDownloadProgressChangedEventArgs(int progress, object userState)
            : base(progress, userState)
        {

        }
    }
}
