using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.DependencyInjection;
using PostgreSqlProvider;

namespace OlxLib.Workers
{
    public abstract class BaseWorker
    {
        private readonly IServiceProvider _serviceProvider;

        private static readonly List<OlxConfig> Configs = new List<OlxConfig>
        {
            new OlxConfig(OlxType.Ua, "http://olx.ua/sitemap.xml", "PlaceHereAdvdataurl{0}",
                "PlaceHereAdvcontacturl{0}"),
            new OlxConfig(OlxType.Uz, "http://olx.uz/sitemap.xml", "PlaceHereAdvdataurl{0}",
                "PlaceHereAdvcontacturl{0}"),
            new OlxConfig(OlxType.Kz, "http://www.olx.kz/sitemap.xml", "PlaceHereAdvdataurl{0}",
                "PlaceHereAdvcontacturl{0}"),
            new OlxConfig(OlxType.By, "https://www.olx.by/sitemap.xml", "PlaceHereAdvdataurl{0}",
                "PlaceHereAdvcontacturl{0}")
        };

        protected BaseWorker(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public static OlxConfig GetOlxConfig(OlxType type)
        {
            return Configs.First(c => c.OlxType == type);
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