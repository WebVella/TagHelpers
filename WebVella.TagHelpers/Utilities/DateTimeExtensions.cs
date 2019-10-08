
namespace System
{
	public  static class DateTimeExtensions
	{

		public static DateTime WvClearKind(this DateTime datetime)
		{
			return ((DateTime?)datetime).WvClearKind().Value;
		}

		public static DateTime? WvClearKind(this DateTime? datetime)
		{
			if (datetime == null)
				return null;

			return new DateTime(datetime.Value.Ticks, DateTimeKind.Unspecified);
		}

		public static DateTime WvConvertToTZDate(this DateTime datetime, string timeZoneName)
		{
			return ((DateTime?)datetime).WvConvertToTZDate(timeZoneName).Value;
		}

		public static DateTime? WvConvertToTZDate(this DateTime? datetime, string timeZoneName )
        {
			if (datetime == null)
				return null;

			TimeZoneInfo appTimeZone = TimeZoneInfo.FindSystemTimeZoneById(timeZoneName);
			return TimeZoneInfo.ConvertTimeBySystemTimeZoneId(datetime.Value, appTimeZone.Id);
		}

		public static DateTime WvConvertTZDateToUtc(this DateTime datetime, string timeZoneName)
		{
			return ((DateTime?)datetime).WvConvertTZDateToUtc(timeZoneName).Value;
		}

		public static DateTime? WvConvertTZDateToUtc(this DateTime? inputDate, string timeZoneName)
		{
			if (inputDate == null)
				return null;
			TimeZoneInfo appTimeZone = TimeZoneInfo.FindSystemTimeZoneById(timeZoneName);

			DateTime tmpDT = inputDate.Value;
			if (tmpDT.Kind == DateTimeKind.Utc)
				return tmpDT;
			else if (tmpDT.Kind == DateTimeKind.Local && appTimeZone != TimeZoneInfo.Local)
			{
				var convertedToAppZoneDate = TimeZoneInfo.ConvertTime(tmpDT, appTimeZone);
				return TimeZoneInfo.ConvertTimeToUtc(convertedToAppZoneDate, appTimeZone);
			}

			return TimeZoneInfo.ConvertTimeToUtc(inputDate.Value, appTimeZone);
		}

	}
}
