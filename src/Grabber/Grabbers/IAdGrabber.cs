using System.Net.Http;
using Grabber.Models;
using Infrastructure;

namespace Grabber.Grabbers
{
    public interface IAdGrabber
    {
        AdGrabJobResult Grab(AdGrabJob job);

        SourceType GetSourceType();
    }
}