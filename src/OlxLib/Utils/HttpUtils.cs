using System.Net.Http;
using System.Threading.Tasks;

namespace OlxLib.Utils
{
    public class HttpUtils
    {
        public static async Task<string> DownloadString(string url)
        {
            var client = new HttpClient();
            return await client.GetStringAsync(url);
        }
    }
}