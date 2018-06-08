using System;
using Newtonsoft.Json;

namespace Zooqle.Net
{
    public class ZooqleItem
    {
        [JsonConstructor]
        internal ZooqleItem(
            [JsonProperty("t", Required = Required.Always)] char typeChar,
            [JsonProperty("id", Required = Required.Always)] string id,
            [JsonProperty("i", Required = Required.DisallowNull)] int imageId)
        {
            Type = (ItemType)typeChar;

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
        public readonly int Year;

        public override string ToString()
        {
            string name, countThing;
            if (Type == ItemType.Actor)
            {
                name = Name;
                countThing = "movies";
            }
            else
            {
                name = $"{Name} ({Year})";
                countThing = "torrents";
            }

            return $"{Type} - {name} - {Count} {countThing}";
        }

        public enum ItemType
        {
            Movie = 'm',
            Tv = 't',
            Actor = 'a'
        }
    }
}
