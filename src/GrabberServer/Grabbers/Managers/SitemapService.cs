using System;
using System.Collections.Generic;
using System.Linq;
using GrabberServer.Entities;
using Infrastructure;

namespace GrabberServer.Grabbers.Managers
{
    public class SitemapService
    {
        private readonly GrabberContext _grabberContext;

        public SitemapService()
        {
        }

        public SitemapService(GrabberContext grabberContext)
        {
            _grabberContext = grabberContext;
        }

        public List<SitemapEntry> GetSitemapsForType(SourceType sourceType)
        {
            return _grabberContext.SitemapEntries.Where(s => s.SourceType == sourceType).ToList();
        }

        public void SaveSitemaps(List<SitemapEntry> sitemapEntries)
        {
            // TODO: save and update sitemaps
            throw new NotImplementedException();
        }
    }
}