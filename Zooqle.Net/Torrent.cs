using System;

namespace Zooqle.Net
{
    /// <summary>
    /// Data class for <see cref="https://zooqle.com/xmlns/0.1/index.xmlns"/>
    /// </summary>
    public class Torrent
    {
        /// <summary>
        /// Title of the torrent. XML property "title"
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// HEX/SHA1 value of the torrent metadata. XML property "torrent:infoHash"
        /// </summary>
        public string InfoHash { get; set; }

        /// <summary>
        /// Total size of the torrent in bytes. XML property "torrent:contentLength"
        /// </summary>
        public long Size { get; set; }

        /// <summary>
        /// Current count of seeders. XML property "torrent:seeds"
        /// </summary>
        public int Seeds { get; set; }

        /// <summary>
        /// Current count of leechers. XML property "torrent:peers"
        /// </summary>
        public int Peers { get; set; }

        /// <summary>
        /// The date on which the torrent was published. XML property "pubDate"
        /// </summary>
        public DateTimeOffset PublishDate { get; set; }

        /// <summary>
        /// Torrent's Zooqle page URL. XML property "link" and "guid"
        /// </summary>
        public Uri PageUrl { get; set; }

        /// <summary>
        /// Direct link to the torrent file. XML attribute "url" of property "enclosure"
        /// </summary>
        public Uri TorrentUrl { get; set; }

        /// <summary>
        /// Magnet link for the torrent. XML property "torrent:magnetURI"
        /// </summary>
        public Uri MagnetUri { get; set; }
    }
}
