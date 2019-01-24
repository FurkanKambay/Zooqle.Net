using System;

namespace Zooqle.Net
{
    public class Poster
    {
        internal Poster(int id) => this.id = id;

        private readonly int id;
        private Uri tiny;
        private Uri small;
        private Uri medium;
        private Uri large;
        private Uri blurryCover;

        public Uri Tiny => tiny ?? (tiny = GetUri(2));
        public Uri Small => small ?? (small = GetUri(1));
        public Uri Medium => medium ?? (medium = GetUri(3));
        public Uri Large => large ?? (large = GetUri(0));
        public Uri BlurryCover => blurryCover ?? (blurryCover = GetUri(10));

        private Uri GetUri(int sizeNumber) =>
            new Uri($"{ZooqleClient.ZooqleBaseUrl}img-{id}-{sizeNumber}.jpg");
    }
}
