using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using QPP.ComponentModel;

namespace QPP.Memento
{
    public class PropertyChangedCommand : IExcutable
    {
        Snapshot snapshot { get; set; }

        public PropertyChangedCommand(IStatefulObject model, ValueChangedEventArgs e)
        {
            if (model.DataState == DataState.Initializing) return;
            snapshot = new Snapshot()
            {
                Model = model as StatefulObject,
                OldState = model.DataState,
                NewState = DataState.Modified,
                OldValue = e.OldValue,
                NewValue = e.NewValue,
                PropertyName = e.PropertyName
            };
        }

        public void Redo()
        {
            snapshot.Model.SuppressPropertyChanged();
            snapshot.Model.Set(snapshot.PropertyName, snapshot.NewValue);
            snapshot.Model.ResumePropertyChanged();
            if (snapshot.OldState == DataState.Normal)
                snapshot.Model.MarkModified();
            snapshot.Model.RaisePropertyChanged(snapshot.PropertyName);
        }

        public void Undo()
        {
            snapshot.Model.SuppressPropertyChanged();
            snapshot.Model.Set(snapshot.PropertyName, snapshot.OldValue);
            snapshot.Model.ResumePropertyChanged();
            if (snapshot.OldState == DataState.Normal)
                snapshot.Model.ResetState();
            snapshot.Model.RaisePropertyChanged(snapshot.PropertyName);
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
            public StatefulObject Model { get; set; }
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
