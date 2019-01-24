using System;

namespace Zooqle.Net
{
    public class Poster
    {
        internal Poster(int id)
        {
            this.id = id;
            BlurryCover = GetUri(10);
            Large = GetUri(0);
            Medium = GetUri(3);
            Small = GetUri(1);
            Tiny = GetUri(2);
        }

        private readonly int id;
        public readonly Uri BlurryCover;
        public readonly Uri Large;
        public readonly Uri Medium;
        public readonly Uri Small;
        public readonly Uri Tiny;

        private Uri GetUri(int sizeNumber) =>
            new Uri($"{ZooqleClient.ZooqleBaseUrl}img-{id}-{sizeNumber}.jpg");
    }
}
