using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Zooqle.Net
{
    public static partial class ZooqleClient
    {
        internal const string zooqleItemSearchPath = "qss";

        /// <summary>
        /// Finds the movie, TV show, or actor from the given search terms.
        /// </summary>
        /// <param name="searchTerms">The search terms to search for. Can also be an IMDb ID.</param>
        /// <returns>
        /// If <paramref name="searchTerms"/> is not whitespace and the search turned up something,
        /// a readonly list of the results. Otherwise, an empty one.
        /// </returns>
        public static async Task<ReadOnlyCollection<ZooqleItem>> SearchItemAsync(string searchTerms)
        {
            if (string.IsNullOrWhiteSpace(searchTerms))
                return new ReadOnlyCollection<ZooqleItem>(new List<ZooqleItem>());

            var query = $"{zooqleItemSearchPath}/{searchTerms}";
            var response = await httpClient.GetStringAsync(query).ConfigureAwait(false);

            return JsonConvert.DeserializeObject<ReadOnlyCollection<ZooqleItem>>(response);
        }

        /// <summary>
        /// Finds the movie or TV show from an IMDb ID.
        /// </summary>
        /// <param name="imdbId">The IMDb ID that starts with "tt".</param>
        /// <returns>
        /// If <paramref name="imdbId"/> is invalid or the item wasn't found, <see langword="null"/>.
        /// Otherwise, the <see cref="ZooqleItem"/> that was found.
        /// </returns>
        public static async Task<ZooqleItem> GetItemFromImdbIdAsync(string imdbId)
        {
            if (!IsImdbId(imdbId))
                return null;

            var items = await SearchItemAsync(imdbId);
            return items.FirstOrDefault();
        }
    }
}
