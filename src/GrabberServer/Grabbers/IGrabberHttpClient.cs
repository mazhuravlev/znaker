using System.Net.Http;
using System.Threading.Tasks;

namespace GrabberServer.Grabbers
{
    public interface IGrabberHttpClient
    {
        Task<HttpResponseMessage> GetAsync(string url);
    }
}