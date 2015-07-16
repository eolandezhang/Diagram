using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace QPP.Parameter
{
    public class SysParameterCollectionValue<T> : List<T>, ISysParameterValue where T : SysParameterValue
    {
    }
}
