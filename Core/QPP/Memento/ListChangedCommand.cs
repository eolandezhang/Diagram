using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using QPP.ComponentModel;
using System.Collections.Specialized;
using QPP.Collections;

namespace QPP.Memento
{
    public class ListChangedCommand : IExcutable
    {
        Snapshot snapshot { get; set; }

        public ListChangedCommand(CollectionObserver collection, NotifyCollectionChangedEventArgs e)
        {
            if (e.Action == System.Collections.Specialized.NotifyCollectionChangedAction.Add)
            {
                foreach (var item in e.NewItems)
                {
                    snapshot = new Snapshot()
                    {
                        Collection = collection,
                        NewState = DataState.Created,
                        NewValue = item,
                        Index = e.NewStartingIndex
                    };
                }
            }
            else if (e.Action == System.Collections.Specialized.NotifyCollectionChangedAction.Remove)
            {
                foreach (var item in e.OldItems)
                {
                    snapshot = new Snapshot()
                    {
                        Collection = collection,
                        NewState = DataState.Deleted,
                        OldValue = item,
                        Index = e.OldStartingIndex
                    };
                }
            }
        }

        public void Redo()
        {
            if (snapshot.NewState == DataState.Created)
            {
                var item = snapshot.NewValue;
                snapshot.Collection.Created.Add(item);
                snapshot.Collection.Add(item);
            }
            else if (snapshot.NewState == DataState.Deleted)
            {
                var item = snapshot.OldValue;
                snapshot.Collection.Deleted.Add(item);
                snapshot.Collection.Remove(item);
            }
        }

        public void Undo()
        {
            if (snapshot.NewState == DataState.Created)
            {
                var item = snapshot.NewValue;
                snapshot.Collection.Created.Remove(item);
                snapshot.Collection.Remove(item);
            }
            else if (snapshot.NewState == DataState.Deleted)
            {
                var item = snapshot.OldValue;
                snapshot.Collection.Deleted.Remove(item);
                snapshot.Collection.Add(item);
            }
        }

        /// <summary>
        /// 快照
        /// </summary>
        public class Snapshot
        {
            /// <summary>
            /// 新状态
            /// </summary>
            public DataState NewState { get; set; }
            /// <summary>
            /// 模型对象
            /// </summary>
            public CollectionObserver Collection { get; set; }
            /// <summary>
            /// 属性名称
            /// </summary>
            public string PropertyName { get; set; }
            /// <summary>
            /// 新值
            /// </summary>
            public object NewValue { get; set; }
            /// <summary>
            /// 原值
            /// </summary>
            public object OldValue { get; set; }

            public int Index { get; set; }
        }
    }
}
