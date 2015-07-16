using System;
using System.ComponentModel;
using System.Runtime.Serialization;
using QPP.Validation;

namespace QPP.ComponentModel
{
    /// <summary>
    /// 有状态的对象，创建的时候状态为Created，反序列化后状态为Normal。
    /// </summary>
    [Serializable]
    [System.Runtime.Serialization.DataContract]
    public abstract class StatefulObject : ObservableObject, IStatefulObject
    {
        [NonSerialized]
        DataState state;

        public virtual event EventHandler StateChanged;

        protected virtual void OnStateChanged()
        {
            if (StateChanged != null)
                StateChanged(this, EventArgs.Empty);
        }

        /// <summary>
        /// 状态
        /// </summary>
        [Browsable(false)]
        public virtual DataState DataState
        {
            get { return state; }
            private set
            {
                if (state != value)
                {
                    state = value;
                    OnStateChanged();
                }
            }
        }

        public StatefulObject()
        {
            MarkCreated();
        }

        /// <summary>
        /// Change the DataState to Created.
        /// </summary>
        public virtual void MarkCreated()
        {
            DataState = DataState.Created;
        }

        /// <summary>
        /// Change the DataState to Modified.
        /// </summary>
        public virtual void MarkModified()
        {
            DataState = DataState.Modified;
        }

        /// <summary>
        /// Change the DataState to Deleted.
        /// </summary>
        public virtual void MarkDeleted()
        {
            DataState = DataState.Deleted;
        }

        /// <summary>
        /// Change the DataState to Normal.
        /// </summary>
        public virtual void ResetState()
        {
            DataState = DataState.Normal;
        }

        /// <summary>
        /// Change the DataState to Initializing.
        /// </summary>
        public virtual void BeginInit()
        {
            DataState = DataState.Initializing;
        }

        /// <summary>
        /// Call ResetState().
        /// </summary>
        public virtual void EndInit()
        {
            ResetState();
        }

        protected override void OnPropertyChanging(string propertyName)
        {
            if (DataState == DataState.Initializing) return;
            base.OnPropertyChanging(propertyName);
        }

        protected internal override void OnPropertyChanged(string propertyName)
        {
            if (DataState == DataState.Initializing) return;
            base.OnPropertyChanged(propertyName);
        }

        protected override void OnValueChanged(string propertyName,
           object newValue, object oldValue)
        {
            if (DataState == DataState.Initializing) return;
            base.OnValueChanged(propertyName, newValue, oldValue);
            if (DataState == DataState.Normal)
                MarkModified();
        }

        [OnDeserializingAttribute]
        void OnDeserializing(StreamingContext context)
        {
            BeginInit();
        }

        [OnDeserializedAttribute]
        void OnDeserialized(StreamingContext context)
        {
            ResetState();
        }
    }
}
