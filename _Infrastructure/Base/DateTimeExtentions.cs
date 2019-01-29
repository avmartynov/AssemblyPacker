using System;

namespace Twidlle.Infrastructure
{
    public static class DateTimeExtensions
    {
        /// <summary> Порядковый номер дня в неделе (0 - понедельник,... 6 - воскресенье) </summary>
        public static int GetDayIndexInWeek(this DateTime dateTime)
        {
            var weekDay = ((int) dateTime.DayOfWeek) - 1;
            return weekDay < 0 ? 6 : weekDay;
        }


        /// <summary> Неделя, содержащая текущую дату (округляет дату до недели) </summary>
        public static DateTime GetWeekDate(this DateTime dateTime)
        {
            var dayOfWeek = dateTime.GetDayIndexInWeek();
            return dateTime.Date - TimeSpan.FromDays(dayOfWeek);
        }


        /// <summary> Месяц, содержащий текущую дату (округляет дату до месяца)</summary>
        public static DateTime GetMonthDate(this DateTime dateTime)
        {
            var dayOfMonth = dateTime.Day - 1;
            return dateTime.Date - TimeSpan.FromDays(dayOfMonth);
        }


        /// <summary> Первая неделя месяца, содержащего текущую дату. 
        /// Неделя считается принадлежащей месяцу, если большинство дней (4) принадлежит месяцу. </summary>
        public static DateTime GetFirstWeekOfMonth(this DateTime dayDateTime)
        {
            var monthDateTime = dayDateTime.GetMonthDate();
            var dayIndex = monthDateTime.GetDayIndexInWeek();
            return (dayIndex < 4 ? monthDateTime : monthDateTime.AddDays(7)).GetWeekDate();
        }


        /// <summary> Проверка на выходной день (суббота или воскресенье). </summary>
        public static bool IsWeekEnd(this DateTime dateTime)
        {
            var idx = GetDayIndexInWeek(dateTime);
            return idx == 5 || idx == 6;
        }


        /// <summary> Проверка на принадлежность к определённому дню </summary>
        /// <param name="testDateTime"> Проверяемая дата. </param>
        /// <param name="dayDate"> Дата дня, на принадлежность к которому проводится проверка. </param>
        /// <returns>true - момент времени относится к заданному дню. </returns>
        public static bool BelongsToDay(this DateTime testDateTime, DateTime dayDate)
        {
            return testDateTime.Date == dayDate;
        }
    }
}
