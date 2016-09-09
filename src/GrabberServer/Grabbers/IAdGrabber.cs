using GrabberServer.Entities;
using Infrastructure;

namespace GrabberServer.Grabbers
{
    public interface IAdGrabber
    {
        AdGrabResult Grab(AdDownloadJob job);

        SourceType GetSourceType();
    }
}