using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Reflection;
using System.Threading.Tasks;
using System.Xml;

namespace Zooqle.Net
{
    public class ZooqleClient
    {
        private static readonly HttpClient _httpClient = new HttpClient();

        /// <summary>
        /// Gets the first 30 search results for the given search terms.
        /// </summary>
        public async Task<ReadOnlyCollection<Torrent>> SearchAsync(string searchTerms)
        {
            if (searchTerms == null)
                throw new ArgumentNullException(nameof(searchTerms));

            var results = new List<Torrent>();

            if (!string.IsNullOrWhiteSpace(searchTerms) && searchTerms.Length > 1)
            {
                using (var stream = await _httpClient.GetStreamAsync($"https://zooqle.com/search?fmt=rss&q={searchTerms}"))
                using (var xml = XmlReader.Create(stream, new XmlReaderSettings { IgnoreWhitespace = true }))
                {
                    while (xml.ReadToFollowing("item"))
                    {
                        xml.ReadToDescendant("title");

#pragma warning disable IDE0017
                        var torrent = new Torrent();
                        torrent.Title = xml.ReadElementContentAsString();
                        xml.Skip(); // <description>
                        torrent.PageUrl = new Uri(xml.ReadElementContentAsString());
                        xml.Skip(); // <guid>
                        torrent.PublishDate = DateTimeOffset.Parse(xml.ReadElementContentAsString());
                        torrent.TorrentUrl = new Uri(xml.GetAttribute("url"));
                        xml.Skip(); // <enclosure>
                        torrent.Size = xml.ReadElementContentAsLong();
                        torrent.InfoHash = xml.ReadElementContentAsString();
                        torrent.MagnetUri = new Uri(xml.ReadElementContentAsString());
                        torrent.Seeds = xml.ReadElementContentAsInt();
                        torrent.Peers = xml.ReadElementContentAsInt();

                        results.Add(torrent);
                    }
                }
            }

            return new ReadOnlyCollection<Torrent>(results);
        }

        static ZooqleClient()
        {
            var v = Assembly.GetExecutingAssembly().GetName().Version;
            _httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("applicaton/rss+xml"));
            _httpClient.DefaultRequestHeaders.UserAgent.Add(new ProductInfoHeaderValue("Zooqle.Net", $"{v.Major}.{v.Minor}.{v.Build}"));
        }
    }
}
