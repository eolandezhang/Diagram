using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.Linq.Expressions;
using System.Collections;
using QPP.Utils;
using System.ComponentModel.DataAnnotations;
using System.Reflection;

namespace QPP.ComponentModel
{
    [Serializable]
    [System.Runtime.Serialization.DataContract]
    public class ObservableObject : IObservableObject
    {
        //[NonSerialized]
        Hashtable valueTable = new Hashtable();

        [NonSerialized]
        PropertyChangedEventHandler propertyChanged;
        [NonSerialized]
        PropertyChangingEventHandler propertyChanging;
        [NonSerialized]
        EventHandler<ValueChangedEventArgs> valueChanged;
        [NonSerialized]
        bool suppressPropertyChanged = false;

        protected internal virtual Hashtable ValueTable
        {
            get { return valueTable; }
        }

        public virtual event EventHandler<ValueChangedEventArgs> ValueChanged
        {
            add
            {
                valueChanged = (EventHandler<ValueChangedEventArgs>)
                    System.Delegate.Combine(valueChanged, value);
            }
            remove
            {
                valueChanged = (EventHandler<ValueChangedEventArgs>)
                    System.Delegate.Remove(valueChanged, value);
            }
        }

        public virtual event PropertyChangingEventHandler PropertyChanging
        {
            add
            {
                propertyChanging = (PropertyChangingEventHandler)
                    System.Delegate.Combine(propertyChanging, value);
            }
            remove
            {
                propertyChanging = (PropertyChangingEventHandler)
                    System.Delegate.Remove(propertyChanging, value);
            }
        }

        public virtual event PropertyChangedEventHandler PropertyChanged
        {
            add
            {
                propertyChanged = (PropertyChangedEventHandler)
                    System.Delegate.Combine(propertyChanged, value);
            }
            remove
            {
                propertyChanged = (PropertyChangedEventHandler)
                    System.Delegate.Remove(propertyChanged, value);
            }
        }

        [EditorBrowsable(EditorBrowsableState.Advanced)]
        protected virtual void OnPropertyChanging(string propertyName)
        {
            if (!suppressPropertyChanged && propertyChanging != null)
                propertyChanging.Invoke(this, new PropertyChangingEventArgs(propertyName));
        }

        [EditorBrowsable(EditorBrowsableState.Advanced)]
        protected virtual internal void OnPropertyChanged(string propertyName)
        {
            if (!suppressPropertyChanged && propertyChanged != null)
            {
                propertyChanged.Invoke(this, new PropertyChangedEventArgs(propertyName));
                propertyChanged.Invoke(this, new PropertyChangedEventArgs("Item[]"));
            }
        }

        [EditorBrowsable(EditorBrowsableState.Advanced)]
        protected virtual void OnValueChanged(string propertyName,
            object newValue, object oldValue)
        {
            if (!suppressPropertyChanged && valueChanged != null)
                valueChanged.Invoke(this, new ValueChangedEventArgs(propertyName, newValue, oldValue));
        }
        /// <summary>
        /// 屬性索引器，可用於存取額外的屬性
        /// </summary>
        /// <param name="propertyName"></param>
        /// <returns></returns>
        public virtual object this[string propertyName]
        {
            get { return Get<object>(propertyName); }
            set { Set(propertyName, value); }
        }

        protected internal virtual T Get<T>(string property, ValueInitializer<T> initializer = null)
        {
            if (valueTable.ContainsKey(property))
                return (T)valueTable[property];
            if(initializer != null)
            {
                var value = initializer.Invoke();
                valueTable[property] = value;
                return value;
            }
            return default(T);
        }

        protected internal virtual bool Set<T>(string property, T newValue)
        {
            T oldValue = Get<T>(property);

            if (EqualityComparer<T>.Default.Equals(oldValue, newValue))
                return false;

            OnPropertyChanging(property);
            valueTable[property] = newValue;
            OnValueChanged(property, newValue, oldValue);
            OnPropertyChanged(property);
            return true;
        }

        /// <summary>
        /// 拷贝数据到目标，只触发一次通知事件。
        /// <para>目標可以是當前類型的父類或者子類</para>
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="destination"></param>
        /// <exception cref="ArgumentException">当前类型与目标类型不一致</exception>
        public virtual void CopyTo<T>(T destination) where T : ObservableObject
        {
            if (this == destination) return;
            Type sourceType = GetType();
            Type targetType = typeof(T);
            if (!sourceType.Equals(targetType) && !(this is T) && !targetType.IsSubclassOf(sourceType))
                throw new ArgumentException("当前类型{0}与目标类型{1}不一致".FormatArgs(sourceType, targetType));
            destination.valueTable.Clear();
            foreach (DictionaryEntry e in valueTable)
                destination.valueTable.Add(e.Key, ObjectHelper.Clone(e.Value));
            destination.OnPropertyChanged("");
        }
        /// <summary>
        /// 觸發屬性變更通知
        /// </summary>
        /// <param name="propertyName"></param>
        public virtual void RaisePropertyChanged(string propertyName)
        {
            OnPropertyChanged(propertyName);
        }
        /// <summary>
        /// 恢復屬性變更通知
        /// </summary>
        public virtual void ResumePropertyChanged()
        {
            suppressPropertyChanged = false;
        }
        /// <summary>
        /// 禁用屬性變更通知
        /// </summary>
        public virtual void SuppressPropertyChanged()
        {
            suppressPropertyChanged = true;
        }

        object ICloneable.Clone()
        {
            return CloneObject();
        }

        protected virtual object CloneObject()
        {
            var type = GetType();
            var m = (ObservableObject)Activator.CreateInstance(type);
            CopyTo(m);
            return m;
        }
    }
}
