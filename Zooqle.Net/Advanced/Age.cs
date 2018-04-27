namespace Zooqle.Net.Advanced
{
    public struct Age
    {
        public int Amount;
        public TimeUnit Unit;
        public bool IsNewer;

        public Age(int amount, TimeUnit unit, bool isNewer)
        {
            Amount = amount;
            Unit = unit;
            IsNewer = isNewer;
        }

        public bool IsValid => Amount > 0 && Unit >= TimeUnit.Hour && Unit <= TimeUnit.Month;

        public override string ToString() => (IsNewer ? "Newer" : "Older") + $" than {Amount} {Unit}s";

        public static Age NewerThan(int amount, TimeUnit unit) => new Age(amount, unit, true);
        public static Age OlderThan(int amount, TimeUnit unit) => new Age(amount, unit, false);
    }
}
