using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyToPg
{
    public class Program
    {
        public static void Main(string[] args)
        {
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
            new Transfer().Run();
            Console.WriteLine("Complete!!!!!!");
            Console.Read();
        }
    }
}
