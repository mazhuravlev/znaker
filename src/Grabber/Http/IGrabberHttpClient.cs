using System.Net.Http;
using System.Threading.Tasks;

namespace Grabber.Http
{
    public interface IGrabberHttpClient
    {
        Task<HttpResponseMessage> GetAsync(string url);
    }
}