using System;
using QPP.Wpf.UI.Controls.XGantt.Core.TimeUnitConverter;
namespace QPP.Wpf.UI.Controls.XGantt.Core
{
    
   public class TimeUnitScalar
    {
        static AbstractTimeUnitConverter converter { get; set; }
        static TimeUnitScalar()
        {
            converter = new TimeUnit24HConverter();
        }
        public static double ConvertToPixels(DateTime CurrentTime, TimeUnits timeUnit)
        {
           return converter.ConvertToPixels(CurrentTime,timeUnit);
        }
		public static double GetWidth(DateTime CurrentTime, DateTime time, TimeUnits timeUnit)
		{

			double result = TimeUnitScalar.ConvertToPixels(time, timeUnit);

			if (timeUnit == TimeUnits.Years)
			{
                if (CurrentTime.Year == time.Year)
                {
                    double DaysInYear = 365d;
                    if (DateTime.IsLeapYear(time.Year))
                        DaysInYear = 366;
                    result *= (double)(DaysInYear - time.DayOfYear + 1) / DaysInYear;
                }
			}
			else if (timeUnit == TimeUnits.Months)
			{
				if (CurrentTime.Month == time.Month)
					result *= (double)(DateTime.DaysInMonth(time.Year, time.Month) - time.Day + 1) / (double)DateTime.DaysInMonth(time.Year, time.Month);

			}
            else if (timeUnit == TimeUnits.Weeks)
            {
                int daysInWeek = 7;

                int weekCurrent = GetWeekOfYear(CurrentTime);
                int weekTime = GetWeekOfYear(time);


                if (weekTime == 52)
                {
                    daysInWeek = 7;

                    if (DateTime.IsLeapYear(time.Year))
                        daysInWeek++;

                    result *= (double)daysInWeek / 7d;
                }

                if (weekTime == 53)
                {
                    result = 0;

                }
                else if (weekCurrent == weekTime)
                {
                    daysInWeek = 7 - (CurrentTime.DayOfYear - (7 * (weekCurrent-1))) +2 ;
                    
                    result *= (double)daysInWeek  / 7d;
                }
            }
          

			return result;
		}
        public static int GetWeekOfYear(DateTime dateTime)
        {
            return ((int)Math.Ceiling((double)dateTime.DayOfYear / 7d) );
        }
        public static TimeSpan GetTimespan(DateTime CurrentTime, double distance)
        {
            if (distance == 0)
                return TimeSpan.Zero;

            double UnitWidth = ConvertToPixels(CurrentTime, TimeUnits.Hours);

            var units = (int)Math.Round(distance / UnitWidth);
            TimeSpan result = new TimeSpan(units, 0, 0);
         
            return result;


        }
		public static double GetPosition(DateTime CurrentTime, DateTime TargetTime)
		{
			double result = 0d;

			TimeSpan ts = TargetTime - CurrentTime.Date;

			double UnitWidth = TimeUnitScalar.ConvertToPixels(TargetTime, TimeUnits.Hours);
			result = (UnitWidth * ts.TotalHours);

			return result;
		}
		public static bool IsEquivolent(DateTime A, DateTime B, TimeUnits Scale)
		{
			TimeSpan diff = (A - B);

			switch (Scale)
			{
				case TimeUnits.Weeks:
					return Math.Abs(diff.TotalDays) < 7;

				case TimeUnits.Months:
					return Math.Abs(diff.TotalDays) < 30;

				case TimeUnits.Hours:
					return Math.Abs(diff.TotalHours) < 1;

				default:
				case TimeUnits.Days:
					diff = A.Date - B.Date;
					return Math.Abs(Math.Round(diff.TotalDays)) < 1;
			}

		}
	}
}
