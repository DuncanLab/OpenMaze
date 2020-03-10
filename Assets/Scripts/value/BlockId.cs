namespace value
{
    public class BlockId : Id
    {
        public BlockId(int value) : base(value)
        {
        }
        
        public static readonly BlockId EMPTY = new BlockId(-1);
    }
}