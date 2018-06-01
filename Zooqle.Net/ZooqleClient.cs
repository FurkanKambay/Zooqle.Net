using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Xml.Linq;
using Zooqle.Net.Advanced;

namespace Zooqle.Net
{
    public static partial class ZooqleClient
    {
        internal const string zooqleBaseUrl = "https://zooqle.com/";
        internal const string zooqleTorrentSearchPath = "search";
        private static readonly HttpClient httpClient = new HttpClient { BaseAddress = new Uri(zooqleBaseUrl) };

        /// <summary>
        /// Searches for torrents with the given advanced query.
        /// </summary>
        /// <param name="searchQuery">The advanced search query.</param>
        /// <param name="page">The requested page number. Must be greater than 0.</param>
        /// <exception cref="HttpRequestException"/>
        /// <remarks>Info hashes and IMDb IDs are treated as exact-match searches.</remarks>
        public static async Task<SearchResult> SearchTorrentAsync(AdvancedQuery searchQuery, int page = 1) =>
            await SearchTorrentAsync(searchQuery.ToString(), page);

        /// <summary>
        /// Searches for torrents with the given search terms.
        /// </summary>
        /// <param name="searchTerms">The search terms.</param>
        /// <param name="page">The requested page number. Must be greater than 0.</param>
        /// <exception cref="HttpRequestException"/>
        /// <remarks>Info hashes and IMDb IDs are treated as exact-match searches.</remarks>
        public static async Task<SearchResult> SearchTorrentAsync(string searchTerms, int page = 1)
        {
            if (string.IsNullOrWhiteSpace(searchTerms) || page < 1)
                return SearchResult.Empty;

            if (IsImdbId(searchTerms) || IsInfoHash(searchTerms))
                searchTerms = $"\"{searchTerms}\"";

            var query = $"{zooqleTorrentSearchPath}?fmt=rss&q={searchTerms}&pg={page}";
            var xmlContent = await httpClient.GetStringAsync(query).ConfigureAwait(false);

            return GetSearchResults(xmlContent);
        }

        /// <summary>
        /// Finds the torrent with the given info hash.
        /// </summary>
        /// <param name="infoHash">The info hash in base-16 or base-32.</param>
        /// <returns>
        /// The page URL if the info hash is valid and the torrent exists.
        /// <see langword="null"/> if the torrent does not exist or the info hash is invalid.
        /// </returns>
        /// <exception cref="HttpRequestException"/>
        public static async Task<Uri> FindTorrentByInfoHashAsync(string infoHash)
        {
            if (IsInfoHash(infoHash))
            {
                var query = $"{zooqleTorrentSearchPath}?q=" + infoHash;
                var request = new HttpRequestMessage(HttpMethod.Head, query);
                var response = await httpClient.SendAsync(request).ConfigureAwait(false);

                if (request.RequestUri.AbsolutePath != $"/{zooqleTorrentSearchPath}")
                    return request.RequestUri;
            }
            return null;
        }

        private static SearchResult GetSearchResults(string xmlContent)
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
                Results = new ReadOnlyCollection<Torrent>(channel.Elements("item").Select(item => new Torrent
                {
                    Title = item.Element("title").Value,
                    PageUrl = new Uri(item.Element("link").Value),
                    PublishDate = DateTime.Parse(item.Element("pubDate").Value).ToUniversalTime(),
                    TorrentUrl = new Uri(item.Element("enclosure").Attribute("url").Value),
                    Size = long.Parse(getTorrentValue("contentLength", item)),
                    InfoHash = getTorrentValue("infoHash", item),
                    MagnetUri = new Uri(getTorrentValue("magnetURI", item)),
                    SeedCount = int.Parse(getTorrentValue("seeds", item)),
                    PeerCount = int.Parse(getTorrentValue("peers", item))
                }).ToList())
            };
        }

        private static bool IsInfoHash(string infoHash)
        {
            // Info hash: base-16 or base-32
            return IsMatch(infoHash.ToUpperInvariant(), @"(?:^[0-9A-F]{40}$)|(?:^[0-9A-V]{32}$)");
        }

        private static bool IsImdbId(string imdbId) =>
            IsMatch(imdbId, @"^tt[0-9]{7}$");

        private static bool IsMatch(string input, string pattern) =>
            (input != null && Regex.IsMatch(input, pattern));

        static ZooqleClient()
        {
            var headers = httpClient.DefaultRequestHeaders;
            headers.Accept.ParseAdd("applicaton/rss+xml");
            headers.Accept.ParseAdd("application/json");
            headers.Referrer = new Uri(zooqleBaseUrl);
            headers.UserAgent.ParseAdd($"Zooqle.Net/{ThisAssembly.Git.BaseTag.Substring(1)}");
        }
    }
}
