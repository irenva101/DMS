using DMS.Shared.Constants;

namespace DMS.Shared.Helper
{
    public static class DateTimeHelper
    {
        public const string Iso8601DateTimeFormat = "yyyy-MM-ddTHH:mm:ss.fffZ";
        public const string Iso8601DateTimeFormatWithoutMilliseconds = "yyyy-MM-ddTHH:mm:ssZ";
        public static readonly string[] DlmsEventFormats = {
                "MM/dd/yyyy HH:mm:ss",
                "M/d/yyyy h:mm:ss tt"
            };
        public const string IsoDatePattern = "yyyy-MM-dd";
        public const string MMMyyyyDatePattern = "MMM-yyyy";
        public const string ReportDateTimePattern = "yyyy-MM-dd HH:mm:ss";
        public static long LocalDateTimeToUtcEpoch(DateTime meterTime, int timeZoneOffset)
        {
            var utcDateTime = meterTime.AddMinutes(timeZoneOffset);
            var meterEpochTime = new DateTimeOffset(utcDateTime, TimeSpan.Zero).ToUnixTimeSeconds();
            return meterEpochTime;
        }

        public static long UtcDateTimeToEpoch(DateTime meterTime)
        {
            var meterEpochTime = new DateTimeOffset(meterTime, TimeSpan.Zero).ToUnixTimeSeconds();
            return meterEpochTime;
        }
        public static DateTime CalculateMeterTime(this ClockSetType clockSetType, DateTime meterTime, int timeZone)
        {
            if (clockSetType == ClockSetType.Utc)
            {
                return meterTime;
            }

            return meterTime.AddMinutes(-timeZone);
        }
        public static DateTime LocalDateTimeToUtc(DateTime localTime, int timeZoneOffset)
        {
            return localTime.AddMinutes(timeZoneOffset);
        }

        // rounds a timestamp down to the start of its slotMinutes bucket
        // (e.g. slotMinutes = 15 maps 10:02 -> 10:00, 10:16 -> 10:15).
        public static DateTime FloorToSlot(DateTime value, int slotMinutes)
        {
            var slotTicks = TimeSpan.FromMinutes(slotMinutes).Ticks;
            return new DateTime(value.Ticks - (value.Ticks % slotTicks), DateTimeKind.Utc);
        }
    }
}
