using System.Collections.Generic;
using Infrastructure;

namespace Grabber.Models
{
    public class AdGrabJobResult
    {
        public string Text;
        public List<KeyValuePair<ContactType, string>> Contacts;
    }
}