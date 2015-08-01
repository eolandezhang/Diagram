using System;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Markup;

namespace QPP.Wpf.UI.TreeEditor
{
    //[DefaultProperty("Items")]
    [ContentProperty("Items")]
    public class DiagramItem : FrameworkElement
    {
        public string Id { get; set; }
        public string PId { get; set; }
        public ObservableCollection<DiagramItem> Items { get; set; }
        public string Text { get; set; }
        public DiagramItem()
        {
            Id = Guid.NewGuid().ToString();
            PId = "";
            Items = new ObservableCollection<DiagramItem>();
        }
        
    }
}
