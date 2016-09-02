using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace OlxLib.Utils
{
    public class TextUtils
    {
        public static string CleanSpacesAndNewlines(string s)
        {
            return Regex.Replace(s, @"[\t\n\r]+", " ");
        }
    }
}
