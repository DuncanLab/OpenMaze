namespace value
{
    public class Id
    {
        protected Id(int value)
        {
            Value = value - 1;
        }

        public int Value { get; }

        public override string ToString()
        {
            return (Value + 1).ToString();
        }

        public override bool Equals(object obj)
        {
            if (obj == null) return false;

            return ((Id) obj).Value == Value;
        }

        public override int GetHashCode()
        {
            return Value;
        }
    }
}
