using System;
using System.Collections.Generic;

namespace Grabber.Infrastructure.Entries
{
    public class AdvertEntry
    {
        public IAdvertGrabber Grabber;
        public bool IsEnabled = true;
        public int JobsLimit = 1;
        public int RunningJobsCount = 0;
    }
}
