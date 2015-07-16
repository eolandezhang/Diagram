using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace QPP.Wpf.UI.Controls.XGantt
{
    interface ICalendarCalculator
    {
        DateTime GetEndDateTime(DateTime begin, double workHours);
        DateTime GetStartDateTime(DateTime end, double workHours);
         double GetWorkHours(DateTime begin, DateTime end);
    }
}
