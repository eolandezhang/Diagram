using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace QPP.Parameter
{
    public class SysParameterValue<T> : SysParameterValue
    {
        public SysParameterValue() { }
        public SysParameterValue(T value) { Value = value; }
        public T Value { get; set; }
        protected internal override void Load(SysParameterValueModel m)
        {
            Value = m.Value1.ConvertTo<T>();
        }
        public override string ToString()
        {
            return Value.ToSafeString();
        }

        protected internal override SysParameterValueModel Save()
        {
            var m = new SysParameterValueModel();
            m.Value1 = Value.ToSafeString();
            return m;
        }
    }
}
