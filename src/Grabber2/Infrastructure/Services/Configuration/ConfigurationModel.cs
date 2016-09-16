using System;

namespace Grabber2.Infrastructure.Services.Configuration
{
    public class ConfigurationModel
    {
        public bool AutoStart;
        public bool AutoRestart;

        private bool _requestStart;
        public bool RequestStart
        {
            set
            {
                _requestStart = value;
            }
            get
            {
                if (_requestStart)
                {
                    _requestStart = false;
                    return true;
                }
                return false;
            }
        }
        private bool _requestStop;
        public bool RequestStop
        {
            set
            {
                _requestStop = value;
            }
            get
            {
                if (_requestStop)
                {
                    _requestStop = false;
                    return true;
                }
                return false;
            }
        }

        public object Config;
        public DateTime UpdatedAt = DateTime.Now.AddYears(-1);
        
    }
}
