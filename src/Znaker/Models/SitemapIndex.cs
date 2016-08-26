using System.Collections.Generic;

namespace Znaker.Models
{
    public class SitemapIndex
    {
        public List<Sitemap> Sitemaps;

        public class Sitemap
        {
            public string Loc;
            public string Lastmod;
        }
    }
}