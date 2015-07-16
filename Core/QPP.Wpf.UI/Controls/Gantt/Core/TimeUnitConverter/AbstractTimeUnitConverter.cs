using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace QPP.Wpf.UI.Controls.Gantt.Core.TimeUnitConverter
{
    public abstract class AbstractTimeUnitConverter
    {

        public abstract double ConvertToPixels(DateTime CurrentTime, TimeUnits timeUnit);
    }
}
