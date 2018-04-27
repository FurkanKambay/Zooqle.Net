namespace Zooqle.Net.Advanced
{
    public struct Size
    {
        public int Amount;
        public SizeUnit Unit;

        public Size(int amount, SizeUnit unit)
        {
            Amount = amount;
            Unit = unit;
        }

        public bool IsValid => Amount > 0 && Unit >= SizeUnit.KB && Unit <= SizeUnit.GB;
        public long Kilobytes => (long)Amount << (10 * (int)Unit);

        public override string ToString() => $"{Amount} {Unit}";
    }
}
