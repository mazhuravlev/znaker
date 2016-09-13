using System.Net.Http;
using Grabber.Models;
using Infrastructure;

namespace Grabber.Infrastructure
{
    public interface IAdvertGrabber
    {
        AdvertJobResult Process(AdvertJob job);

        SourceType GetSourceType();
    }
}