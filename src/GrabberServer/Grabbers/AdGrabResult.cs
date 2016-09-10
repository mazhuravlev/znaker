using System.Collections.Generic;
using GrabberServer.Entities;
using Infrastructure;

namespace GrabberServer.Grabbers
{
    public class AdGrabResult
    {
        public AdDownloadJob DownloadJob;
        public string Text;
        public List<KeyValuePair<ContactType, string>> Contacts;
    }
}