using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace QPP.Modularity
{
    public interface IModuleTypeLoader
    {
        event EventHandler<LoadComponentEventArgs> BeforeDownload;

        event EventHandler<ModuleDownloadProgressChangedEventArgs> DownloadProgressChanged;

        event EventHandler DownloadCompleted;

        event EventHandler<LoadComponentEventArgs> PreLoad;

        event EventHandler LoadCompleted;

        object LoadComponent(Uri uri);
    }
}
