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

        public static async Task<ReadOnlyCollection<ZooqleItem>> SearchItemAsync(string searchTerms)
        {
            if (string.IsNullOrWhiteSpace(searchTerms))
                return new ReadOnlyCollection<ZooqleItem>(new List<ZooqleItem>());

            var query = $"{zooqleItemSearchPath}/{searchTerms}";
            var response = await httpClient.GetStringAsync(query).ConfigureAwait(false);

            return JsonConvert.DeserializeObject<ReadOnlyCollection<ZooqleItem>>(response);
        }

        public static async Task<ZooqleItem> GetItemFromImdbIdAsync(string imdbId)
        {
            if (!IsImdbId(imdbId))
                return null;

            var items = await SearchItemAsync(imdbId);
            return items.FirstOrDefault();
        }
    }
}
