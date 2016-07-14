using System;

namespace Trakkr.Console
{
    public class UnixTime
    {
        private static readonly DateTime UnixEpoch =
            new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);

        public static DateTime DateTimeFromUnixTimestampMillis(long millis)
        {
            return UnixEpoch.AddMilliseconds(millis);
        }

        public static long GetUnixTimestampMilliseconds(DateTime dateTime)
        {
            return (long)dateTime.Subtract(UnixEpoch).TotalMilliseconds;
        }
    }
}