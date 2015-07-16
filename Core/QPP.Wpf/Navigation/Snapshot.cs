using QPP.Layout;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Navigation;

namespace QPP.Wpf.Navigation
{
    public class Snapshot
    {
        public string Caption { get; private set; }
        public string Uri { get; private set; }
        public string Key { get; private set; }
        public string Data { get; private set; }

        public Snapshot(IDockingContent content)
        {
            //Key = content.ContentKey;
            //Uri = BaseUriHelper.GetBaseUri(content as DependencyObject).AbsolutePath;
            //var args = new SerializationEventArgs(content);
            //content.OnSerializing(args);
            //Data = args.Data;
            //Caption = content.Title;
        }
    }
}
