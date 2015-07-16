using QPP.Wpf.UI.Controls.AvalonDock.Layout;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace QPP.Wpf.Layout
{
    public class LayoutContentCollection
    {
        Dictionary<string, LayoutContent> m_Contents = new Dictionary<string, LayoutContent>();

        public void Add(string contentKey, LayoutContent content)
        {
            content.Closed += (s, e) => { Remove(contentKey); };
            m_Contents.Add(contentKey.ToSafeString().ToLower(), content);
        }

        public bool ContainsKey(string contentKey)
        {
            var key = contentKey.ToSafeString().ToLower();
            return m_Contents.ContainsKey(key);
        }

        public LayoutContent this[string contentKey]
        {
            get { return m_Contents[contentKey.ToSafeString().ToLower()]; }
        }

        public IEnumerable<LayoutContent> Values
        {
            get { return m_Contents.Values; }
        }

        public void Remove(string contentKey)
        {
            m_Contents.Remove(contentKey.ToSafeString().ToLower());
        }
    }
}
