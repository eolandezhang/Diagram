using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace QPP.Data
{
    [Serializable]
    [DataContract]
    public class ParameterValue
    {
        object value;
        public ParameterValue(object value)
        {
            this.value = value;
        }
        public ParameterValue() : this(null) { }

        [DataMember]
        public virtual object Value
        {
            get { return this.value; }
            set { this.value = value; }
        }
        public override bool Equals(object obj)
        {
            if (obj == null)
                return false;
            if (!object.ReferenceEquals(this.GetType(), obj.GetType()))
                return false;
            return object.Equals(this.Value, ((ParameterValue)obj).Value);
        }
        public override int GetHashCode()
        {
            object value = Value;
            return value != null ? value.GetHashCode() : -1;
        }
        public static explicit operator ParameterValue(Boolean val)
        {
            return new ParameterValue(val);
        }
        public static implicit operator ParameterValue(Byte val)
        {
            return new ParameterValue(val);
        }
        public static implicit operator ParameterValue(Char val)
        {
            return new ParameterValue(val);
        }
        public static implicit operator ParameterValue(Decimal val)
        {
            return new ParameterValue(val);
        }
        public static implicit operator ParameterValue(Double val)
        {
            return new ParameterValue(val);
        }
        public static implicit operator ParameterValue(Single val)
        {
            return new ParameterValue(val);
        }
        public static implicit operator ParameterValue(Int16 val)
        {
            return new ParameterValue(val);
        }
        public static implicit operator ParameterValue(Int32 val)
        {
            return new ParameterValue(val);
        }
        public static implicit operator ParameterValue(Int64 val)
        {
            return new ParameterValue(val);
        }
        public static implicit operator ParameterValue(Guid val)
        {
            return new ParameterValue(val);
        }
        public static implicit operator ParameterValue(String val)
        {
            return new ParameterValue(val);
        }
        public static implicit operator ParameterValue(DateTime val)
        {
            return new ParameterValue(val);
        }
        public static implicit operator ParameterValue(TimeSpan val)
        {
            return new ParameterValue(val);
        }
        public static implicit operator ParameterValue(Byte[] val)
        {
            return new ParameterValue(val);
        }
        public ParameterValue Clone()
        {
            ICloneable cloneableValue = Value as ICloneable;
            if (cloneableValue != null)
                return new ParameterValue(cloneableValue.Clone());
            return new ParameterValue(Value);
        }
    }
}
