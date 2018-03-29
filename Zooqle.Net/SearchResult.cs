using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("Zooqle.Net.Tests")]

namespace Zooqle.Net
{
    // TODO Documentation
    [DebuggerDisplay("Search results for {SearchTerms} (page {PageNumber} of {TotalPageCount})")]
    public sealed class SearchResult
    {
        internal SearchResult() { }

        private static SearchResult empty;
        public static SearchResult Empty => empty ?? (empty = new SearchResult
        {
            Results = new List<Torrent>().AsReadOnly(),
            ItemCountPerPage = 30,
            SearchTerms = string.Empty,
            SearchUrl = ZooqleClient.zooqleSearchUrl + "?q=&amp;encode=1"
        });

        public ReadOnlyCollection<Torrent> Results { get; internal set; }
        public int TotalResultCount { get; internal set; }
        public int StartIndex { get; internal set; }
        public int ItemCountPerPage { get; internal set; }
        public string SearchTerms { get; internal set; }
        public string SearchUrl { get; internal set; }

        public int TotalPageCount => (int)Math.Ceiling((float)TotalResultCount / ItemCountPerPage);
        public int PageNumber => StartIndex / ItemCountPerPage + 1;
    }
}
