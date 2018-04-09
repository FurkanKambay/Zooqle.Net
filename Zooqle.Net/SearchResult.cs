using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;

namespace Zooqle.Net
{
    // TODO Documentation

    [DebuggerDisplay("Search results for {SearchTerms} (page {PageNumber} of {PageCount})")]
    public sealed class SearchResult
    {
        internal SearchResult() { }

        private static SearchResult empty;
        public static SearchResult Empty => empty ?? (empty = new SearchResult
        {
            Results = new List<Torrent>().AsReadOnly(),
            ItemCountPerPage = 30,
            SearchTerms = string.Empty,
            SearchUrl = ZooqleClient.zooqleSearchUrl
        });

        public ReadOnlyCollection<Torrent> Results { get; internal set; }
        public int TotalResultCount { get; internal set; }
        public int StartIndex { get; internal set; }
        public int ItemCountPerPage { get; internal set; }
        public string SearchTerms { get; internal set; }
        public string SearchUrl { get; internal set; }

        public int PageCount => (TotalResultCount + ItemCountPerPage - 1) / ItemCountPerPage;
        public int PageNumber => StartIndex / ItemCountPerPage + (StartIndex == 0 ? 0 : 1);
    }
}
