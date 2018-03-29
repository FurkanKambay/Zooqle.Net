using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Reflection;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Zooqle.Net
{
    public static class ZooqleClient
    {
        internal const string zooqleSearchUrl = "https://zooqle.com/search";
        private static readonly HttpClient httpClient = new HttpClient();

        /// <summary>
        /// Retrieves the requested page of the search results for the given query.
        /// </summary>
        /// <param name="searchQuery">The search query.</param>
        /// <param name="page">The requested page number. Must be greater than 0.</param>
        /// <exception cref="HttpRequestException"/>
        public static async Task<SearchResult> SearchAsync(string searchQuery, int page = 1)
        {
            return string.IsNullOrWhiteSpace(searchQuery) || page < 1 ? SearchResult.Empty
                : GetSearchResultsFromXml(await httpClient.GetStringAsync(
                    $"{zooqleSearchUrl}?q={searchQuery}&pg={Math.Max(page, 1)}&fmt=rss").ConfigureAwait(false));
        }

        private static SearchResult GetSearchResultsFromXml(string xmlContent)
        {
            var channel = XDocument.Parse(xmlContent).Element("rss").Element("channel");

            XElement getOpenSearchElement(string localName) =>
                channel.Element(XName.Get(localName, "http://a9.com/-/spec/opensearch/1.1/"));

            string getTorrentValue(string localName, XElement source) =>
                source.Element(XName.Get(localName, "https://zooqle.com/xmlns/0.1/index.xmlns")).Value;

            return new SearchResult
            {
                SearchTerms = WebUtility.HtmlDecode(getOpenSearchElement("Query").Attribute("searchTerms").Value),
                SearchUrl = channel.Element("link").Value,
                TotalResultCount = int.Parse(getOpenSearchElement("totalResults").Value),
                StartIndex = int.Parse(getOpenSearchElement("startIndex").Value),
                ItemCountPerPage = int.Parse(getOpenSearchElement("itemsPerPage").Value),
                Results = channel.Elements("item").Select(item => new Torrent
                {
                    Title = item.Element("title").Value,
                    PageUrl = new Uri(item.Element("link").Value),
                    PublishDate = DateTime.Parse(item.Element("pubDate").Value),
                    TorrentUrl = new Uri(item.Element("enclosure").Attribute("url").Value),
                    Size = long.Parse(getTorrentValue("contentLength", item)),
                    InfoHash = getTorrentValue("infoHash", item),
                    MagnetUri = new Uri(getTorrentValue("magnetURI", item)),
                    SeedCount = int.Parse(getTorrentValue("seeds", item)),
                    PeerCount = int.Parse(getTorrentValue("peers", item))
                }).ToList().AsReadOnly()
            };
        }

        static ZooqleClient()
        {
            var v = Assembly.GetExecutingAssembly().GetName().Version;
            httpClient.DefaultRequestHeaders.Accept.ParseAdd("applicaton/rss+xml");
            httpClient.DefaultRequestHeaders.UserAgent.ParseAdd($"Zooqle.Net/{v.Major}.{v.Minor}.{v.Build}");
        }
    }
}
