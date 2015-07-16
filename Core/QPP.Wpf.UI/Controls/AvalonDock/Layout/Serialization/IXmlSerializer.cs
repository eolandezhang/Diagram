using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace QPP.Wpf.UI.Controls.AvalonDock.Layout.Serialization
{
    public interface IXmlSerializer
    {
        object Deserialize(System.Xml.XmlReader reader);
        void Serialize(System.Xml.XmlWriter writer, object o);
    }
}
