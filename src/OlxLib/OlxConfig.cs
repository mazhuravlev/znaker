namespace OlxLib
{
    public class OlxConfig
    {
        private readonly string _advertDataUrlTemplate;
        private readonly string _advertContactUrlTemplate;
        private readonly string _sitemapUrl;
        public readonly OlxType OlxType;

        public OlxConfig(OlxType olxType, string sitemapUrl, string advertDataUrlTemplate, string advertContactUrlTemplate)
        {
            _sitemapUrl = sitemapUrl;
            _advertDataUrlTemplate = advertDataUrlTemplate;
            _advertContactUrlTemplate = advertContactUrlTemplate;
            OlxType = olxType;
        }

        public string GetAdvertDataUrl(int id)
        {
            return string.Format(_advertDataUrlTemplate, id);
        }
        public string GetAdvertContactUrl(int id)
        {
            return string.Format(_advertContactUrlTemplate, id);
        }

        public string GetSitemapUrl()
        {
            return _sitemapUrl;
        }
    }
}
