namespace value
{
    public class Id
    {
        protected Id(int value)
        {
            Value = value - 1;
        }

        public override string ToString()
        {
            return Value.ToString();
        }

        public int Value { get; }
    }
}