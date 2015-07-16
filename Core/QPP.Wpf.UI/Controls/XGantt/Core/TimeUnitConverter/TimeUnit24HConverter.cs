using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace QPP.Wpf.UI.Controls.XGantt.Core.TimeUnitConverter
{
    public class TimeUnit24HConverter : AbstractTimeUnitConverter
    {
        public override double ConvertToPixels(DateTime CurrentTime, TimeUnits timeUnit)
        {
            //double TickWidth = 2.38E-11;
            double HourWidth = 1.6;// 0.7;// TickWidth * 8.64E11 / 24d;
            double TickWidth = HourWidth * 24d / 8.64E11;
            double DayWidth = HourWidth * 24;
            switch (timeUnit)
            {
                case TimeUnits.Years:
                    double DaysInYear = 365d;
                    if (DateTime.IsLeapYear(CurrentTime.Year))
                        DaysInYear = 366;
                    double YearWidth = DayWidth * DaysInYear;

                    return YearWidth * Zoom.Value;
                case TimeUnits.Months:
                    double MonthWidth = DayWidth * DateTime.DaysInMonth(CurrentTime.Year, CurrentTime.Month);

                    return MonthWidth * Zoom.Value;
                case TimeUnits.Weeks:
                    double WeekWidth = DayWidth * 7;
                    return WeekWidth * Zoom.Value;
                case TimeUnits.Days:
                    return DayWidth * Zoom.Value;
                case TimeUnits.Hours:
                    return HourWidth * Zoom.Value;
                default:
                    return TickWidth * Zoom.Value;
            }
        }
    }
    
}
