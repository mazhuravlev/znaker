using System.Net.Http;
using GrabberServer.Entities;
using Infrastructure;

namespace GrabberServer.Grabbers
{
    public interface IAdGrabber
    {
        AdJobResult Grab(AdDownloadJob job);

        SourceType GetSourceType();
    }
}