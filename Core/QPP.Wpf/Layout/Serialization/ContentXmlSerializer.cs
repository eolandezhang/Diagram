using QPP.Layout;
using QPP.Runtime;
using QPP.Runtime.Serialization;
using QPP.Wpf.UI.Controls.AvalonDock.Layout.Serialization;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Windows;
using System.Windows.Navigation;

namespace QPP.Wpf.Layout.Serialization
{
    public class ContentXmlSerializer : IXmlSerializer
    {
        public object Deserialize(System.Xml.XmlReader reader)
        {
            //读取Data 和 ContentUri
            if (reader.MoveToAttribute("ContentUri"))
            {
                try
                {
                    var component = Application.LoadComponent(new Uri(reader.Value, UriKind.RelativeOrAbsolute));
                    if (component is IDockingContent)
                    {
                        var content = component as IDockingContent;
                        SerializationInfo info = null;
                        if (reader.MoveToAttribute("ContentData"))
                        {
                            var json = Encoding.UTF8.GetString(Convert.FromBase64String(reader.Value));
                            info = Newtonsoft.Json.JsonConvert.DeserializeObject<SerializationInfo>(json);
                        }
                        if (info == null)
                            info = new SerializationInfo();
                        content.Presenter.Deserialize(info);

                        if (reader.MoveToAttribute("ContentKey"))
                            content.ContentKey = reader.Value;
                    }
                    return component;
                }
                catch { }
            }
            return null;
        }

        public void Serialize(System.Xml.XmlWriter writer, object o)
        {
            if (o is IDockingContent)
            {
                var content = o as IDockingContent;
                if (!content.Presenter.CanSerialize)
                    return;
                var info = new SerializationInfo();
                content.Presenter.Serialize(info);
                var json = Newtonsoft.Json.JsonConvert.SerializeObject(info);
                var data = Convert.ToBase64String(Encoding.UTF8.GetBytes(json));
                writer.WriteAttributeString("ContentData", data);
                var uri = BaseUriHelper.GetBaseUri(content as DependencyObject);
                writer.WriteAttributeString("ContentUri", uri.AbsolutePath);
                writer.WriteAttributeString("ContentKey", content.ContentKey);
            }
        }
    }
}
