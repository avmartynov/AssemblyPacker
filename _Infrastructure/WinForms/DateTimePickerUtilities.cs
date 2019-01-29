using System;
using System.Windows.Forms;

namespace Twidlle.Infrastructure.WinForms
{
    public static class DateTimePickerUtilities
    {
        /// <summary> Время суток в формате DateTime. </summary>
        /// <remarks> Используется, когда надо редактировать промежуток времени с помощью контрола DateTimePicker. </remarks>
        public static DateTime ToDateTime(this TimeSpan timeOfDay)
        {
            return DateTimePicker.MinimumDateTime + timeOfDay;
        }

        /// <summary> Время суток в формате DateTime. </summary>
        /// <remarks> Используется, когда надо редактировать промежуток времени с помощью контрола DateTimePicker. </remarks>
        public static DateTime ToDateTime(this TimeSpan? timeOfDay)
        {
            return ToDateTime(timeOfDay ?? TimeSpan.Zero);
        }

        /// <summary> Время суток, полученное из контрола DateTimePicker и переключателя CheckBox. </summary>
        /// <param name="dateTime"> Значение даты в контроле DateTimePicker.</param>
        /// <param name="specified"> Признак необходимости использовать значение даты в контроле в качестве времени суток.</param>
        /// <returns></returns>
        public static TimeSpan? GetTimeOfDay(this DateTime dateTime, bool specified)
        {
            return specified ? new TimeSpan?(dateTime.TimeOfDay) : null;
        }
    }
}
