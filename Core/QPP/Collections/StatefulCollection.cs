using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Text;
using QPP.ComponentModel;
using QPP.Data;

namespace QPP.Collections
{
    public class StatefulCollection<T> : BindingCollection<T>, IStatefulCollection
        where T : IStatefulObject
    {
        [NonSerialized]
        protected internal List<T> deleted = new List<T>();

        protected override void ClearItems()
        {
            deleted.Clear();
            base.ClearItems();
        }

        protected override void InsertItem(int index, T item)
        {
            item.MarkCreated();
            base.InsertItem(index, item);
        }

        void RemoveItem(T item)
        {
            if (item.DataState != DataState.Created)
            {
                item.MarkDeleted();
                deleted.Add(item);
            }
        }

        protected override void RemoveItem(int index)
        {
            var item = this[index];
            RemoveItem(item);
            base.RemoveItem(index);
        }

        protected override void SetItem(int index, T item)
        {
            var oldItem = this[index];
            RemoveItem(oldItem);
            item.MarkCreated();
            base.SetItem(index, item);
        }

        public IEnumerable<T> GetDeleted()
        {
            return deleted;
        }

        /// <summary>
        /// 获取删除的列表
        /// </summary>
        IList IStatefulCollection.Deleted
        {
            get { return deleted; }
        }
    }
}
