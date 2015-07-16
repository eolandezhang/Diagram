using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace QPP.Wpf.UI.Controls.XGantt
{
    public class AccurateToHourCalculator : ICalendarCalculator
    {
        GanttCalendar calendar;
        public AccurateToHourCalculator(GanttCalendar cal)
        {
            calendar = cal;
        }

        /// <summary>
        /// 如果时间没有处在上班时段，获得其相应的上下班时刻
        /// </summary>
        /// <param name="date"></param>
        /// <returns></returns>
        public DateTime GetCorrectTime(DateTime date)
        {
            //时间 <= 上午上班时刻
            if (TimeCompare(date, calendar.firstHaftWorkStartTime) < 0)
                date = calendar.SetWithFirstHaftWorkStartTime(date);
            // 上午下班时刻 <= 时间 <= 下午上班时刻
            else if (TimeCompare(date, calendar.firstHaftWorkEndTime) >= 0 && TimeCompare(date, calendar.secondHaftWorkStartTime) < 0)
                date = calendar.SetWithSecondHaftWorkStartTime(date);
            //时间 >= 下午下班时刻
            else if (TimeCompare(date, calendar.secondHaftWorkEndTime) >= 0)
                date = calendar.SetWithSecondHaftWorkEndTime(date);
            return date;
        }


        public DateTime GetStartDateTime(DateTime end, double workHours)
        {
            if (workHours == 0) return end;

            int days = (int)(workHours / calendar.workHoursNumber);
            var date = GetStartDateWithoutTime(end, days);

            var hours = workHours % calendar.workHoursNumber;//获得剩余工时
            var begin = SubtractWorkHoursToDateTime(date, hours);
            //如果开始时间等于一天的下午下班时刻，使开始时间等于下个工作日的上午上班时刻
            if (TimeCompare(begin, calendar.secondHaftWorkEndTime) >= 0)
            {
                begin = calendar.SetWithFirstHaftWorkStartTime(begin);
                begin = calendar.GetDate(begin.AddDays(1), false);
            }
            return begin;
        }
        public DateTime GetEndDateTime(DateTime begin, double workHours)
        {
            if (workHours == 0) return begin;
            if (TimeCompare(begin, calendar.secondHaftWorkEndTime) >= 0)
            {
                begin = calendar.SetWithSecondHaftWorkEndTime(begin);
            }
            int days = (int)(workHours / calendar.workHoursNumber);
            var hours = workHours % calendar.workHoursNumber;
            var date = GetEndDateWithoutTime(begin, days);
            var end = AddWorkHoursToDateTime(date, hours);
            end = GetCorrectTime(end);
            //如果结束时间等于一天的上午上班时刻，使开始时间等于上个工作日的下午下班时刻
            if (TimeCompare(end, calendar.firstHaftWorkStartTime) == 0)
            {
                end = calendar.SetWithSecondHaftWorkEndTime(end);
                end = calendar.GetDate(end.AddDays(-1), true);
            }
            return end;
        }

        public DateTime GetStartDateWithoutTime(DateTime end, int days)
        {
            var holiday = 0;
            DateTime result = end;
            for (int i = 0; i <= days + holiday; i++)
            {
                result = end.AddDays(-i);
                if (!calendar.IsWorking(result))
                    holiday++;
            }
            return result;
        }
        public DateTime GetEndDateWithoutTime(DateTime begin, int days)
        {
            var holiday = 0;
            DateTime result = begin;
            for (int i = 0; i <= days + holiday; i++)
            {
                result = begin.AddDays(i);
                if (!calendar.IsWorking(result))
                    holiday++;
            }
            return result;
        }

        public DateTime SubtractWorkHoursToDateTime(DateTime date, double workHours)
        {
            if (workHours <= 0) return date;
            TimeSpan ts = GetDifferenceTimeToWorkStartTime(date);
            //减去剩余工时，还在date的工作日
            if (workHours <= ts.TotalHours)
                return SubtractWorkHours(date, workHours);
            //减去剩余工时，跨到上一个工作日
            else
            {
                var tempdate = SubtractWorkHours(date, workHours);
                return calendar.GetDate(tempdate.AddDays(-1), false);
            }
        }

        public DateTime AddWorkHoursToDateTime(DateTime date,double workHours)
        {
            if (workHours <= 0) return date;
            TimeSpan ts = GetDifferenceTimeToWorkEndTime( date);
            //加上剩余工时，还在date当前工作日
            if ( workHours <= ts.TotalHours )
                return AddWorkHours(date, workHours);
            //加上剩余工时，跨到上一个工作日
            else
            {
                //如果date加上剩余工时会跨到第二天
                //int h = (int)calendar.workHoursNumber / 1;
                //int m = (int)calendar.workHoursNumber * 60 % 60;
                //int s = (int)calendar.workHoursNumber * 3600 % 3600;
                //var ts2 = new TimeSpan(h, m, s);
                //ts = ts - ts2;
                var tempdate = AddWorkHours(date, workHours);
                return calendar.GetDate(tempdate.AddDays(1),false);
            }
        }

        /// <summary>
        /// 获得date的时间距离下班时刻的时间差
        /// </summary>
        public TimeSpan GetDifferenceTimeToWorkEndTime(DateTime date)
        {
            //将date的日期改为与firstHaftWorkStartTime的日期一样，但date的时分秒不变
            date = GanttCalendar.ChangeOnlyDate(date, calendar.firstHaftWorkStartTime);

            //时间 <= 上午上班时刻
            if (TimeCompare(date, calendar.firstHaftWorkStartTime) <= 0)
            {
                int h = (int)calendar.workHoursNumber / 1;
                int m = (int)calendar.workHoursNumber * 60 % 60;
                int s = (int)calendar.workHoursNumber * 3600 % 3600;
                return new TimeSpan(h, m, s);
            }
            //上午上班时刻 < 时间 < 上午下班班时刻
            if (TimeCompare(date, calendar.firstHaftWorkStartTime) > 0 && TimeCompare(date, calendar.firstHaftWorkEndTime) < 0)
            {
                return (calendar.firstHaftWorkEndTime - date) + (calendar.secondHaftWorkEndTime - calendar.secondHaftWorkStartTime);
            }
            // 上午下班时刻 <= 时间 <= 下午上班时刻
            else if (TimeCompare(date, calendar.firstHaftWorkEndTime) >= 0 && TimeCompare(date, calendar.secondHaftWorkStartTime) <= 0)
            {
                return calendar.secondHaftWorkEndTime - calendar.secondHaftWorkStartTime;
            }
            // 下午上班时刻 < 时间 < 下午下班时刻
            else if (TimeCompare(date, calendar.secondHaftWorkStartTime) > 0 && TimeCompare(date, calendar.secondHaftWorkEndTime) < 0)
            {
                return calendar.secondHaftWorkEndTime - date;
            }
            //时间 >= 下午下班时刻
            return new TimeSpan(0, 0, 0);
            
        }

        /// <summary>
        /// 获得date的时间距离上班时刻的时间差
        /// </summary>
        public TimeSpan GetDifferenceTimeToWorkStartTime(DateTime date)
        {
            //将date的日期改为与firstHaftWorkStartTime的日期一样，但date的时分秒不变
            date = GanttCalendar.ChangeOnlyDate(date, calendar.firstHaftWorkStartTime);

            // 时间 >= 下午下班时刻
            if (TimeCompare(date, calendar.secondHaftWorkEndTime) >= 0)
                //if (TimeCompare(date, calendar.firstHaftWorkStartTime) <= 0)
            {
                int h = (int)calendar.workHoursNumber / 1;
                int m = (int)calendar.workHoursNumber * 60 % 60;
                int s = (int)calendar.workHoursNumber * 3600 % 3600;
                return new TimeSpan(h, m, s);
            }
            // 下午下班时刻 > 时间 > 下午上班时刻
            else if (TimeCompare(date, calendar.secondHaftWorkStartTime) > 0 && TimeCompare(date, calendar.secondHaftWorkEndTime) < 0)
            {
                return (calendar.secondHaftWorkEndTime - date) + (calendar.firstHaftWorkEndTime - calendar.firstHaftWorkStartTime);
            }
            // 下午上班时刻 >= 时间 >= 上午下班时刻
            else  if (TimeCompare(date, calendar.firstHaftWorkEndTime) >= 0 && TimeCompare(date, calendar.secondHaftWorkStartTime) <= 0)
            {
                return calendar.firstHaftWorkEndTime - calendar.firstHaftWorkStartTime;
            }
            //上午下班班时刻 > 时间 > 上午上班时刻
            else if (TimeCompare(date, calendar.firstHaftWorkStartTime) > 0 && TimeCompare(date, calendar.firstHaftWorkEndTime) < 0)
            {
                return date - calendar.firstHaftWorkStartTime ;
            }
            //上午上班时刻 >=时间
            return new TimeSpan(0, 0, 0);

        }


        public double GetWorkHours(DateTime begin, DateTime end)
        {
            if (begin > end) return 0;
            double duraiton = 0;
            DateTime dt = begin;
            while (dt.Date < end.Date)
            {
                if (calendar.IsWorking(dt))
                    duraiton++;
                dt = dt.AddDays(1);
            }
            double hours = duraiton * 8;
            var ts1 = GetDifferenceTimeToWorkEndTime(dt);
            var ts2 = GetDifferenceTimeToWorkEndTime(end);
            hours += ts1.TotalHours - ts2.TotalHours;
            return hours;
        }

        //如果是夜班，则要修改算法
        public DateTime AddTimeSpan(DateTime date, TimeSpan ts)
        {
            if (ts.TotalHours <= 0) return date;
            
            if (date.Date != calendar.firstHaftWorkStartTime)
                date = GanttCalendar.ChangeOnlyDate(date, calendar.firstHaftWorkStartTime);

            //如果 时间 >= 下午下班时刻 ，使 时间 = 上午上班时刻
            if (TimeCompare(date, calendar.secondHaftWorkEndTime) >= 0)
                date = calendar.SetWithFirstHaftWorkStartTime(date);
            //如果 时间 <= 上午上班时刻 ，使 时间 = 上午上班时刻
            if (TimeCompare(date, calendar.firstHaftWorkStartTime) <= 0)
                date = calendar.SetWithFirstHaftWorkStartTime(date);
            //如果 上午下班时刻 <= 时间 <= 下午上班时刻 ，使 时间 = 下午上班时刻
            if (TimeCompare(date, calendar.firstHaftWorkEndTime) >= 0 && TimeCompare(date, calendar.secondHaftWorkStartTime) <= 0)
                date = calendar.SetWithSecondHaftWorkStartTime(date);
            
            //上午上班时刻 <= 时间 < 上午下班班时刻
            if (TimeCompare(date, calendar.firstHaftWorkStartTime) >= 0 && TimeCompare(date, calendar.firstHaftWorkEndTime) < 0)
            {
                if (ts.TotalHours <= (calendar.firstHaftWorkEndTime - date).TotalHours)
                    return date.Add(ts);
                else
                {
                    var ts2 = calendar.firstHaftWorkEndTime - date;
                    date = date.Add(ts2);
                    ts = ts - ts2;
                    return AddTimeSpan(date, ts);
                }
            }
            //下午上班时刻 <= 时间 < 下午下班班时刻
            else if (TimeCompare(date, calendar.secondHaftWorkStartTime) >= 0 && TimeCompare(date, calendar.secondHaftWorkEndTime) < 0)
            {
                if (ts.TotalHours <= (calendar.secondHaftWorkEndTime - date).TotalHours)
                    return date.Add(ts);
                else
                {
                    var ts2 = calendar.secondHaftWorkEndTime - date;
                    date = date.Add(ts2);
                    ts = ts - ts2;
                    return AddTimeSpan(date, ts);
                }
            }
            return date;
        }

        public DateTime AddWorkHours(DateTime targetDate,double workHours)
        {
            int h = (int)workHours / 1;
            int m = (int)workHours * 60 % 60;
            int s = (int)workHours * 3600 % 3600;
            TimeSpan ts = new TimeSpan(h, m, s);
            var date = AddTimeSpan(targetDate, ts);
            return targetDate.Date.AddHours(date.Hour).AddMinutes(date.Minute).AddSeconds(date.Second);

        }

        public DateTime SubtractWorkHours(DateTime targetDate, double workHours)
        {
            int h = (int)workHours / 1;
            int m = (int)workHours * 60 % 60;
            int s = (int)workHours * 3600 % 3600;
            TimeSpan ts = new TimeSpan(h, m, s);
            var date = SubtractTimeSpan(targetDate, ts);
            return targetDate.Date.AddHours(date.Hour).AddMinutes(date.Minute).AddSeconds(date.Second);

        }

        //如果是夜班，则要修改算法
        public DateTime SubtractTimeSpan(DateTime date, TimeSpan ts)
        {
            if (ts.TotalHours <= 0) return date;
            
            if (date.Date != calendar.firstHaftWorkStartTime)
                date = GanttCalendar.ChangeOnlyDate(date, calendar.firstHaftWorkStartTime);

            //如果 时间 >= 下午下班时刻 ，使 时间 = 下午下班时刻
            if (TimeCompare(date, calendar.secondHaftWorkEndTime) >= 0)
                date = calendar.SetWithSecondHaftWorkEndTime(date);
            //如果 时间 <= 上午上班时刻 ，使 时间 = 下午下班时刻
            if (TimeCompare(date, calendar.firstHaftWorkStartTime) <= 0)
                date = calendar.SetWithSecondHaftWorkEndTime(date);

            //如果 上午下班时刻 <= 时间 <= 下午上班时刻 ，使 时间 = 上午下班时刻
            if (TimeCompare(date, calendar.firstHaftWorkEndTime) >= 0 && TimeCompare(date, calendar.secondHaftWorkStartTime) <= 0)
                date = calendar.SetWithFirstHaftWorkEndTime(date);

            //下午下班时刻 >= 时间 > 下午上班时刻
            if (TimeCompare(date, calendar.secondHaftWorkStartTime) > 0 && TimeCompare(date, calendar.secondHaftWorkEndTime) <= 0)
            {
                if (ts.TotalHours <= (date - calendar.secondHaftWorkStartTime).TotalHours)
                    return date.Add(-ts);
                else
                {
                    var ts2 = date - calendar.secondHaftWorkStartTime;
                    date = date.Add(-ts2);
                    ts = ts - ts2;
                    return SubtractTimeSpan(date, ts);
                }
            }

            //上午下班班时刻 >= 时间 > 上午上班时刻
            if (TimeCompare(date, calendar.firstHaftWorkStartTime) > 0 && TimeCompare(date, calendar.firstHaftWorkEndTime) <= 0)
            {
                if (ts.TotalHours <= (date - calendar.firstHaftWorkStartTime).TotalHours)
                    return date.Add(-ts);
                else
                {
                    var ts2 = date - calendar.firstHaftWorkStartTime;
                    date = date.Add(-ts2);
                    ts = ts - ts2;
                    return SubtractTimeSpan(date, ts);
                }
            }
            return date;
        }


        int TimeCompare(DateTime dateTime1, DateTime dateTime2)
        {
            dateTime1 = GanttCalendar.ChangeOnlyDate(dateTime1, DateTime.Now);
            dateTime2 = GanttCalendar.ChangeOnlyDate(dateTime2, DateTime.Now);
            TimeSpan ts = dateTime1 - dateTime2;
            return (int)ts.TotalSeconds;

        }
    }
}
