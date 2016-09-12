using System.Collections.Generic;
using Infrastructure;

namespace Grabber.Models
{
    public class AdGrabJobResult
    {
        public AdGrabJob Job;
        public string Text;
        public List<KeyValuePair<ContactType, string>> Contacts;
    }
}