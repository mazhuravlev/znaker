using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace Grabber2.Infrastructure.Services.Logging
{
    public class LoggingModel
    {
        public Guid SessionId;
        public Guid ComponentId;
        public Guid? RelatedComponentId;
        public DateTime CreatedAt;
        public LogLevel Level;
        public string Message;
        public string Exception;
    }
}
