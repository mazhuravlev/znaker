using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Infrastructure;

namespace OlxLib.Utils
{
    public class PhoneNormalizer
    {
        private static readonly Regex GoodNumber = new Regex(@"(7|38)\d{10}");
        private static readonly Regex UaMobile = new Regex(@"(38)?(?<base>0(39|50|63|66|67|68|91|92|93|94|95|96|97|98|99)\d{7})");
        private static readonly Regex RuMobile = new Regex(@"(8|7)?(?<base>9(00|01|02|03|04|05|06|08|09|10|11|12|13|14|15|16|17|18|19|20|21|22|23|24|25|26|27|28|29|30|31|32|33|34|36|37|38|39|50|51|52|53|58|60|61|62|63|64|65|66|67|68|69|77|78|80|81|82|83|84|85|86|87|88|89|91|92|93|94|95|96|99)\d{7})");

        private static readonly Regex CleanRegex = new Regex(@"\D");


        public static string Normalize(string phone)
        {
            if (!phone.HasText())
            {
                throw new ArgumentException("Phone number is null or empty");
            }

            phone = CleanRegex.Replace(phone, "");
            if (phone.Length < 10 && phone.Length > 12)
            {
                throw new FormatException("Ну и что с такими делать, будем добавлять?");
            }

            var uaMatch = UaMobile.Match(phone);
            if (uaMatch.Success)
            {
                return $"38{uaMatch.Groups["base"].Value}";
            }

            var ruMatch = RuMobile.Match(phone);
            if (ruMatch.Success)
            {
                return $"7{ruMatch.Groups["base"].Value}";
            }



            throw new FormatException("Ну и что с такими делать, будем добавлять?");
        }

    }
}
