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
        private string _ImageUrl;
        public string ImageUrl
        {
            get { return _ImageUrl; }
            set
            {
                _ImageUrl = value;
                if (designerItem != null)
                    designerItem.DiagramControl.SetItemStyle(designerItem, this);
            }
        }
    }
}
