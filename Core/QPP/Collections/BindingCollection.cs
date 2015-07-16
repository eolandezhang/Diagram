using QPP.ComponentModel;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace QPP.Collections
{
    /// <summary>
    /// 實現綁定接口IBindingList的列表
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class BindingCollection<T> : ObservableCollection<T>, IBindingList
    {
        public delegate T ObjectFactory();

        public event ListChangedEventHandler ListChanged;

        public ObjectFactory ItemCreator { get; set; }

        [NonSerialized]
        PropertyDescriptor m_PropertyDescriptor;
        [NonSerialized]
        ListSortDirection m_ListSortDirection;

        [NonSerialized]
        bool suppressPropertyChanged = false;

        protected override void ClearItems()
        {
            base.ClearItems();
            OnListChanged(new ListChangedEventArgs(ListChangedType.Reset, 0));
        }

        protected override void InsertItem(int index, T item)
        {
            base.InsertItem(index, item);
            OnListChanged(new ListChangedEventArgs(ListChangedType.ItemAdded, index));
        }

        protected override void RemoveItem(int index)
        {
            base.RemoveItem(index);
            OnListChanged(new ListChangedEventArgs(ListChangedType.ItemDeleted, index));
        }

        protected override void SetItem(int index, T item)
        {
            base.SetItem(index, item);
            OnListChanged(new ListChangedEventArgs(ListChangedType.ItemChanged, index));
        }

        protected override void MoveItem(int oldIndex, int newIndex)
        {
            base.MoveItem(oldIndex, newIndex);
            OnListChanged(new ListChangedEventArgs(ListChangedType.ItemMoved, newIndex, oldIndex));
        }

        protected override void OnCollectionChanged(NotifyCollectionChangedEventArgs e)
        {
            if (!suppressPropertyChanged)
                base.OnCollectionChanged(e);
        }

        public void RaiseCollectionChanged()
        {
            base.OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
            OnListChanged(new ListChangedEventArgs(ListChangedType.Reset, 0));
        }

        /// <summary>
        /// 清空后把数据加载到集合中，加载完成后触发CollectionChanged事件
        /// </summary>
        /// <param name="source"></param>
        public virtual void Load(IEnumerable<T> source)
        {
            Items.Clear();
            foreach (var item in source)
                Items.Add(item);
            OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
            OnListChanged(new ListChangedEventArgs(ListChangedType.Reset, 0));
            OnPropertyChanged(new PropertyChangedEventArgs("Count"));
        }

        /// <summary>
        /// 把数据加载到集合中，加载完成后触发CollectionChanged事件
        /// </summary>
        /// <param name="source"></param>
        public void AddRange(IEnumerable<T> source)
        {
            foreach (var item in source)
                Items.Add(item);
            OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
            OnListChanged(new ListChangedEventArgs(ListChangedType.Reset, 0));
            OnPropertyChanged(new PropertyChangedEventArgs("Count"));
        }

        public virtual void ResumePropertyChanged()
        {
            suppressPropertyChanged = false;
        }

        public virtual void SuppressPropertyChanged()
        {
            suppressPropertyChanged = true;
        }

        public string Sort
        {
            get
            {
                if (m_PropertyDescriptor != null)
                    return m_PropertyDescriptor.Name + (m_ListSortDirection == ListSortDirection.Descending ? " DESC" : " ASC");
                return null;
            }
        }

        protected virtual void OnListChanged(ListChangedEventArgs e)
        {
            if (ListChanged != null)
                ListChanged(this, e);
        }

        #region IBindingList

        void IBindingList.AddIndex(PropertyDescriptor property)
        {
            OnListChanged(new ListChangedEventArgs(ListChangedType.PropertyDescriptorAdded, property));
        }

        object IBindingList.AddNew()
        {
            if (ItemCreator != null)
                return ItemCreator();
            var t = typeof(T);
            if (!t.IsAbstract && !t.IsInterface)
                return Activator.CreateInstance(t);
            return default(T);
        }

        bool IBindingList.AllowEdit
        {
            get { return true; }
        }

        bool IBindingList.AllowNew
        {
            get { return true; }
        }

        bool IBindingList.AllowRemove
        {
            get { return true; }
        }

        void IBindingList.ApplySort(PropertyDescriptor property, ListSortDirection direction)
        {
            bool isChanged = m_PropertyDescriptor != property || m_ListSortDirection != direction;
            m_PropertyDescriptor = property;
            m_ListSortDirection = direction;
            if (isChanged)
                OnListChanged(new ListChangedEventArgs(ListChangedType.PropertyDescriptorChanged, property));
        }

        int IBindingList.Find(PropertyDescriptor property, object key)
        {
            foreach (var item in this)
            {
                if (item is IObservableObject)
                {
                    if (object.Equals(((IObservableObject)item)[property.Name], key))
                        return IndexOf(item);
                }
                else
                {
                    if (object.Equals(property.GetValue(item), key))
                        return IndexOf(item);
                }
            }
            return -1;
        }

        bool IBindingList.IsSorted
        {
            get { return m_PropertyDescriptor != null; }
        }

        event ListChangedEventHandler IBindingList.ListChanged
        {
            add { ListChanged += value; }
            remove { ListChanged -= value; }
        }

        void IBindingList.RemoveIndex(PropertyDescriptor property)
        {
            OnListChanged(new ListChangedEventArgs(ListChangedType.PropertyDescriptorDeleted, property));
        }

        void IBindingList.RemoveSort()
        {
            m_PropertyDescriptor = null;
        }

        ListSortDirection IBindingList.SortDirection
        {
            get { return m_ListSortDirection; }
        }

        PropertyDescriptor IBindingList.SortProperty
        {
            get { return m_PropertyDescriptor; }
        }

        bool IBindingList.SupportsChangeNotification
        {
            get { return true; }
        }

        bool IBindingList.SupportsSearching
        {
            get { return true; }
        }

        bool IBindingList.SupportsSorting
        {
            get { return true; }
        }

        #endregion
    }
}
