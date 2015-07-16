using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace QPP.Collections
{
    [Serializable]
    public class NameObjectCollection : NameObjectCollectionBase
    {
        public NameObjectCollection() { }

        public NameObjectCollection(IEqualityComparer equalityComparer) : base(equalityComparer) { }

        public NameObjectCollection(int capacity) : base(capacity) { }

        public NameObjectCollection(NameObjectCollection col)
        {
            foreach (string key in col.Keys)
                BaseAdd(key, col[key]);
        }

        public NameObjectCollection(int capacity, IEqualityComparer equalityComparer) : base(capacity, equalityComparer) { }

        public NameObjectCollection(int capacity, NameObjectCollection col)
            : base(capacity)
        {
            Add(col);
        }

        protected NameObjectCollection(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }

        public virtual string[] AllKeys { get { return BaseGetAllKeys(); } }

        public object this[int index] { get { return BaseGet(index); } }

        public object this[string name]
        {
            get { return BaseGet(name); }
            set { BaseSet(name, value); }
        }

        public void Add(NameObjectCollection col)
        {
            foreach (string key in col.Keys)
                BaseSet(key, col[key]);
        }

        public virtual void Clear()
        {
            BaseClear();
        }

        public virtual object Get(int index)
        {
            return BaseGet(index);
        }

        public virtual object Get(string name)
        {
            return BaseGet(name);
        }

        public virtual string GetKey(int index)
        {
            return BaseGetKey(index);
        }

        public virtual object[] GetValues(Type type)
        {
            return BaseGetAllValues(type);
        }

        public bool HasKeys()
        {
            return BaseHasKeys();
        }

        public virtual void Remove(string name)
        {
            BaseRemove(name);
        }

        public virtual void Set(string name, object value)
        {
            BaseSet(name, value);
        }
    }
}
