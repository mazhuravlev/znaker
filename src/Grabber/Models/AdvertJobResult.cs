using System.Collections.Generic;
using Infrastructure;

namespace Grabber.Models
{
    public class AdvertJobResult
    {
        public AdvertJob Job;
        public string Text;
        public List<KeyValuePair<ContactType, string>> Contacts;
    }
}