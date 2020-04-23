namespace value
{
    public class BlockId : Id
    {
        public static readonly BlockId EMPTY = new BlockId(-1);

        public BlockId(int value) : base(value)
        {
        }
    }
}
