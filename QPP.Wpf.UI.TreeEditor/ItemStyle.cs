using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Media;
using Newtonsoft.Json;
using QPP.ComponentModel;
using System.Collections.ObjectModel;
using System.Collections.Specialized;

namespace QPP.Wpf.UI.TreeEditor
{
    public class ImageUrl
    {
        public ImageUrl(string url)
        {
            Url = url;
        }
        public string Url { get; set; }
    }
    [Serializable]
    public class ItemStyle
    {
        public ItemStyle()
        {
            ImageUrl = new ObservableCollection<ImageUrl>();
            ImageUrl.CollectionChanged += ImageUrl_CollectionChanged;

        }

        private void ImageUrl_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            if (e.Action == NotifyCollectionChangedAction.Add)
            {
                if (designerItem != null)
                {
                    designerItem.DiagramControl.SetItemStyle(designerItem, this);
                    designerItem.DiagramControl.Manager.SetWidth(designerItem);
                    designerItem.DiagramControl.Manager.Arrange();
                }
            }
        }

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
        private ObservableCollection<ImageUrl> _ImageUrl;
        public ObservableCollection<ImageUrl> ImageUrl
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
