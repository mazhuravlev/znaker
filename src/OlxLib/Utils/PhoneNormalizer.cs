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
        //                                                      Ru/Kz-Ua          By-Uz
        private static readonly Regex GoodNumber = new Regex(@"^((7|38)\d{10})|((375|998)\d{9})$");
        private static readonly Regex CleanRegex = new Regex(@"\D");
        private static readonly List<CountryRule> Rules = new List<CountryRule>
        {
            new CountryRule
            {
                Country = "Ua",
                Prefix = "38",
                Rule = new Regex(@"^(38)?(?<base>0(39|50|63|66|67|68|91|92|93|94|95|96|97|98|99)\d{7})$"),
                Weight = 1
            },
            new CountryRule
            {
                Country = "By",
                Prefix = "375",
                Rule = new Regex(@"^(375|80)(?<base>(24|25|29|33|44)\d{7})$"),
                Weight = 2
            },
            new CountryRule
            {
                Country = "Ru",
                Prefix = "7",
                Rule = new Regex(@"^(8|7)?(?<base>9(00|01|02|03|04|05|06|08|09|10|11|12|13|14|15|16|17|18|19|20|21|22|23|24|25|26|27|28|29|30|31|32|33|34|36|37|38|39|50|51|52|53|58|60|61|62|63|64|65|66|67|68|69|77|78|80|81|82|83|84|85|86|87|88|89|91|92|93|94|95|96|99)\d{7})$"),
                Weight = 2
            },
            new CountryRule
            {
                Country = "Kz",
                Prefix = "7",
                Rule = new Regex(@"^(8|7)?(?<base>7(00|01|02|05|07|27|47|71|75|76|77|78)\d{7})$"),
                Weight = 2
            }
        };


        
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

            if (GoodNumber.IsMatch(phone))
            {
                return phone;
            }


            foreach (var rule in Rules.OrderBy(c => c.Weight))
            {
                var match = rule.Rule.Match(phone);
                if (match.Success)
                {
                    return rule.Prefix + match.Groups["base"].Value;
                }
            }
            throw new FormatException("Что то непонятное тут, что с такими делать, будем добавлять?");
        }
        protected class CountryRule
        {
            public string Prefix;
            public string Country;
            public Regex Rule;
            public int Weight;
            public OlxType? OlxType;
        }
    }

    

}
