using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace ConsoleApp1
{
    public class Program
    {
        public static void Main(string[] args)
        {
            //var xdoc = XDocument.Load("map.xml");
            var sitemap = downloadSitemap(0).Result;
            var xdoc = XDocument.Parse(sitemap);
            var adIds = GetSitemapIds(xdoc);
            Console.ReadKey();
        }

        static async Task<string> downloadSitemap(int id)
        {
            var client = new System.Net.Http.HttpClient();
            var sitemapUrlTemplate = "http://olx.ua/sitemap-ads-{0}.xml";
            return await client.GetStringAsync(string.Format(sitemapUrlTemplate, id));
        }

        static List<long> GetSitemapIds(XDocument xdoc)
        {
            XNamespace ns = "http://www.sitemaps.org/schemas/sitemap/0.9";
            var regex = new Regex(@"ID([a-zA-Z0-9]+)");
            return xdoc.Root.Elements(ns + "url")
                .Select(u => u.Element(ns + "loc").Value)
                .Select(s => regex.Match(s).Groups[1].Value)
                .Select(s => DecryptOlxId(s))
                .ToList();
        }

        static long DecryptOlxId(string olxId)
        {
            return ArbitraryToDecimalSystem(SwapCase(olxId), 62);
        }

        static string SwapCase(string input)
        {
            return new string(input.Select(c => char.IsLetter(c) ? (char.IsUpper(c) ?
                       char.ToLower(c) : char.ToUpper(c)) : c).ToArray());
        }

        public static long ArbitraryToDecimalSystem(string number, int radix)
        {
            const string Digits = "0123456789abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ";
            if (radix < 2 || radix > Digits.Length)
            {
                throw new ArgumentException("The radix must be >= 2 and <= " +
                    Digits.Length.ToString());
            }
            if (String.IsNullOrEmpty(number))
            {
                throw new Exception();
            }
            long result = 0;
            long multiplier = 1;
            for (int i = number.Length - 1; i >= 0; i--)
            {
                char c = number[i];
                if (i == 0 && c == '-')
                {
                    result = -result;
                    break;
                }
                int digit = Digits.IndexOf(c);
                if (digit == -1)
                    throw new ArgumentException(
                        "Invalid character in the arbitrary numeral system number",
                        "number");
                result += digit * multiplier;
                multiplier *= radix;
            }
            return result;
        }
    }
}
