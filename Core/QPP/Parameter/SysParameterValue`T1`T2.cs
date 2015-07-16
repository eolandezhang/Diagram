using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace QPP.Parameter
{
    public class SysParameterValue<T1, T2> : SysParameterValue
    {
        public SysParameterValue() { }
        public SysParameterValue(T1 value1, T2 value2)
        {
            Value1 = value1;
            Value2 = value2;
        }
        public T1 Value1 { get; set; }
        public T2 Value2 { get; set; }
        protected internal override void Load(SysParameterValueModel m)
        {
            Value1 = m.Value1.ConvertTo<T1>();
            Value2 = m.Value2.ConvertTo<T2>();
        }
        public override string ToString()
        {
            return Value1.ToSafeString() + Value2;
        }

        protected internal override SysParameterValueModel Save()
        {
            var m = new SysParameterValueModel();
            m.Value1 = Value1.ToSafeString();
            m.Value2 = Value2.ToSafeString();
            return m;
        }
    }
}
