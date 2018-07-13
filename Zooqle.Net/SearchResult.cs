using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Zooqle.Net
{
    // TODO SearchResult documentation

    public sealed class SearchResult
    {
        internal SearchResult() { }

        public override string ToString() => $@"Results for ""{SearchTerms}"" (page {PageNumber} of {PageCount})";

        public ReadOnlyCollection<Torrent> Results { get; internal set; }
        public int TotalResultCount { get; internal set; }
        public int StartIndex { get; internal set; }
        public int ItemCountPerPage { get; internal set; }
        public string SearchTerms { get; internal set; }
        public string SearchUrl { get; internal set; }

        public int PageCount => (TotalResultCount + ItemCountPerPage - 1) / ItemCountPerPage;
        public int PageNumber => (TotalResultCount == 0) ? 0 : (StartIndex / ItemCountPerPage + 1);

        public readonly static SearchResult Empty = new SearchResult
        {
            Results = new ReadOnlyCollection<Torrent>(new List<Torrent>()),
            ItemCountPerPage = 30,
            SearchTerms = string.Empty,
            SearchUrl = ZooqleClient.zooqleBaseUrl + ZooqleClient.zooqleTorrentSearchPath
        };
    }
}
