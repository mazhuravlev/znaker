using System.Net.Http;
using System.Threading.Tasks;

namespace Grabber.Infrastructure
{
    public interface IHttpClient
    {
        Task<HttpResponseMessage> GetAsync(string url);
    }
}