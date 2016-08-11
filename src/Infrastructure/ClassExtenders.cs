using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Infrastructure
{
    public static class ExtensionsCollection
    {
        public static bool HasText(this string s)
        {
            return !(string.IsNullOrEmpty(s) || string.IsNullOrWhiteSpace(s));
        }
    }
    public static class DateTimeExtensions
    {
        public static double UnixTimeStamp(this DateTime date)
        {
            return Math.Floor((date - new DateTime(1970, 1, 1, 0, 0, 0, 0)).TotalSeconds);
        }
    }
}
