using System;
using Newtonsoft.Json;

namespace Zooqle.Net
{
    public class ZooqleItem
    {
        [JsonConstructor]
        internal ZooqleItem(
            [JsonProperty("t", Required = Required.Always)] string typeString,
            [JsonProperty("id", Required = Required.Always)] string id,
            [JsonProperty("i", Required = Required.DisallowNull)] int imageId)
        {
            // ASCII value of the char (t is always a single char)
            Type = (ItemType)typeString[0];

            if (imageId > 0)
                Poster = new Poster(imageId);

            var type = Type.ToString().ToLowerInvariant();
            var tail = (Type == ItemType.Tv) ? "/" : ".html";
            PageUrl = new Uri($"https://zooqle.com/{type}/--{id}{tail}");
        }

        public readonly ItemType Type;
        public readonly Uri PageUrl;
        public readonly Poster Poster;

        [JsonProperty("n", Required = Required.Always)]
        public readonly string Name;

        [JsonProperty("c", Required = Required.Always)]
        public readonly int Count;

        [JsonProperty("d", Required = Required.DisallowNull)]
        public readonly int Date;

        public enum ItemType
        {
            Movie = 109,
            Tv = 116,
            Actor = 97
        }
    }
}
