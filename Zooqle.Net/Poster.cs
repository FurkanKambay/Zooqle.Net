using System;

namespace Zooqle.Net
{
    public class Poster
    {
        internal Poster(int id) => this.id = id;

        private readonly int id;

        private Uri tiny;
        public Uri Tiny => tiny ?? (tiny = GetUri(2));

        private Uri small;
        public Uri Small => small ?? (small = GetUri(1));

        private Uri medium;
        public Uri Medium => medium ?? (medium = GetUri(3));

        private Uri large;
        public Uri Large => large ?? (large = GetUri(0));

        private Uri blurryCover;
        public Uri BlurryCover => blurryCover ?? (blurryCover = GetUri(10));

        private Uri GetUri(int sizeNumber) =>
            new Uri($"{ZooqleClient.BaseUrl}/img-{id}-{sizeNumber}.jpg");
    }
}
