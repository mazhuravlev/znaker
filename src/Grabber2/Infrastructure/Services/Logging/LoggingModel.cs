using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace Grabber2.Infrastructure.Services.Logging
{
    public class LoggingModel
    {
        public Guid ComponentId;
        public DateTime CreatedAt;
        public int Level;
        public int LogType;
        public string Message;
    }
}
