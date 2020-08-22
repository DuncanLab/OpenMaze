namespace value
{
    public class PickupId : Id
    {
        public static readonly PickupId EMPTY = new PickupId(-1);

        public PickupId(int value) : base(value)
        {
        }
    }
}
