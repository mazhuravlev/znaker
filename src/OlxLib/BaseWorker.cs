using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Hangfire.Storage;
using Microsoft.Extensions.DependencyInjection;
using PostgreSqlProvider;

namespace OlxLib
{
    public abstract class BaseWorker 
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly List<OlxConfig> _configs;

        protected BaseWorker(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
            _configs = new List<OlxConfig>
            {
                new OlxConfig(OlxType.Ua, "http://olx.ua/sitemap.xml", "PlaceHereAdvdataurl{0}",
                    "PlaceHereAdvcontacturl{0}"),

                new OlxConfig(OlxType.Uz, "http://olx.uz/sitemap.xml", "PlaceHereAdvdataurl{0}",
                    "PlaceHereAdvcontacturl{0}"),

                new OlxConfig(OlxType.Kz, "http://olx.kz/sitemap.xml", "PlaceHereAdvdataurl{0}",
                    "PlaceHereAdvcontacturl{0}")
            };




        }

        public OlxConfig GetOlxConfig(OlxType type)
        {
            return _configs.First(c => c.OlxType == type);
        }
        public ParserContext GetParserContext()
        {
            return _serviceProvider.GetService<ParserContext>();
        }
        public ZnakerContext GetZnakerContext()
        {
            return _serviceProvider.GetService<ZnakerContext>();
        }

    }
}
