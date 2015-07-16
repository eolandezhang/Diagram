using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace QPP.Parameter
{
    public abstract class SysParameterValue : ISysParameterValue
    {
        protected internal abstract void Load(SysParameterValueModel m);
        protected internal abstract SysParameterValueModel Save();
    }
}
