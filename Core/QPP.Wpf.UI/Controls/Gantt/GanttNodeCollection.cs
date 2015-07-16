using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using QPP.Wpf.UI.Models;
using QPP.ComponentModel;
using QPP.Collections;

namespace QPP.Wpf.UI.Controls.Gantt
{
    public class GanttNodeCollection : HierarchicalCollection<IGanttNode>
    {
        public GanttNodeCollection()
        {

        }

        public GanttNodeCollection(List<IGanttNode> list)
            : base(list)
        {

        }

        public GanttNodeCollection(IEnumerable<IGanttNode> collection)
            : base(collection)
        {

        }

        ///// <summary>
        ///// 清空并重新加载集合
        ///// </summary>
        ///// <param name="collection"></param>
        //public void ReLoad(IEnumerable<IGanttNode> collection)
        //{
        //    Items.Clear();
        //    foreach (var i in collection)
        //        Items.Add(i);
        //    RaiseCollectionChanged();
        //}

        //public void RaiseCollectionChanged()
        //{
        //    OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
        //}
    }
}
