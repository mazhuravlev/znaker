using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Infrastructure
{
    public class TextUtils
    {
        private static readonly Regex CleanRegex = new Regex(@"[\s\n\t]+");

        public static string CleanSpacesAndNewlines(string text)
        {
            return CleanRegex.Replace(text, " ");
        }
    }
}