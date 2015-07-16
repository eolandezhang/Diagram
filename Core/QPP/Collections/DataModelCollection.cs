using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using QPP.ComponentModel;
using QPP.Data;

namespace QPP.Collections
{
    /// <summary>
    /// 数据模型的集合
    /// </summary>
    /// <typeparam name="T">IDataModel</typeparam>
    public class DataModelCollection<T> : StatefulCollection<T>, IBindingList
        where T : IDataModel
    {
        /// <summary>
        /// 保存数据
        /// </summary>
        //public void Save()
        //{
        //    var proxy = ModelManager.Default.GetProxy<T>();
        //    foreach (var item in deleted)
        //    {
        //        proxy.Delete(item);                
        //    }
        //    deleted.Clear();

        //    var modified = GetModified();
        //    foreach (var item in modified)
        //    { 
        //        proxy.Update(item).CopyTo(item);
        //        item.ResetState();
        //    }

        //    var created = GetCreated();
        //    foreach (var item in created)
        //    {
        //        proxy.Create(item).CopyTo(item);//CopyTo 更新所有数据
        //        item.ResetState();
        //    }

        //    OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
        //    OnListChanged(new ListChangedEventArgs(ListChangedType.Reset, 0));
        //}
    }
}
