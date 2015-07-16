using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.Serialization;
using QPP.Filtering;

namespace QPP.ComponentModel
{
    [Serializable]
    [DataContract]
    public class EntityId : INotifyPropertyChanged, INotifyPropertyChanging
    {
        public event PropertyChangedEventHandler PropertyChanged;

        public event PropertyChangingEventHandler PropertyChanging;

        [EditorBrowsable(EditorBrowsableState.Advanced)]
        protected virtual void OnPropertyChanging(string propertyName)
        {
            if (PropertyChanging != null)
                PropertyChanging.Invoke(this, new PropertyChangingEventArgs(propertyName));
        }

        [EditorBrowsable(EditorBrowsableState.Advanced)]
        protected virtual void OnPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
                PropertyChanged.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        Dictionary<string, object> m_Values = new Dictionary<string, object>();

        public EntityId()
        {

        }

        public EntityId(string property, object value)
        {
            m_Values[property] = value;
        }

        public static bool operator ==(EntityId a, EntityId b)
        {
            if (object.Equals(a, null)) return false;
            if (object.Equals(b, null)) return false;
            return a.Equals(b);
        }

        public static bool operator !=(EntityId a, EntityId b)
        {
            return !(a == b);
        }

        public override bool Equals(object obj)
        {
            if (m_Values.Count == 0) return false;
            var entityId = obj as EntityId;
            if (entityId == null) return false;
            if (m_Values.Count != entityId.m_Values.Count) return false;
            foreach (var v in m_Values)
            {
                if (!entityId.m_Values.ContainsKey(v.Key))
                    return false;
                if (!object.Equals(v.Value, entityId.m_Values[v.Key]))
                    return false;
            }
            return true;
        }

        public override int GetHashCode()
        {
            return ToQueryString().GetHashCode();
        }

        [DataMember]
        public Dictionary<string, object> Values
        {
            get { return m_Values; }
            set { m_Values = value; }
        }

        public object this[string propertyName]
        {
            get
            {
                if (m_Values.ContainsKey(propertyName))
                    return m_Values[propertyName];
                else
                {
                    return null;
                }
            }
            set
            {
                if (!object.Equals(this[propertyName], value))
                {
                    OnPropertyChanging("Item[]");
                    OnPropertyChanging(propertyName);
                    m_Values[propertyName] = value;
                    OnPropertyChanged("Item[]");
                    OnPropertyChanged(propertyName);
                }
            }
        }

        public CriteriaOperator GetCriteria()
        {
            GroupOperator group = new GroupOperator(GroupOperatorType.And);
            foreach (var v in m_Values)
                group.Operands.Add(new BinaryOperator(v.Key, v.Value));
            return group;
        }

        public override string ToString()
        {
            return GetCriteria().ToString();
        }

        public virtual string ToQueryString()
        {
            if (m_Values.Count == 0) return "";
            var str = "?";
            foreach (var v in m_Values)
                str += v.Key + "=" + v.Value + "&";
            return str.TrimEnd('&');
        }

        public static EntityId Parse(string str)
        {
            EntityId id = new EntityId();
            var c = CriteriaOperator.Parse(str);
            if (c is GroupOperator)
            {
                foreach (BinaryOperator o in ((GroupOperator)c).Operands)
                {
                    var p = o.LeftOperand as OperandProperty;
                    var v = o.RightOperand as OperandValue;
                    id[p.PropertyName] = v.Value;
                }
            }
            else if (c is BinaryOperator)
            {
                var o = c as BinaryOperator;
                var p = o.LeftOperand as OperandProperty;
                var v = o.RightOperand as OperandValue;
                id[p.PropertyName] = v.Value;
            }
            return id;
        }

        public static EntityId GetId(object obj)
        {
            EntityId id = new EntityId();
            foreach (var p in obj.GetType().GetProperties())
            {
                var attr = p.GetCustomAttributes(typeof(System.ComponentModel.DataAnnotations.KeyAttribute), true);
                if (attr != null && attr.Length > 0)
                    id[p.Name] = p.GetValue(obj, null);
            }
            return id;
        }
    }
}
