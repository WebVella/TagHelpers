
namespace System
{
	public  static class DateTimeExtensions
	{

		public static DateTime ClearKind(this DateTime datetime)
		{
			return ((DateTime?)datetime).ClearKind().Value;
		}

		public static DateTime? ClearKind(this DateTime? datetime)
		{
			if (datetime == null)
				return null;

			return new DateTime(datetime.Value.Ticks, DateTimeKind.Unspecified);
		}

		public static DateTime ConvertToTZDate(this DateTime datetime, string timeZoneName)
		{
			return ((DateTime?)datetime).ConvertToTZDate(timeZoneName).Value;
		}

		public static DateTime? ConvertToTZDate(this DateTime? datetime, string timeZoneName )
        {
			if (datetime == null)
				return null;

			TimeZoneInfo appTimeZone = TimeZoneInfo.FindSystemTimeZoneById(timeZoneName);
			return TimeZoneInfo.ConvertTimeBySystemTimeZoneId(datetime.Value, appTimeZone.Id);
		}

		public static DateTime ConvertTZDateToUtc(this DateTime datetime, string timeZoneName)
		{
			return ((DateTime?)datetime).ConvertTZDateToUtc(timeZoneName).Value;
		}

		public static DateTime? ConvertTZDateToUtc(this DateTime? inputDate, string timeZoneName)
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
