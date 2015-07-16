using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace QPP.Wpf.UI.Controls.XGantt
{
    public class GanttCalendar
    {
        Dictionary<DayOfWeek, bool> week = new Dictionary<DayOfWeek, bool>();
        Dictionary<DateTime, bool> specialDays = new Dictionary<DateTime, bool>();

        internal double workHoursNumber = 8;
        internal DateTime firstHaftWorkStartTime;//上午上班时刻   (只取时、分、秒用)
        internal DateTime firstHaftWorkEndTime;//上午下班时刻     (只取时、分、秒用)
        internal DateTime secondHaftWorkStartTime;//下午上班时刻  (只取时、分、秒用)
        internal DateTime secondHaftWorkEndTime;//下午下班时刻    (只取时、分、秒用)
        ICalendarCalculator calculator;
        void InitWorkStartTimeAndEndTime()
        {
            firstHaftWorkStartTime = new DateTime(1, 1, 1, 8, 0, 0);
            firstHaftWorkEndTime = new DateTime(1, 1, 1, 12, 30, 0);
            secondHaftWorkStartTime = new DateTime(1, 1, 1, 13, 30, 0);
            secondHaftWorkEndTime = new DateTime(1, 1, 1, 17, 0, 0);
        }
        public GanttCalendar()
        {
            week[DayOfWeek.Monday] = true;
            week[DayOfWeek.Tuesday] = true;
            week[DayOfWeek.Wednesday] = true;
            week[DayOfWeek.Thursday] = true;
            week[DayOfWeek.Friday] = true;
            week[DayOfWeek.Saturday] = false;
            week[DayOfWeek.Sunday] = false;
            InitWorkStartTimeAndEndTime();
            calculator = new AccurateToHourCalculator(this);
        }

        public void ClearSpecialDays()
        {
            specialDays.Clear();
        }

        /// <summary>
        /// 设置一周的某天是否工作
        /// </summary>
        /// <param name="day"></param>
        /// <param name="isWorking"></param>
        /// <returns></returns>
        public GanttCalendar SetWorking(DayOfWeek day, bool isWorking)
        {
            week[day] = isWorking;
            return this;
        }
        /// <summary>
        /// 设置指定日期是否工作
        /// </summary>
        /// <param name="day"></param>
        /// <param name="isWorking"></param>
        /// <returns></returns>
        public GanttCalendar SetWorking(DateTime day, bool isWorking)
        {
            specialDays[day.Date] = isWorking;
            return this;
        }

        public bool IsWorking(DateTime dt)
        {
            if(specialDays.ContainsKey(dt.Date))
                return specialDays[dt.Date];
            return week[dt.DayOfWeek];
        }

        public DateTime GetOffsetingDate(DateTime oldDate, double duration)
        {
            if (duration > 0)
                return GetEndDate(oldDate,duration+1);
            if (duration < 0)
                return GetStartDate(oldDate, -duration+1);
            else
                return oldDate;
        }
        public DateTime GetStartDate(DateTime end, double duration)
        {
            var holiday = 0;
            DateTime result = end;
            for (int i = 1; i < duration + holiday; i++)
            {
                result = end.AddDays(-i);
                if (!IsWorking(result))
                    holiday++;
            }
            return result;
        }

        

        /// <summary>
        /// 根据任务的结束时间和工时，计算出开始时间
        /// </summary>
        public DateTime GetStartDateTime( DateTime end, double workHours)
        {
            return calculator.GetStartDateTime(end, workHours);
        }

        /// <summary>
        /// 根据任务的结束时间和工时，计算出开始时间
        /// </summary>
        public DateTime GetStartDateTime(DateTime begin, DateTime end, double workHours)
        {
            //参数end用于检测是否是就数据
            //if (IsOldData(begin, end))//兼容旧数据
            //    return GetStartDate(end, workHours / workHoursNumber);
            return calculator.GetStartDateTime(end, workHours);
        }

        public DateTime GetEndDate(DateTime begin, double duration)
        {
            var holiday = 0;
            DateTime result = begin;
            for (int i = 1; i < duration + holiday; i++)
            {
                result = begin.AddDays(i);
                if (!IsWorking(result))
                    holiday++;
            }
            return result;
        }

        /// <summary>
        /// 检测是否是旧数据
        /// </summary>
        //public bool IsOldData(DateTime begin, DateTime end)
        //{
        //    return false;
        //    //begin和end 必须是某个任务的开始时间和结束时间
        //    return IsTimeEqual(begin, new DateTime(2014, 1, 1, 0, 0, 0))
        //        && IsTimeEqual(end, new DateTime(2014, 1, 1, 0, 0, 0));
        //}

        //bool IsOldData(DateTime begin)
        //{
        //    return false;
        //    return IsTimeEqual( begin,new DateTime(2014, 1, 1, 0, 0, 0) );       
        //}
        

        /// <summary>
        /// 根据任务的开始时间和工时，计算出结束时间
        /// </summary>
        public DateTime GetEndDateTime(DateTime begin, DateTime end, double workHours)
        {
            //参数end用于检测是否是就数据
            //if (IsOldData(begin,end))//兼容旧数据
            //    return GetEndDate(begin, workHours / workHoursNumber);
            return calculator.GetEndDateTime(begin, workHours);
        }

        /// <summary>
        /// 根据任务的开始时间和工时，计算出结束时间
        /// </summary>
        public DateTime GetEndDateTime(DateTime begin, double workHours)
        {
            return calculator.GetEndDateTime(begin, workHours);
        }

 
        /// <summary>
        /// 比较2个日期时间是否相等，不比较日期
        /// </summary>
        bool IsTimeEqual(DateTime dateTime1, DateTime dateTime2)
        {
            if (dateTime1.Hour == dateTime2.Hour && dateTime1.Minute == dateTime2.Minute
                && dateTime1.Second == dateTime2.Second)
                return true;
            else
                return false;
        }

        public static DateTime ChangeOnlyDate(DateTime targetDate, DateTime changer)
        {
            return changer.Date.AddHours(targetDate.Hour)
                             .AddMinutes(targetDate.Minute)
                             .AddSeconds(targetDate.Second);
        }


        public double GetDuration(DateTime begin, DateTime end)
        {
            double duraiton = 0;
            DateTime dt = begin.Date;
            while (dt <= end.Date)
            {
                if (IsWorking(dt))
                    duraiton++;
                dt = dt.AddDays(1);
            }
            return duraiton;
        }

        public double GetWorkHours(DateTime begin, DateTime end)
        {
            return calculator.GetWorkHours(begin, end);
        }

        public double GetWorkHours(DateTime begin, DateTime end, double duration)
        {
            //if (IsOldData(begin, end))
            //    return duration * workHoursNumber;
            //else
                return calculator.GetWorkHours(begin,end);
        }


        /// <summary>
        /// 获取有效工作日
        /// </summary>
        /// <param name="date">开始或者结束日期，由inverse决定</param>
        /// <param name="inverse"></param>
        /// <returns></returns>
        public DateTime GetDate(DateTime date, bool inverse)
        {
            var dt = date;
            
            while (!IsWorking(dt))
            {
                if (inverse)
                    dt = dt.AddDays(-1);
                else
                    dt = dt.AddDays(1);
            }
            return dt;
        }

        /// <summary>
        /// 将时间设置为上午上班时刻
        /// </summary>
        public DateTime SetWithFirstHaftWorkStartTime(DateTime date)
        {
            return SetWithWorkStartOrEndTime(date, firstHaftWorkStartTime);
        }
        /// <summary>
        /// 将时间设置为上午下班时刻
        /// </summary>
        public DateTime SetWithFirstHaftWorkEndTime(DateTime date)
        {
            return SetWithWorkStartOrEndTime(date, firstHaftWorkEndTime);
        }
        /// <summary>
        /// 将时间设置为下午上班时刻
        /// </summary>
        public DateTime SetWithSecondHaftWorkStartTime(DateTime date)
        {
            return SetWithWorkStartOrEndTime(date, secondHaftWorkStartTime);
        }
        /// <summary>
        /// 将时间设置为下午下班时刻
        /// </summary>
        public DateTime SetWithSecondHaftWorkEndTime(DateTime date)
        {
            return SetWithWorkStartOrEndTime(date, secondHaftWorkEndTime);
        }

        DateTime SetWithWorkStartOrEndTime(DateTime date, DateTime workTime)
        {
            return date.Date.AddHours(workTime.Hour).AddMinutes(workTime.Minute).AddSeconds(workTime.Second);
        }


        public DateTime GetSuitableStartTime(DateTime begin)
        {
            //如果是上午下班时刻，就返回下午上班时刻
            if (IsTimeEqual(begin, firstHaftWorkEndTime))
                return SetWithSecondHaftWorkStartTime(begin);
            //如果是下午下班时刻，就返回下个工作日上班时刻
            if (IsTimeEqual(begin, secondHaftWorkEndTime))
            {
                //return GetDate(SetWithFirstHaftWorkStartTime(begin.AddDays(1)), false);
                var date = GetDate(begin.AddDays(1), false);
                date = SetWithFirstHaftWorkStartTime(date);
                return date;
            }
            return begin;
        }
        
        /// <summary>
        /// 根据前置任务的结束时间来获得开始时间
        /// </summary>
        /// <param name="end"></param>
        /// <returns></returns>
        public DateTime GetStartDateByPreTaskEndDate(DateTime end)
        {
            //if (IsOldData(end))
            //    return GetEndDate(end,2);
            //else
                return GetSuitableStartTime(end);
        }

        /// <summary>
        /// 根据后置任务的开始时间来获得结束时间
        /// </summary>
        /// <param name="end"></param>
        /// <returns></returns>
        public DateTime GetEndDateBySucTaskStartDate(DateTime begin)
        {
            //if (IsOldData(begin))
            //    return GetStartDate(begin, 2);
            //else
                return GetSuitableEndTime(begin);
        }


        public DateTime GetSuitableEndTime(DateTime begin)
        {
            //如果是下午上班时刻，就返回上午下班时刻
            if (IsTimeEqual(begin, secondHaftWorkStartTime))
                return SetWithFirstHaftWorkEndTime(begin);

            //如果是上午上班时刻，就返回上个工作日下午下班时刻
            if (IsTimeEqual(begin, firstHaftWorkStartTime))
            {
                var date = GetDate(begin.AddDays(-1), true);
                return SetWithSecondHaftWorkEndTime(date);
            }
            return begin;
        }



        /// <summary>
        /// 根据前置任务的结束时间、前置后置工时（前置任务的后置工时以及当前任务的前置工时之和），
        /// 来获得当前任务的开始时间
        /// </summary>
        public DateTime GetStartDateByPreTaskEndDateAndLearRearTime(DateTime date, double hours)
        {
            var begin = GetEndDateTime(date,hours);
            return GetSuitableStartTime(begin);
        }
    }
}
