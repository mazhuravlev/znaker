using System;
using System.Linq;

namespace GrabberServer.Grabbers.Olx
{
    public class IdUtils
    {
        public static long DecryptOlxId(string olxId)
        {
            return ArbitraryToDecimalSystem(SwapCase(olxId), 62);
        }

        private static string SwapCase(string input)
        {
            return new string(input.Select(c => char.IsLetter(c)
                ? (char.IsUpper(c)
                    ? char.ToLower(c)
                    : char.ToUpper(c))
                : c).ToArray());
        }

        private static long ArbitraryToDecimalSystem(string number, int radix)
        {
            const string digits = "0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz";
            if (radix < 2 || radix > digits.Length)
            {
                throw new ArgumentException("The radix must be >= 2 and <= " + digits.Length);
            }
            if (string.IsNullOrEmpty(number))
            {
                throw new Exception();
            }
            long result = 0;
            long multiplier = 1;
            for (var i = number.Length - 1; i >= 0; i--)
            {
                var c = number[i];
                if (i == 0 && c == '-')
                {
                    result = -result;
                    break;
                }
                var digit = digits.IndexOf(c);
                if (digit == -1)
                {
                    throw new ArgumentException("Invalid character in the arbitrary numeral system number",
                        nameof(number));
                }
                result += digit * multiplier;
                multiplier *= radix;
            }
            return result;
        }
    }
}