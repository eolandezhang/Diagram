using QPP.Layout;
using QPP.Runtime;
using QPP.Wpf.Runtime;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace QPP.Wpf
{
    public class RuntimeContainers
    {
        public static void Initialize()
        {
            Current = new RuntimeContainer();
        }

        public static IRuntimeContainer Current { get; set; }
    }
}
