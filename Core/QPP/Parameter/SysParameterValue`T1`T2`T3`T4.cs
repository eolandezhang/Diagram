using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace QPP.Parameter
{
    public class SysParameterValue<T1, T2, T3, T4> : SysParameterValue
    {
        public SysParameterValue() { }
        public SysParameterValue(T1 value1, T2 value2, T3 value3, T4 value4)
        {
            Value1 = value1;
            Value2 = value2;
            Value3 = value3;
            Value4 = value4;
        }
        public T1 Value1 { get; set; }
        public T2 Value2 { get; set; }
        public T3 Value3 { get; set; }
        public T4 Value4 { get; set; }

        protected internal override void Load(SysParameterValueModel m)
        {
            Value1 = m.Value1.ConvertTo<T1>();
            Value2 = m.Value2.ConvertTo<T2>();
            Value3 = m.Value3.ConvertTo<T3>();
            Value4 = m.Value4.ConvertTo<T4>();
        }
        public override string ToString()
        {
            return Value1 + "," + Value2 + "," + Value3 + "," + Value4;
        }

        protected internal override SysParameterValueModel Save()
        {
            var m = new SysParameterValueModel();
            m.Value1 = Value1.ToSafeString();
            m.Value2 = Value2.ToSafeString();
            m.Value3 = Value3.ToSafeString();
            m.Value4 = Value4.ToSafeString();
            return m;
        }
    }
}
