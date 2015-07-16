using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace QPP.Wpf.UI.Controls.AvalonDock.Layout.Serialization
{
    public interface IXmlSerializeContract
    {
        bool CanSerialize { get; }
        IXmlSerializer CreateSerializer();
    }
}
