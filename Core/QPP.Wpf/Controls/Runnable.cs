using QPP.ComponentModel;
using QPP.Runtime;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;

namespace QPP.Wpf.Controls
{
    public class Runnable : DependencyObject, IRunnable
    {
        public virtual void Run(DataArgs args)
        {

        }
    }
}
