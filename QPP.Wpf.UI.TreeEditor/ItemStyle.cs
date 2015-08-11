using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Media;
using Newtonsoft.Json;
using QPP.ComponentModel;

namespace QPP.Wpf.UI.TreeEditor
{
    [Serializable]
    public class ItemStyle
    {
        [NonSerialized]
        public DesignerItem designerItem;

        private SolidColorBrush _SolidColorBrush;
        public SolidColorBrush BorderBrush
        {
            get { return _SolidColorBrush; }
            set
            {
                _SolidColorBrush = value;
                if (designerItem != null)
                    designerItem.DiagramControl.SetItemStyle(designerItem, this);
            }
        }
        // { get { return Get<SolidColorBrush>("BorderBrush"); } set { Set("BorderBrush", value); } }
        private SolidColorBrush _Background;
        public SolidColorBrush Background
        {
            get { return _Background; }
            set
            {
                _Background = value;
                if (designerItem != null)
                    designerItem.DiagramControl.SetItemStyle(designerItem, this);
            }
        }
    }
}
