using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using QPP.ComponentModel;
using System.Collections.Specialized;
using QPP.Collections;

namespace QPP.Memento
{
    public class CollectionChangedCommand : IExcutable
    {
        Snapshot snapshot { get; set; }

        public CollectionChangedCommand(IStatefulCollection collection, NotifyCollectionChangedEventArgs e)
        {
            if (e.Action == System.Collections.Specialized.NotifyCollectionChangedAction.Add)
            {
                foreach (StatefulObject obj in e.NewItems)
                {
                    snapshot = new Snapshot()
                    {
                        Collection = collection,
                        OldState = obj.DataState,
                        NewState = DataState.Created,
                        NewValue = obj
                    };
                }
            }
            else if (e.Action == System.Collections.Specialized.NotifyCollectionChangedAction.Remove)
            {
                foreach (StatefulObject obj in e.OldItems)
                {
                    snapshot = new Snapshot()
                    {
                        Collection = collection,
                        OldState = obj.DataState,
                        NewState = DataState.Deleted,
                        OldValue = obj
                    };
                }
            }
        }

        public void Redo()
        {
            if (snapshot.NewState == DataState.Created)
            {
                var model = snapshot.NewValue as StatefulObject;
                model.MarkCreated();
                snapshot.Collection.SuppressPropertyChanged();
                snapshot.Collection.Add(model);
                snapshot.Collection.ResumePropertyChanged();
                snapshot.Collection.RaiseCollectionChanged();
            }
            else if (snapshot.NewState == DataState.Deleted)
            {
                var model = snapshot.OldValue as StatefulObject;
                snapshot.Collection.Deleted.Add(model);
                snapshot.Collection.SuppressPropertyChanged();
                snapshot.Collection.Remove(model);
                snapshot.Collection.ResumePropertyChanged();
                snapshot.Collection.RaiseCollectionChanged();
            }
        }

        public void Undo()
        {
            if (snapshot.NewState == DataState.Created)
            {
                var model = snapshot.NewValue as StatefulObject;
                snapshot.Collection.SuppressPropertyChanged();
                snapshot.Collection.Remove(model);
                snapshot.Collection.ResumePropertyChanged();
                snapshot.Collection.RaiseCollectionChanged();
            }
            else if (snapshot.NewState == DataState.Deleted)
            {
                var model = snapshot.OldValue as StatefulObject;
                snapshot.Collection.Deleted.Remove(model);
                if (snapshot.OldState == DataState.Created)
                    model.MarkCreated();
                else if (snapshot.OldState == DataState.Modified)
                    model.MarkModified();
                else if (snapshot.OldState == DataState.Normal)
                    model.ResetState();
                snapshot.Collection.SuppressPropertyChanged();
                snapshot.Collection.Add(model);
                snapshot.Collection.ResumePropertyChanged();
                snapshot.Collection.RaiseCollectionChanged();
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
            /// 原状态
            /// </summary>
            public DataState OldState { get; set; }
            /// <summary>
            /// 模型对象
            /// </summary>
            public IStatefulCollection Collection { get; set; }
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
        }
    }
}
