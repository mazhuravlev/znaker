using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Infrastructure;
using Microsoft.AspNetCore.Mvc;
using OlxLib;
using PostgreSqlProvider;

namespace OlxServer.Controllers
{
    public class HomeController : Controller
    {
        private readonly ParserContext _parserContext;
        private readonly ZnakerContext _znakerContext;

        public HomeController(ParserContext parserContext, ZnakerContext znakerContext)
        {
            _parserContext = parserContext;
            _znakerContext = znakerContext;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Stats()
        {
            var olxSources = new List<SourceType> {SourceType.OlxBy, SourceType.OlxKz, SourceType.OlxUa, SourceType.OlxUz};
            var model = new Dictionary<string, string>()
            {
                {"DownloadJobsCount", _parserContext.DownloadJobs.Count().ToString()},
                {"ExportJobsCount", _parserContext.ExportJobs.Count().ToString()},
                {"ZnakerOlxCount", _znakerContext.Entries.Count(e => olxSources.Contains(e.SourceId)).ToString()}
            };

            return View(model);
        }
    }
}