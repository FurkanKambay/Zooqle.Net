using Newtonsoft.Json;

namespace Zooqle.Net
{
    public class ZooqleItem
    {
        [JsonProperty("t", Required = Required.Always)]
        public readonly string Type;

        [JsonProperty("n", Required = Required.Always)]
        public readonly string Name;

        [JsonProperty("id", Required = Required.Always)]
        public readonly string Id;

        [JsonProperty("c", Required = Required.Always)]
        public readonly int Count;

        [JsonProperty("i", Required = Required.DisallowNull)]
        public readonly int ImageId;

        [JsonProperty("d", Required = Required.DisallowNull)]
        public readonly int Date;
    }
}
