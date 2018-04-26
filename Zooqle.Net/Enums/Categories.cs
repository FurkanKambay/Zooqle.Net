namespace Zooqle.Net
{
    [System.Flags]
    public enum Categories
    {
        Any = 0 << 0,
        Movies = 1 << 0,
        TV = 1 << 1,
        Music = 1 << 2,
        Games = 1 << 3,
        Apps = 1 << 4,
        Books = 1 << 5,
        Anime = 1 << 6,
        Other = 1 << 7
    }
}
