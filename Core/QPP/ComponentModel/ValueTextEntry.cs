using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace QPP.ComponentModel
{
    [Serializable]
    [DataContract]
    public class ValueTextEntry : QPP.ComponentModel.DataModel, ICloneable, IComparable<ValueTextEntry>, IEquatable<ValueTextEntry>
    {
        [DataMember]
        public object Value 
        {
            get { return Get<object>("Value"); }
            set { Set("Value", value); }
        }
        [DataMember]
        public string Text
        {
            get { return Get<string>("Text"); }
            set { Set("Text", value); }
        }

        public ValueTextEntry() { }

        public ValueTextEntry(object value, string text)
        {
            Value = value;
            Text = text;
        }

        public override string ToString()
        {
            if (Text == null)
                return string.Empty;
            return Text;
        }

        public override bool Equals(object obj)
        {
            ValueTextEntry o = obj as ValueTextEntry;
            if (object.Equals(o, null))
                return false;
            return Equals(o);
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        public ValueTextEntry Clone()
        {
            return new ValueTextEntry(Value, Text);
        }

        object ICloneable.Clone()
        {
            return Clone();
        }

        public int CompareTo(ValueTextEntry other)
        {
            return ToString().CompareTo(other.Text);
        }

        public bool Equals(ValueTextEntry other)
        {
            return Value == other.Value && Text == other.Text;
        }

        public static bool operator ==(ValueTextEntry a, ValueTextEntry b)
        {
            bool isNull1 = object.Equals(a, null);
            bool isNull2 = object.Equals(b, null);
            if (isNull1 && isNull2)
                return true;
            if (isNull1 || isNull2)
                return false;
            return a.Equals(b);
        }

        public static bool operator !=(ValueTextEntry a, ValueTextEntry b)
        {
            return !(a == b);
        }
    }
}
